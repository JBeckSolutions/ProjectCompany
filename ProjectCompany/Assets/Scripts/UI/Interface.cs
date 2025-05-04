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
        this.Close();
    }

    public void Open()
    {
        inventoryRoot.SetEnabled(true);
        headsUpRoot.SetEnabled(true);
        
        inventoryRoot.style.display = DisplayStyle.Flex;
        headsUpRoot.style.display = DisplayStyle.Flex;
    }

    public void Close()
    {
        inventoryRoot.SetEnabled(false);
        headsUpRoot.SetEnabled(false);
        
        inventoryRoot.style.display = DisplayStyle.None;
        headsUpRoot.style.display = DisplayStyle.None;
    }
}
