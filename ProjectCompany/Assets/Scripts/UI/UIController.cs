using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    private VisualElement root;
    private VisualElement mainMenu;
    private VisualElement settingsMenu;

    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        // Get both menu containers
        mainMenu = root.Q<VisualElement>("MainMenu");
        settingsMenu = root.Q<VisualElement>("Options");

        // Hide settings initially
        settingsMenu.style.display = DisplayStyle.None;

        // Main Menu buttons
        root.Q<Button>("NewGameButton").clicked += () =>
        {
            NetworkManager.Singleton.StartHost();
            ShowInventoryUI();
            HidePanel(mainMenu);
        };

        root.Q<Button>("JoinButton").clicked += () =>
        {
            NetworkManager.Singleton.StartClient();
            ShowInventoryUI();
            HidePanel(mainMenu);
        };

        root.Q<Button>("SettingsButton").clicked += () =>
        {
            ShowPanel(settingsMenu);
            HidePanel(mainMenu);
        };

        root.Q<Button>("QuitButton").clicked += () =>
        {
            Application.Quit();
        };

        // Settings controls
        root.Q<DropdownField>("QualityDD").RegisterValueChangedCallback(evt =>
        {
            Debug.Log("Quality changed to: " + evt.newValue);
        });

        root.Q<Slider>("AudioSlider").RegisterValueChangedCallback(evt =>
        {
            Debug.Log("Audio level: " + evt.newValue);
        });

        root.Q<Slider>("BrightnessSlider").RegisterValueChangedCallback(evt =>
        {
            Debug.Log("Brightness: " + evt.newValue);
        });

        root.Q<Button>("BackButton").clicked += () =>
        {
            ShowPanel(mainMenu);
            HidePanel(settingsMenu);
        };
    }

    private void ShowPanel(VisualElement panel)
    {
        panel.SetEnabled(true);
        panel.style.display = DisplayStyle.Flex;
    }

    private void HidePanel(VisualElement panel)
    {
        panel.SetEnabled(false);
        panel.style.display = DisplayStyle.None;
    }

    private void ShowInventoryUI()
    {
        // Call into your Interface.cs script or enable inventory logic
        FindFirstObjectByType<Interface>().EnableInventory();
    }
}

