using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverButton;
    [SerializeField] private Button HostButton;
    [SerializeField] private Button ClientButton;

    private void Awake()
    {
        serverButton.onClick.AddListener(() => NetworkManager.Singleton.StartServer());
        HostButton.onClick.AddListener(() => NetworkManager.Singleton.StartHost());
        ClientButton.onClick.AddListener(() => NetworkManager.Singleton.StartClient());
    }
}
