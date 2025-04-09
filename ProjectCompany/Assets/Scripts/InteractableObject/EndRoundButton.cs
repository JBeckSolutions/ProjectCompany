using UnityEngine;

public class EndRoundButton : InteractableObject
{
    //Ends the round when the button is pressed
    public override void Use()
    {
        GameManager.Singelton.EndRoundServerRpc();
    }
    
}
