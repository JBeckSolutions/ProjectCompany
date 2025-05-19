using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerInventoryManager : NetworkBehaviour
{
    [Header("Player Inventory Settings")]
    public int PlayerWeight = 0;

    [Header("Inventory Items")]
    public List<Item> serverInventoryItems;
    private Item[] inventoryItems;

    [Header("Inventory Configuration")]
    private int InventorySpace = 7;
    private int ActiveInventorySlot = 0;

    [Header("Inventory Interaction")]
    public Transform playerHand;

    [Header("Item Drop Settings")]
    public Transform dropLocation;

    [Header("UI Components")]
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
            AddItemOnServerRpc(ItemToAdd.GetComponent<NetworkObject>());

            inventoryItems[ActiveInventorySlot] = ItemToAdd;
            inventoryUi.InventoryTiles[ActiveInventorySlot].SetItemImage(ItemToAdd.InventoryImage);

            for (int i = 1; i < ItemToAdd.ItemWeight; i++)
            {
                inventoryItems[ActiveInventorySlot + i] = ItemToAdd;
                inventoryUi.InventoryTiles[ActiveInventorySlot + i].SetItemImage(ItemToAdd.InventoryImage, new Color(1,1,1,0.3f));
            }

            PlayerWeight += ItemToAdd.ItemWeight;
        }
    }
    [ServerRpc]
    private void AddItemOnServerRpc(NetworkObjectReference ItemNetworkId)
    {
        Debug.Log("Adding item to server list");
        if (ItemNetworkId.TryGet(out NetworkObject item))
        {
            serverInventoryItems.Add(item.gameObject.GetComponent<Item>());
        }
    }
    public void DropItem()
    {
        if (inventoryItems[ActiveInventorySlot] != null)
        {
            Vector3 dropPositon = dropLocation.position;
            dropPositon.y = 0.2f;

            inventoryItems[ActiveInventorySlot].DropServerRpc(dropPositon);
            RemoveItemOnServerRpc(inventoryItems[ActiveInventorySlot].GetComponent<NetworkObject>());

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
            //[WwiseCall]
        }
    }
    [ServerRpc]
    private void RemoveItemOnServerRpc(NetworkObjectReference ItemNetworkId)
    {
        Debug.Log("Removing item from server list");
        if (ItemNetworkId.TryGet(out NetworkObject item))
        {
            serverInventoryItems.Remove(item.gameObject.GetComponent<Item>());
        }
    }
    public void DropAllItems()
    {
        for (int i = 0; i < inventoryItems.Length; i++)
        {
            ActiveInventorySlot = i;
            DropItem();
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
    //[WwiseCall]
}
