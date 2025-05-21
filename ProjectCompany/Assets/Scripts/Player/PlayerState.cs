using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerState : NetworkBehaviour
{
    [Header("Player Health Settings")]
    public NetworkVariable<int> PlayerHealth = new NetworkVariable<int>(100);
    public NetworkVariable<bool> PlayerAlive = new NetworkVariable<bool>(true);

    [Header("UI Components")]
    [SerializeField] private GameObject PlayerUi;
    [SerializeField] private TMP_Text healthText;

    [Header("Player Components")]
    public Camera playerCamera;
    public GameObject model;
    public override void OnNetworkSpawn()
    {
        GameManager.Singelton.PlayerStates.Add(this);
        if (!IsOwner)
        {
            playerCamera.enabled = false;
            PlayerUi.SetActive(false);
        }
        if (IsOwner)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            GetComponentsInChildren<Renderer>().ToList().ForEach(r => r.enabled = false);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(int Amount)
    {
        PlayerHealth.Value -= Amount;
        TakeDamageClientRpc();
        if (PlayerHealth.Value <= 0 && PlayerAlive.Value)
        {
            Debug.Log("Player " + OwnerClientId + " died");
            GameManager.Singelton.playerDeaths.Value += 1;
            PlayerAlive.Value = false;
            GameManager.Singelton.OnPlayerDeathServerRpc(this.OwnerClientId);
        }
    }
    [ClientRpc]
    public void TakeDamageClientRpc()
    {
        if (IsOwner)
        {
            healthText.text = "HP: " + PlayerHealth.Value;
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
