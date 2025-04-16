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
    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(int Amount)
    {
        PlayerHealth.Value -= Amount;
        if (PlayerHealth.Value <= 0)
        {
            Debug.Log("Player " + OwnerClientId + " died");
        }
    }

    [ClientRpc]
    public void DisableClientControlsAndGravityClientRpc()
    {
        this.transform.GetComponent<PlayerController>().enabled = false;
        this.transform.GetComponent<CharacterController>().enabled = false;
    }

    [ClientRpc]
    public void EnableClientControlsAndGravityClientRpc()
    {
        this.transform.GetComponent<PlayerController>().enabled = true;
        this.transform.GetComponent<CharacterController>().enabled = true;
    }
    [ClientRpc]
    public void SetPlayerPositionClientRpc(Vector3 position)
    {
        this.transform.position = position;
    }
}
