using Unity.Netcode;
using UnityEngine;

public class PlayerState : NetworkBehaviour
{
    public NetworkVariable<bool> PlayerReadyState = new NetworkVariable<bool>(false);

    public override void OnNetworkSpawn()
    {
        GameManager.Singelton.PlayerStates.Add(this);
    }
}
