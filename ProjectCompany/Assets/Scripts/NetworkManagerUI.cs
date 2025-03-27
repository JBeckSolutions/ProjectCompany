using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button loadLvl;

    private void Awake()
    {
        hostButton.onClick.AddListener(() => NetworkManager.Singleton.StartHost());
        clientButton.onClick.AddListener(() => NetworkManager.Singleton.StartClient());
        loadLvl.onClick.AddListener(() => LoadTestLvl());
    }

    public void LoadTestLvl()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("TestLevel", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
