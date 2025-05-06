using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerState : NetworkBehaviour
{
    //public NetworkVariable<bool> PlayerReadyState = new NetworkVariable<bool>(false);
    public NetworkVariable<int> PlayerHealth = new NetworkVariable<int>(100);
    public NetworkVariable<bool> PlayerAlive = new NetworkVariable<bool>(true);
    public GameObject playerCamera;
    public GameObject model;
    public override void OnNetworkSpawn()
    {
        GameManager.Singelton.PlayerStates.Add(this);
        if (!IsOwner)
        {
            playerCamera.SetActive(false);
        }
        if (IsOwner)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            model.SetActive(false);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(int Amount)
    {
        PlayerHealth.Value -= Amount;
        if (PlayerHealth.Value <= 0 && PlayerAlive.Value)
        {
            Debug.Log("Player " + OwnerClientId + " died");
            GameManager.Singelton.playerDeaths.Value += 1;
            PlayerAlive.Value = false;
            GameManager.Singelton.OnPlayerDeathServerRpc(this.OwnerClientId);
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
