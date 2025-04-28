using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerInventoryManager : MonoBehaviour
{
    public int PlayerWeight = 0;

    [SerializeField] private Item[] inventoryItems;
    //[SerializeField] private RawImage[] inventroyImages;
    //[SerializeField] private RawImage[] activeInventroySlot;
    [SerializeField] private int InventorySpace = 7;
    [SerializeField] private int ActiveInventorySlot = 0;
    
    public Transform playerHand;
    [SerializeField] private Transform dropLocation;

    private StyleColor activeColor = Color.blue;
    private StyleColor oldColor;
    private VisualElement[] slots;
    private UnityEngine.UIElements.StyleBackground defaultImage;

    private void Start()
    {
        inventoryItems = new Item[InventorySpace];
        slots = new VisualElement[InventorySpace];
        //inventroyImages = new RawImage[InventorySpace-1];
        //activeInventroySlot = new RawImage[InventorySpace - 1];

        var root = GameObject.Find("Interface").GetComponent<UIDocument>().rootVisualElement;
        slots[0] = root.Q<VisualElement>("Slot1");
        slots[1] = root.Q<VisualElement>("Slot2");
        slots[2] = root.Q<VisualElement>("Slot3");
        slots[3] = root.Q<VisualElement>("Slot4");
        slots[4] = root.Q<VisualElement>("Slot5");
        slots[5] = root.Q<VisualElement>("Slot6");
        slots[6] = root.Q<VisualElement>("Slot7");

        defaultImage = slots[0].style.backgroundImage;

        oldColor = slots[0].style.color;
        ChangeBorderColor(slots[0], true);
        /*
        inventroyImages[0] = GameObject.Find("InventorySlotOne").GetComponent<RawImage>();
        inventroyImages[1] = GameObject.Find("InventorySlotTwo").GetComponent<RawImage>();
        inventroyImages[2] = GameObject.Find("InventorySlotThree").GetComponent<RawImage>();
        inventroyImages[3] = GameObject.Find("InventorySlotFour").GetComponent<RawImage>();
        inventroyImages[4] = GameObject.Find("InventorySlotFive").GetComponent<RawImage>();
        inventroyImages[5] = GameObject.Find("InventorySlotSix").GetComponent<RawImage>();
        inventroyImages[6] = GameObject.Find("InventorySlotSeven").GetComponent<RawImage>();

        activeInventroySlot[0] = GameObject.Find("InventorySlotOneActive").GetComponent<RawImage>();
        activeInventroySlot[1] = GameObject.Find("InventorySlotTwoActive").GetComponent<RawImage>();
        activeInventroySlot[2] = GameObject.Find("InventorySlotThreeActive").GetComponent<RawImage>();
        activeInventroySlot[3] = GameObject.Find("InventorySlotFourActive").GetComponent<RawImage>();
        activeInventroySlot[4] = GameObject.Find("InventorySlotFive").GetComponent<RawImage>();
        activeInventroySlot[5] = GameObject.Find("InventorySlotSix").GetComponent<RawImage>();
        activeInventroySlot[6] = GameObject.Find("InventorySlotSeven").GetComponent<RawImage>();
        */
    }

    public void ChangeBorderColor(VisualElement slot, bool active)
    {
        if (active)
        {
            slot.style.borderBottomColor = activeColor;
            slot.style.borderLeftColor = activeColor;
            slot.style.borderRightColor = activeColor;
            slot.style.borderTopColor = activeColor;
        }
        else
        {
            slot.style.borderBottomColor = oldColor;
            slot.style.borderLeftColor = oldColor;
            slot.style.borderRightColor = oldColor;
            slot.style.borderTopColor = oldColor;
        }     
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
                slots[ActiveInventorySlot + i].style.backgroundImage = ItemToAdd.InventoryImage;
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
                    slots[i].style.backgroundImage = defaultImage;
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

        ChangeBorderColor(slots[ActiveInventorySlot], false);

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

        ChangeBorderColor(slots[ActiveInventorySlot], true);

        if (inventoryItems[ActiveInventorySlot] != null) //Set item model to be visible if player is now holding something
        {
            inventoryItems[ActiveInventorySlot].ToggleVisibilityServerRpc(true);
        }
        
    }
}
