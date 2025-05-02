using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Slider = UnityEngine.UIElements.Slider;

public class SettingsMenu : MonoBehaviour
{
    public bool DebugMode = false;
    //Visual Elements
    private VisualElement ui_document;
    private VisualElement settingsRoot;
    
    //UI Elements
    private DropdownField qualityDD;
    private Slider audioSlider;
    private Slider brightnessSlider;
    private Button backButton;
    
    [Header("Ui References")]
    public MainMenu ui_mainMenu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ui_document = GetComponent<UIDocument>().rootVisualElement;
        settingsRoot = ui_document.Q<VisualElement>("Options");
        ui_document.style.backgroundColor = Color.black;
        this.Close();
        
        if (DebugMode) Debug.Log("settingsRoot: " + settingsRoot);
        
        //UI Elements
        qualityDD = ui_document.Q<DropdownField>("QualityDD");
        audioSlider = ui_document.Q<Slider>("AudioSlider");
        brightnessSlider = ui_document.Q<Slider>("BrightnessSlider");
        backButton = ui_document.Q<Button>("BackButton");

        // Register value change callbacks
        qualityDD.RegisterValueChangedCallback(evt =>
        {
            if (DebugMode) Debug.Log("Dropdown changed to: " + evt.newValue);
        });

        audioSlider.RegisterValueChangedCallback(evt =>
        {
            if (DebugMode) Debug.Log("Slider1 changed to: " + evt.newValue);
        });

        brightnessSlider.RegisterValueChangedCallback(evt =>
        {
            if (DebugMode) Debug.Log("Slider2 changed to: " + evt.newValue);
        });

        // Register button click event
        backButton.clicked += () =>
        {
            this.Close();
            ui_mainMenu.Open();
            FindFirstObjectByType<MainMenu>().Open();
        };

    }

    public void Open()
    {
        ui_document.SetEnabled(true);
        settingsRoot.SetEnabled(true);
        ui_document.style.display = DisplayStyle.Flex;
        settingsRoot.style.display = DisplayStyle.Flex;
    }

    public void Close()
    {
        ui_document.SetEnabled(false);
        settingsRoot.SetEnabled(false);
        ui_document.style.display = DisplayStyle.None;
        settingsRoot.style.display = DisplayStyle.None;
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
