using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;
/// <summary>
/// The code of the this script was written by: Beck Jonas
/// </summary>
public class DropOffAreaManager : NetworkBehaviour
{
    //Tracks Items that are put inside the DropOffArea Collider

    public List<Item> ItemList;
    public int ItemValue = 0;

    [SerializeField] private TMP_Text quotaText;
    [SerializeField] private Collider DropOffArea;
    
    [Header("Wwise Events")]
    [SerializeField] private AK.Wwise.Event Event_DropOffArea_ScoreIncreased;
    [SerializeField] private AK.Wwise.Event Event_DropOffArea_ScoreDecreased;
    private void Start()
    {
        GameManager.Singelton.DropOffAreaManager = this;
        quotaText.text = new string(ItemValue.ToString() + "/" + GameManager.Singelton.Quota.Value.ToString());
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Item>() != null)
        {
            Item itemToAdd = other.GetComponent<Item>();
            if (!ItemList.Contains(itemToAdd))
            {
                ItemList.Add(itemToAdd);
                int buffer_ItemValue = ItemValue;
                CountValue();
                if (buffer_ItemValue < ItemValue)
                {
                    Event_DropOffArea_ScoreIncreased.Post(gameObject);
                }
                else if (buffer_ItemValue > ItemValue)
                {
                    Event_DropOffArea_ScoreDecreased.Post(gameObject);
                }
            }
        }
    }

    private void CountValue()
    {
        ItemValue = 0;

        foreach (var item in ItemList)
        {
            ItemValue += item.itemValue;
        }

        quotaText.text = new string(ItemValue.ToString() + "/" + GameManager.Singelton.Quota.Value.ToString());
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Item>() != null)
        {
            Item itemToRemove = other.GetComponent<Item>();
            if (ItemList.Contains(itemToRemove))
            {
                int buffer_ItemValue = ItemValue;
                ItemList.Remove(itemToRemove);
                CountValue();
                if (buffer_ItemValue < ItemValue)
                {
                    Event_DropOffArea_ScoreIncreased.Post(gameObject);
                }
                else if (buffer_ItemValue > ItemValue)
                {
                    Event_DropOffArea_ScoreDecreased.Post(gameObject);
                }
            }
        }
    }
}
