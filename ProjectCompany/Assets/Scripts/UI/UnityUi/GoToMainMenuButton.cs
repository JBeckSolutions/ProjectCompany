using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// The code of the this script was written by: Beck Jonas
/// </summary>
public class GoToMainMenuButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private SceneLoader sceneLoader;

    private void Start()
    {
        button.onClick.AddListener(ShutdownClient);
        button.onClick.AddListener(sceneLoader.LoadScene);
    }

    private void ShutdownClient()
    {
        NetworkManager.Singleton.Shutdown();
    }


}
