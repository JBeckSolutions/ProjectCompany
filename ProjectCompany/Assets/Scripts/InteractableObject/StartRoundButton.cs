using Unity.Netcode;
using UnityEngine;

public class StartRoundButton : InteractableObject
{
    //Starts a round when the button is pressed
    public override void Use()
    {
        if (!IsServer) return;

        if (interactable.Value)
        {
            GameManager.Singelton.StartRoundServerRpc();
            interactable.Value = false;
        }
    }
    
    
}
