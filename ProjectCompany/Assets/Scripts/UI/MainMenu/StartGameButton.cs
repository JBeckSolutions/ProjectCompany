using Unity.Netcode;
using UnityEngine;

public class StartGameButton : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("Game started");
        NetworkManager.Singleton.SceneManager.LoadScene("Start with new UI", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
