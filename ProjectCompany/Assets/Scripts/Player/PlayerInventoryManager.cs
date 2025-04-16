using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerInventoryManager : MonoBehaviour
{
    [SerializeField] private Item[] inventoryItems;
    //[SerializeField] private RawImage[] inventroyImages;
    //[SerializeField] private RawImage[] activeInventroySlot;
    [SerializeField] private int InventorySpace = 7;
    [SerializeField] private int ActiveInvetorySlot = 0;
    
    public Transform playerHand;
    [SerializeField] private Transform dropLocation;

    private StyleColor activeColor = Color.blue;
    private StyleColor oldColor;
    [SerializeField] private VisualElement[] slots;
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
        if (inventoryItems[ActiveInvetorySlot] == null)
        {
            if (ItemToAdd.PickupAble.Value == false) return;
            ItemToAdd.PickUpServerRpc(this.GetComponent<NetworkObject>());
            inventoryItems[ActiveInvetorySlot] = ItemToAdd;
            slots[ActiveInvetorySlot].style.backgroundImage = ItemToAdd.InventoryImage;
        }
    }
    public void DropItem()
    {
        if (inventoryItems[ActiveInvetorySlot] != null)
        {
            inventoryItems[ActiveInvetorySlot].DropServerRpc(dropLocation.position);
            inventoryItems[ActiveInvetorySlot] = null;
            slots[ActiveInvetorySlot].style.backgroundImage = defaultImage;
        }
    }
    public void ChangeActiveInventorySlot(int NewActiveInventorySlot)
    {
        if (inventoryItems[ActiveInvetorySlot] != null) //Set item Model to be invisible if Player is holding something at the moment
        {
            inventoryItems[ActiveInvetorySlot].ToggleVisibilityServerRpc(false);
        }

        ChangeBorderColor(slots[ActiveInvetorySlot], false);

        ActiveInvetorySlot = NewActiveInventorySlot;   //Change active inventroy Slot

        ChangeBorderColor(slots[ActiveInvetorySlot], true);

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
