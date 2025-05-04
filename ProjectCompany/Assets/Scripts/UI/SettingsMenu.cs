using UnityEngine;
using UnityEngine.UIElements;
using UI.HoverButton;


using Slider = UnityEngine.UIElements.Slider;

public class SettingsMenu : MonoBehaviour
{
    public bool DebugMode = false;
    //Visual Elements
    private VisualElement ui_document;
    private VisualElement settingsRoot;
    private VisualElement backGroundElement;
    
    //UI Elements
    private DropdownField qualityDD;
    private Slider audioSlider;
    private Slider brightnessSlider;
    private HoverButton backButton;
    
    [Header("Ui References")]
    public MainMenu ui_mainMenu;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ui_document = GetComponent<UIDocument>().rootVisualElement;
        backGroundElement = ui_document.Q<VisualElement>("Background");
        settingsRoot = backGroundElement.Q<VisualElement>("Options");
        
        
        this.Close();
        
        if (DebugMode) Debug.Log("settingsRoot: " + settingsRoot);
        
        //UI Elements
        qualityDD = ui_document.Q<DropdownField>("QualityDD");
        audioSlider = ui_document.Q<Slider>("AudioSlider");
        brightnessSlider = ui_document.Q<Slider>("BrightnessSlider");
        backButton = ui_document.Q<HoverButton>("BackButton");

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
            if (DebugMode) Debug.Log("Back button clicked");
            
            this.Close();
            ui_mainMenu.Open();
        };
        backButton.hovered += () =>
        {
            if (DebugMode) Debug.Log("Back button hovered");
            
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
