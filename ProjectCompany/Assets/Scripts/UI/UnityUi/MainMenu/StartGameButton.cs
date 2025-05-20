using Unity.Netcode;
using UnityEngine;

public class StartGameButton : NetworkBehaviour
{
    public void StartGame()
    {
        StartGameServerRpc();
    }

    [ServerRpc]
    public void StartGameServerRpc()
    {
        Debug.Log("Game started");
        NetworkManager.Singleton.SceneManager.LoadScene("Start with new UI", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
