using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerInventoryManager : MonoBehaviour
{
    [SerializeField] private Item[] inventoryItems;
    [SerializeField] private int InventorySpace = 3;
    [SerializeField] private int ActiveInvetorySlot = 0;
    public Transform playerHand;
    [SerializeField] private Transform dropLocation;

    private void Start()
    {
        inventoryItems = new Item[InventorySpace];
    }
    public void AddItem(Item ItemToAdd)
    {
        if (inventoryItems[ActiveInvetorySlot] == null)
        {
            if (ItemToAdd.PickupAble.Value == false) return;
            ItemToAdd.PickUpServerRpc(this.GetComponent<NetworkObject>());
            inventoryItems[ActiveInvetorySlot] = ItemToAdd;
        }
    }
    public void DropItem()
    {
        if (inventoryItems[ActiveInvetorySlot] != null)
        {
            inventoryItems[ActiveInvetorySlot].DropServerRpc(dropLocation.position);
        }
    }
    public void ChangeActiveInventorySlot(int NewActiveInventorySlot)
    {
        if (inventoryItems[ActiveInvetorySlot] != null) //Set item Model to be invisible if Player is holding something at the moment
        {
            inventoryItems[ActiveInvetorySlot].ToggleVisibilityServerRpc(false);
        }

        ActiveInvetorySlot = NewActiveInventorySlot;   //Change active inventroy Slot

        if (ActiveInvetorySlot < 0)     //Check if slot is valid
        {
            ActiveInvetorySlot = 0;
        }
        else if (ActiveInvetorySlot > InventorySpace - 1)
        {
            ActiveInvetorySlot = InventorySpace - 1;
        }

        if (inventoryItems[ActiveInvetorySlot] != null) //Set item model to be visible if player is now holding something
        {
            inventoryItems[ActiveInvetorySlot].ToggleVisibilityServerRpc(true);
        }
        
    }
}
