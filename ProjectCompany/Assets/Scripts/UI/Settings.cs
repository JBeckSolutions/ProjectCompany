using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Slider = UnityEngine.UIElements.Slider;

public class Settings : MonoBehaviour
{
    private VisualElement settingsRoot;
    private DropdownField qualityDD;
    private Slider audioSlider;
    private Slider brightnessSlider;
    private Button backButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        settingsRoot = root.Q<VisualElement>("Options");

        Debug.Log("settingsRoot: " + settingsRoot);

        settingsRoot.SetEnabled(false);
        settingsRoot.style.display = DisplayStyle.None;

        qualityDD = root.Q<DropdownField>("QualityDD");
        audioSlider = root.Q<Slider>("AudioSlider");
        brightnessSlider = root.Q<Slider>("BrightnessSlider");
        backButton = root.Q<Button>("BackButton");

        // Register value change callbacks
        qualityDD.RegisterValueChangedCallback(evt =>
        {
            Debug.Log("Dropdown changed to: " + evt.newValue);
        });

        audioSlider.RegisterValueChangedCallback(evt =>
        {
            Debug.Log("Slider1 changed to: " + evt.newValue);
        });

        brightnessSlider.RegisterValueChangedCallback(evt =>
        {
            Debug.Log("Slider2 changed to: " + evt.newValue);
        });

        // Register button click event
        backButton.clicked += () => {
            settingsRoot.SetEnabled(false);
            settingsRoot.style.display = DisplayStyle.None;
            FindFirstObjectByType<MainMenu>().OpenMainMenu();
        };

    }

    public void OpenSetting()
    {
        settingsRoot.SetEnabled(true);
        settingsRoot.style.display = DisplayStyle.Flex;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
