using UnityEngine;
using Unity.Netcode;
using UnityEditor.PackageManager;
using Unity.Netcode.Components;

public class PlayerSpawnManager : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    public Transform[] SpawnPoints;
    public static PlayerSpawnManager Singelton = null;
    private void Awake()
    {
        if (Singelton == null)
        {
            Singelton = this;
        }
        else
        {
            Destroy(this);
            return;
        }
    }
    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += ServerStart;
    }
    public void ServerStart()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        
        Debug.Log("Client connected: " + clientId);
        SpawnPlayerServerRpc(clientId);
        
    }

    [ServerRpc]
    private void SpawnPlayerServerRpc(ulong clientId)
    {
        // Get spawn index based on clientId (modulo to ensure we stay within spawn points array)
        int spawnIndex = (int)(clientId % (ulong)SpawnPoints.Length);
        Transform spawnPoint = SpawnPoints[spawnIndex];

        // Instantiate the player prefab at the chosen spawn point
        NetworkObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation).GetComponent<NetworkObject>();

        // Ensure the player prefab is correctly networked across clients
        player.SpawnAsPlayerObject(clientId);
        SpawnPlayerClientRpc(clientId, spawnPoint.position);
    }
    [ClientRpc]
    private void SpawnPlayerClientRpc(ulong clientid, Vector3 position)
    {
        if (clientid == NetworkManager.Singleton.LocalClientId)
        {
            NetworkObject player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();

            player.GetComponent<PlayerController>().enabled = false;
            player.GetComponent<CharacterController>().enabled = false;
            player.transform.position = position;
            player.GetComponent<PlayerController>().enabled = true;
            player.GetComponent<CharacterController>().enabled = true;
        }
    }
    public void TeleportLocalPlayer()
    {
        NetworkObject player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();

        if (player != null && player.IsOwner)
        {
            int spawnIndex = (int)(player.OwnerClientId % (ulong)SpawnPoints.Length);
            Vector3 spawnPos = SpawnPoints[spawnIndex].position;
            player.GetComponent<PlayerController>().enabled = false;
            player.GetComponent<CharacterController>().enabled = false;
            player.transform.position = spawnPos;
            player.GetComponent<PlayerController>().enabled = true;
            player.GetComponent<CharacterController>().enabled = true;

            Debug.Log("Teleported");
        }
    }
}
