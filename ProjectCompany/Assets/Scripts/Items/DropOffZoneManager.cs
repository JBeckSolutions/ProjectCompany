using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DropOffAreaManager : NetworkBehaviour
{
    //Tracks Items that are put inside the DropOffArea Collider

    public List<Item> ItemList;

    [SerializeField] private Collider DropOffArea;
    public int ItemValue = 0;
    private void Start()
    {
        GameManager.Singelton.DropOffAreaManager = this;
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
