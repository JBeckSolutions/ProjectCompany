using Unity.Netcode;
using UnityEngine;
using System.Collections;
public class EndRoundButton : InteractableObject
{
    //Ends the round when the button is pressed
    public override void Use()
    {
        
            GameManager.Singelton.EndRoundServerRpc();
            interactable.Value = false;
        
    }

}
