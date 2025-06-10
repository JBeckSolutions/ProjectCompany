using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// The code of the this script was written by: Beck Jonas
/// </summary>
public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    private void Awake()
    {
        hostButton.onClick.AddListener(() => NetworkManager.Singleton.StartHost());
        clientButton.onClick.AddListener(() => NetworkManager.Singleton.StartClient());
    }
}
