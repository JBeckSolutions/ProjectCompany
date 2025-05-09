using UnityEngine;
using UnityEngine.UIElements;

public class DeathUI : MonoBehaviour
{
    private VisualElement deathScreenRoot;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        deathScreenRoot = root.Q<VisualElement>("DeathScreen");
        this.Close();
    }

    public void Open()
    {
        deathScreenRoot.SetEnabled(true);
        deathScreenRoot.style.display = DisplayStyle.Flex;
    }

    public void Close()
    {
        deathScreenRoot.SetEnabled(false);
        deathScreenRoot.style.display = DisplayStyle.None;
    }
}



