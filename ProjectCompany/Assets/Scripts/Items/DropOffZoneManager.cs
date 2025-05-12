using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class DropOffAreaManager : NetworkBehaviour
{
    //Tracks Items that are put inside the DropOffArea Collider

    public List<Item> ItemList;
    public int ItemValue = 0;

    [SerializeField] private TMP_Text quotaText;
    [SerializeField] private Collider DropOffArea;
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
                CountValue();
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
                ItemList.Remove(itemToRemove);
                CountValue();
            }
        }
    }
}
