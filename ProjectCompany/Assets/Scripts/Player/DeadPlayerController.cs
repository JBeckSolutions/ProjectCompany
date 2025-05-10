using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeadPlayerController : NetworkBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] PlayerDead playerDeadLogic;
    [SerializeField] private GameUi gameUi;
    public bool controllsEnabled = true;
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            playerInput.enabled = false;
        }

        gameUi = GameObject.Find("CanvasGameUi").GetComponent<GameUi>();
    }

    public void OnNextPlayer(InputAction.CallbackContext context)
    {
        if (!controllsEnabled) return;
        if (context.started)
        {
            playerDeadLogic.ChangePlayerToWatch(1);
        }
    }

    public void OnPreviousPlayer(InputAction.CallbackContext context)
    {
        if (!controllsEnabled) return;
        if (context.started)
        {
            playerDeadLogic.ChangePlayerToWatch(-1);
        }
    }

    public void OnOpenMenu(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                gameUi.OpenMenu("PauseMenu");
                controllsEnabled = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                gameUi.CloseAllMenus();
                controllsEnabled = true;
            }
        }
    }

}
