using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class JoinButton : MonoBehaviour
{
    [SerializeField] private TMP_InputField ipText;
    [SerializeField] private GameObject JoinGameMenu;
    [SerializeField] private GameObject LobbyMenu;
    [SerializeField] private PlayerListText playerListManager;

    public void StartClient()
    {
        ConnectToServer(ipText.text);
    }

    private void Awake()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }

    }

    private void ConnectToServer(string ip , ushort port = 7777)
    {
        UnityTransport transport = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        transport.ConnectionData.Address = ip;
        transport.ConnectionData.Port = port;


        Debug.Log("Connecting to ip: " + ip + " and port: " + port.ToString());
        NetworkManager.Singleton.StartClient();
    }
    private void OnClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("Client successfully connected to server");
        }

        LobbyMenu.SetActive(true);
        playerListManager.UpdateUi();
        JoinGameMenu.SetActive(false);
    }
    private void OnClientDisconnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("Failed to connect or disconnected from server.");
            NetworkManager.Singleton.Shutdown();
        }
    }
}
