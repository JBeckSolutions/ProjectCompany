using Unity.Netcode;
using UnityEngine;
using System.Collections;
/// <summary>
/// The code of the this script was written by: Beck Jonas
/// </summary>
public class EndRoundButton : InteractableObject
{
    //Ends the round when the button is pressed
    [Header("Wwise Events")]
    [SerializeField] private AK.Wwise.Event EndRoundButtonPressedEvent;
    public override void Use()
    {
        EndRoundButtonPressedEvent.Post(gameObject);
        GameManager.Singelton.EndRoundServerRpc();
    }
}
