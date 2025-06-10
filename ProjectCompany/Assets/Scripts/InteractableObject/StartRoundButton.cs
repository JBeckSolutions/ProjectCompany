using Unity.Netcode;
using UnityEngine;
/// <summary>
/// The code of the this script was written by: Beck Jonas
/// </summary>
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
