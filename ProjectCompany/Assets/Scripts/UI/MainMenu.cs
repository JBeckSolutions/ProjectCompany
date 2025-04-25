using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    private VisualElement menuRoot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        menuRoot = root.Q<VisualElement>("MainMenu");
        
        
        
        
        var newGameButton = root.Q<Button>("NewGameButton");
        newGameButton.clicked += () => {
            NetworkManager.Singleton.StartHost();
            menuRoot.style.display = DisplayStyle.None;
            FindFirstObjectByType<Interface>().EnableInventory();
            
            
        };
        
        
        var joinButton = root.Q<Button>("JoinButton");
        joinButton.clicked += () => {
            NetworkManager.Singleton.StartClient();
            menuRoot.style.display = DisplayStyle.None;
            FindFirstObjectByType<Interface>().EnableInventory();
        };
        
        var settingsButton = root.Q<Button>("SettingsButton");
        settingsButton.clicked += () => {

        };
        
        var quitButton = root.Q<Button>("QuitButton");
        quitButton.clicked += () => {
            Application.Quit();
        };

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
