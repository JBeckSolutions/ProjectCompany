using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameWonButton : MonoBehaviour
{
    [SerializeField] private Button button;
    private PlayerInput playerInput;
    private void Start()
    {
        button.onClick.AddListener(Ok);
    }
    public void Ok()
    {
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerInput>().enabled = true;
    }
}
