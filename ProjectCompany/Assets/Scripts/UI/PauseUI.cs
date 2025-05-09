using UnityEngine;
using UnityEngine.UIElements;

public class PauseUI : MonoBehaviour
{
    private VisualElement pauseMenuRoot;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        pauseMenuRoot = root.Q<VisualElement>("PauseMenu");
        this.Close();
    }

    public void Open()
    {
        pauseMenuRoot.SetEnabled(true);
        pauseMenuRoot.style.display = DisplayStyle.Flex;
    }

    public void Close()
    {
        pauseMenuRoot.SetEnabled(false);
        pauseMenuRoot.style.display = DisplayStyle.None;
    }
}
