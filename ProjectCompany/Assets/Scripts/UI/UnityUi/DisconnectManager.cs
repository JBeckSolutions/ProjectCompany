using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class DisconnectManager : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.OnClientStopped += OnClientStopped;
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.OnClientStopped -= OnClientStopped;
    }

    private void OnClientStopped(bool obj)
    {
        Debug.Log("LoadScene");
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
