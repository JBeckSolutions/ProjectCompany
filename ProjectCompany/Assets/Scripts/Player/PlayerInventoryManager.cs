using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventoryManager : MonoBehaviour
{
    [SerializeField] private Item[] inventoryItems;
    [SerializeField] private RawImage[] inventroyImages;
    [SerializeField] private RawImage[] activeInventroySlot;
    [SerializeField] private int InventorySpace = 3;
    [SerializeField] private int ActiveInvetorySlot = 0;
    
    public Transform playerHand;
    [SerializeField] private Transform dropLocation;

    private void Start()
    {
        inventoryItems = new Item[InventorySpace];
        inventroyImages = new RawImage[InventorySpace];
        activeInventroySlot = new RawImage[InventorySpace];
        
        inventroyImages[0] = GameObject.Find("InventorySlotOne").GetComponent<RawImage>();
        inventroyImages[1] = GameObject.Find("InventorySlotTwo").GetComponent<RawImage>();
        inventroyImages[2] = GameObject.Find("InventorySlotThree").GetComponent<RawImage>();
        inventroyImages[3] = GameObject.Find("InventorySlotFour").GetComponent<RawImage>();

        activeInventroySlot[0] = GameObject.Find("InventorySlotOneActive").GetComponent<RawImage>();
        activeInventroySlot[1] = GameObject.Find("InventorySlotTwoActive").GetComponent<RawImage>();
        activeInventroySlot[2] = GameObject.Find("InventorySlotThreeActive").GetComponent<RawImage>();
        activeInventroySlot[3] = GameObject.Find("InventorySlotFourActive").GetComponent<RawImage>();
    }
    public void AddItem(Item ItemToAdd)
    {
        if (inventoryItems[ActiveInvetorySlot] == null)
        {
            if (ItemToAdd.PickupAble.Value == false) return;
            ItemToAdd.PickUpServerRpc(this.GetComponent<NetworkObject>());
            inventoryItems[ActiveInvetorySlot] = ItemToAdd;
            inventroyImages[ActiveInvetorySlot].texture = ItemToAdd.InventoryImage;
        }
    }
    public void DropItem()
    {
        if (inventoryItems[ActiveInvetorySlot] != null)
        {
            inventoryItems[ActiveInvetorySlot].DropServerRpc(dropLocation.position);
            inventoryItems[ActiveInvetorySlot] = null;
            inventroyImages[ActiveInvetorySlot].texture = null;
        }
    }
    public void ChangeActiveInventorySlot(int NewActiveInventorySlot)
    {
        if (inventoryItems[ActiveInvetorySlot] != null) //Set item Model to be invisible if Player is holding something at the moment
        {
            inventoryItems[ActiveInvetorySlot].ToggleVisibilityServerRpc(false);
        }

        activeInventroySlot[ActiveInvetorySlot].enabled = false;

        ActiveInvetorySlot = NewActiveInventorySlot;   //Change active inventroy Slot

        activeInventroySlot[ActiveInvetorySlot].enabled = true;

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
