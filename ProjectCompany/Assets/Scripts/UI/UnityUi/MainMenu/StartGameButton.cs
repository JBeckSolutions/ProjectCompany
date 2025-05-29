using Unity.Netcode;
using UnityEngine;
/// <summary>
/// The code of the this script was written by: Beck Jonas
/// </summary>
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
