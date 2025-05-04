using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    private VisualElement ui_MainMenuRoot;
    private VisualElement mainMenuRootQuery;
    
    public bool DebugMode = false;
    
    [Header("UI References")]
    public Interface ui_Interface;
    public SettingsMenu ui_SettingsMenu;
    
    

    //Wwise Events from Inspector
    [Header("Wwise MainMenu Button Click")]
    public AK.Wwise.Event MainMenu_UI_Button_Click_NewGame;
    public AK.Wwise.Event MainMenu_UI_Button_Click_JoinGame;
    public AK.Wwise.Event MainMenu_UI_Button_Click_Settings;
    public AK.Wwise.Event MainMenu_UI_Button_Click_QuitGame;

    [Header("Wwise MainMenu Button Hover")]
    public AK.Wwise.Event MainMenu_UI_Button_Hover_NewGame;
    public AK.Wwise.Event MainMenu_UI_Button_Hover_JoinGame;
    public AK.Wwise.Event MainMenu_UI_Button_Hover_Settings;
    public AK.Wwise.Event MainMenu_UI_Button_Hover_QuitGame;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ui_MainMenuRoot = GetComponent<UIDocument>().rootVisualElement;
        mainMenuRootQuery = ui_MainMenuRoot.Q<VisualElement>("Background");
        
        var newGameHoverButton = mainMenuRootQuery.Q<HoverButton>("NewGameButton");
        newGameHoverButton.clicked += () =>
        {
            if (DebugMode) Debug.Log("NewGameButton clicked");
            MainMenu_UI_Button_Click_NewGame.Post(gameObject);
            NetworkManager.Singleton.StartHost();
            this.Close();
            ui_Interface.Open();
        };
        newGameHoverButton.hovered += () =>
        {
            if (DebugMode) Debug.Log("NewGameButton hovered");
            MainMenu_UI_Button_Hover_NewGame.Post(gameObject);
        };

        var joinButton = mainMenuRootQuery.Q<HoverButton>("JoinButton");
        joinButton.clicked += () =>
        {
            if (DebugMode) Debug.Log("JoinButton clicked");
            MainMenu_UI_Button_Click_JoinGame.Post(gameObject);
            NetworkManager.Singleton.StartClient(); //maybe transition to a loading screen here
            this.Close();
            ui_Interface.Open();
        };
        joinButton.hovered += () =>
        {
            if (DebugMode) Debug.Log("JoinButton hovered");
            MainMenu_UI_Button_Hover_JoinGame.Post(gameObject);
        };

        var settingsButton = mainMenuRootQuery.Q<HoverButton>("SettingsButton");
        settingsButton.clicked += () =>
        {
            if (DebugMode) Debug.Log("SettingsButton clicked");
            MainMenu_UI_Button_Click_Settings.Post(gameObject);
            this.Close();
            ui_SettingsMenu.Open();
        };
        settingsButton.hovered += () =>
        {
            if (DebugMode) Debug.Log("SettingsButton hovered");
            MainMenu_UI_Button_Hover_Settings.Post(gameObject);
        };

        var quitButton = mainMenuRootQuery.Q<HoverButton>("QuitButton");
        quitButton.clicked += () =>
        {
            if (DebugMode) Debug.Log("QuitButton clicked");
            MainMenu_UI_Button_Click_QuitGame.Post(gameObject);
            Application.Quit();
        };
        quitButton.hovered += () =>
        {
            if (DebugMode) Debug.Log("QuitButton hovered");
            MainMenu_UI_Button_Hover_QuitGame.Post(gameObject);
        };
    }

    public void Open()
    {
        ui_MainMenuRoot.SetEnabled(true);
        mainMenuRootQuery.SetEnabled(true);
        
        ui_MainMenuRoot.style.display = DisplayStyle.Flex;
        mainMenuRootQuery.style.display = DisplayStyle.Flex;
    }

    public void Close()
    {
        ui_MainMenuRoot.SetEnabled(false);
        mainMenuRootQuery.SetEnabled(false);
        
        ui_MainMenuRoot.style.display = DisplayStyle.None;
        mainMenuRootQuery.style.display = DisplayStyle.None;
        
    }
}
