using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    private VisualElement menuRoot;

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
        var root = GetComponent<UIDocument>().rootVisualElement;

        menuRoot = root.Q<VisualElement>("MainMenu");
        var newGameHoverButton = root.Q<HoverButton>("NewGameButton");
        newGameHoverButton.clicked += () =>
        {
            MainMenu_UI_Button_Click_NewGame.Post(gameObject);
            NetworkManager.Singleton.StartHost();
            menuRoot.style.display = DisplayStyle.None;
            FindFirstObjectByType<Interface>().EnableInventory();
        };
        newGameHoverButton.hovered += () => { MainMenu_UI_Button_Hover_NewGame.Post(gameObject); };

        var joinButton = root.Q<HoverButton>("JoinButton");
        joinButton.clicked += () =>
        {
            MainMenu_UI_Button_Click_JoinGame.Post(gameObject);
            NetworkManager.Singleton.StartClient();
            menuRoot.style.display = DisplayStyle.None;
            FindFirstObjectByType<Interface>().EnableInventory();
        };
        joinButton.hovered += () => { MainMenu_UI_Button_Hover_JoinGame.Post(gameObject); };

        var settingsButton = root.Q<HoverButton>("SettingsButton");
        settingsButton.clicked += () => { MainMenu_UI_Button_Click_Settings.Post(gameObject); };
        settingsButton.hovered += () => { MainMenu_UI_Button_Hover_Settings.Post(gameObject); };

        var quitButton = root.Q<HoverButton>("QuitButton");
        quitButton.clicked += () =>
        {
            MainMenu_UI_Button_Click_QuitGame.Post(gameObject);
            Application.Quit();
        };
        quitButton.hovered += () => { MainMenu_UI_Button_Hover_QuitGame.Post(gameObject); };
    }

    // Update is called once per frame
    void Update()
    {
    }
}