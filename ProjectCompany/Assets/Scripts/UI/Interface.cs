using UnityEngine;
using UnityEngine.UIElements;

public class Interface : MonoBehaviour
{
    private VisualElement inventoryRoot;
    private VisualElement headsUpRoot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        inventoryRoot = root.Q<VisualElement>("Inventory");
        headsUpRoot = root.Q<VisualElement>("HeadsUpDisplay");
        inventoryRoot.SetEnabled(false);
        headsUpRoot.SetEnabled(false);
        inventoryRoot.style.display = DisplayStyle.None;
        headsUpRoot.style.display = DisplayStyle.None;
    }

    public void EnableInventory()
    {
        inventoryRoot.SetEnabled(true);
        inventoryRoot.style.display = DisplayStyle.Flex;
        headsUpRoot.SetEnabled(true);
        headsUpRoot.style.display = DisplayStyle.Flex;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
