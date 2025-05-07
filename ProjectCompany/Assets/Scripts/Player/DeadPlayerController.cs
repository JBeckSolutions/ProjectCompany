using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeadPlayerController : NetworkBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] PlayerDead playerDeadLogic;
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            playerInput.enabled = false;
        }
    }

    public void OnNextPlayer(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            playerDeadLogic.ChangePlayerToWatch(1);
        }
    }

    public void OnPreviousPlayer(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            playerDeadLogic.ChangePlayerToWatch(-1);
        }
    }

}
