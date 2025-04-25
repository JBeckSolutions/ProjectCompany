using Unity.Netcode;
using Unity.VisualScripting;
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
    public AK.Wwise.Event MainMenu_UI_Button_Hover_NewGame; //to be added after HoverButton is implemented
    public AK.Wwise.Event MainMenu_UI_Button_Hover_JoinGame; //to be added after HoverButton is implemented
    public AK.Wwise.Event MainMenu_UI_Button_Hover_Settings; //to be added after HoverButton is implemented
    public AK.Wwise.Event MainMenu_UI_Button_Hover_QuitGame; //to be added after HoverButton is implemented
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        menuRoot = root.Q<VisualElement>("MainMenu");
        
        var newGameButton = root.Q<Button>("NewGameButton");
        newGameButton.clicked += () => {
            MainMenu_UI_Button_Click_NewGame.Post(gameObject);
            NetworkManager.Singleton.StartHost();
            menuRoot.style.display = DisplayStyle.None;
            FindFirstObjectByType<Interface>().EnableInventory();
        };
        
        
        var joinButton = root.Q<Button>("JoinButton");
        joinButton.clicked += () => {
            MainMenu_UI_Button_Click_JoinGame.Post(gameObject);
            NetworkManager.Singleton.StartClient();
            menuRoot.style.display = DisplayStyle.None;
            FindFirstObjectByType<Interface>().EnableInventory();
        };
        
        var settingsButton = root.Q<Button>("SettingsButton");
        settingsButton.clicked += () => {
            MainMenu_UI_Button_Click_Settings.Post(gameObject);
        };
        
        var quitButton = root.Q<Button>("QuitButton");
        quitButton.clicked += () => {
            MainMenu_UI_Button_Click_QuitGame.Post(gameObject);
            Application.Quit();
        };

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
