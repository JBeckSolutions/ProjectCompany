using Unity.Netcode;
using UnityEngine;

public class PlayerState : NetworkBehaviour
{
    //public NetworkVariable<bool> PlayerReadyState = new NetworkVariable<bool>(false);
    public NetworkVariable<int> PlayerHealth = new NetworkVariable<int>(100);

    public override void OnNetworkSpawn()
    {
        GameManager.Singelton.PlayerStates.Add(this);
    }

    public void TakeDamage(int Amount)
    {
        PlayerHealth.Value -= Amount;
        if (PlayerHealth.Value <= 0)
        {
            Debug.Log("Player " + OwnerClientId + " died");
        }
    }
}
