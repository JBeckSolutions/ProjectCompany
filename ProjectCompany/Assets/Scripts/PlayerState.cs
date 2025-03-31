using Unity.Netcode;
using UnityEngine;

public class PlayerState : NetworkBehaviour
{
    public NetworkVariable<bool> PlayerReadyState = new NetworkVariable<bool>(false);
    private void Start()
    {
        GameManager.Singelton.PlayerStates.Add(this);
    }
}
