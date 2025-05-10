using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerInventoryManager : MonoBehaviour
{
    public int PlayerWeight = 0;

    [SerializeField] private Item[] inventoryItems;

    [SerializeField] private int InventorySpace = 7;
    [SerializeField] private int ActiveInventorySlot = 0;
    
    public Transform playerHand;

    [SerializeField] private Transform dropLocation;

    [SerializeField] private InventoryUi inventoryUi;


    private void Start()
    {
        inventoryItems = new Item[InventorySpace];
    }

    public void AddItem(Item ItemToAdd)
    {
        if (inventoryItems[ActiveInventorySlot] == null)
        {
            if (ItemToAdd.PickupAble.Value == false) return;

            bool canBePickedUp = true;

            for (int i = 1; i <= ItemToAdd.ItemWeight - 1; i++)
            {
                if (inventoryItems[ActiveInventorySlot + i] != null)
                {
                    canBePickedUp = false;
                }
            }

            if (!canBePickedUp)
            {
                return;
            }

            ItemToAdd.PickUpServerRpc(this.GetComponent<NetworkObject>());

            for (int i = 0; i < ItemToAdd.ItemWeight; i++)
            {
                inventoryItems[ActiveInventorySlot + i] = ItemToAdd;
                inventoryUi.InventoryTiles[ActiveInventorySlot + i].SetItemImage(ItemToAdd.InventoryImage);
            }

            PlayerWeight += ItemToAdd.ItemWeight;
        }
    }
    public void DropItem()
    {
        if (inventoryItems[ActiveInventorySlot] != null)
        {
            inventoryItems[ActiveInventorySlot].DropServerRpc(dropLocation.position);

            GameObject itemToRemove = inventoryItems[ActiveInventorySlot].gameObject;

            PlayerWeight -= inventoryItems[ActiveInventorySlot].ItemWeight;

            for (int i = 0; i < inventoryItems.Length; i++)
            {
                if (inventoryItems[i] != null && inventoryItems[i].gameObject == itemToRemove)
                {
                    inventoryItems[i] = null;
                    inventoryUi.InventoryTiles[i].ResetItemImage();
                }
            }
        }
    }
    public void ChangeActiveInventorySlot(int? NewActiveInventorySlot = null, int? NextOrPreviousSlot = null)
    {
        if (inventoryItems[ActiveInventorySlot] != null) //Set item Model to be invisible
        {
            inventoryItems[ActiveInventorySlot].ToggleVisibilityServerRpc(false);
        }

        inventoryUi.InventoryTiles[ActiveInventorySlot].Selected.enabled = false;

        int foundInventorySlot = ActiveInventorySlot;

        if (NewActiveInventorySlot == null && NextOrPreviousSlot != null)
        {
            int direction = NextOrPreviousSlot.Value; // +1 or -1
            int slotToCheck = ActiveInventorySlot;
            bool foundDifferentItem = false;

            do
            {
                slotToCheck += direction;

                // Wrap around
                if (slotToCheck >= InventorySpace)
                {
                    slotToCheck = 0;
                }
                else if (slotToCheck < 0)
                {
                    slotToCheck = InventorySpace - 1;
                }


                if (slotToCheck == ActiveInventorySlot)
                {
                    // Failsave
                    break;
                }

                if (inventoryItems[slotToCheck] == null)
                {
                    foundDifferentItem = true;
                    foundInventorySlot = slotToCheck;
                }
                else if (inventoryItems[slotToCheck] != inventoryItems[ActiveInventorySlot])
                {
                    for (int i = 0; i < inventoryItems.Length; i++)
                    {
                        if (inventoryItems[i] == inventoryItems[slotToCheck])
                        {
                            foundDifferentItem = true;
                            foundInventorySlot = i;
                            break;
                        }
                    }
                }

            } while (!foundDifferentItem);
        }
        else if (NewActiveInventorySlot != null)
        {
            foundInventorySlot = NewActiveInventorySlot.Value;
        }

        ActiveInventorySlot = foundInventorySlot;

        inventoryUi.InventoryTiles[ActiveInventorySlot].Selected.enabled = true;

        if (inventoryItems[ActiveInventorySlot] != null) //Set item model to be visible if player is now holding something
        {
            inventoryItems[ActiveInventorySlot].ToggleVisibilityServerRpc(true);
        }
        
    }
}
