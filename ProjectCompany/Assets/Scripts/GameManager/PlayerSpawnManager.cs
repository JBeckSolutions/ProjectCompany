using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using Unity.Netcode.Components;
using System.Collections;

public class PlayerSpawnManager : NetworkBehaviour
{
    [SerializeField] private List<GameObject> playerPrefabs;
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
        ulong test = this.OwnerClientId;
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

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        Debug.Log("Spawning players...");
        StartCoroutine(SpawnAllConnectedPlayers());
    }

    private IEnumerator SpawnAllConnectedPlayers()
    {
        yield return new WaitForSeconds(1);

        foreach (var player in NetworkManager.Singleton.ConnectedClientsIds)
        {
            SpawnPlayerServerRpc(player);
        }
    }

    [ServerRpc]
    public void SpawnPlayerServerRpc(ulong clientId, int prefab = 0)
    {
        // Get spawn index based on clientId (modulo to ensure we stay within spawn points array)
        int spawnIndex = (int)(clientId % (ulong)SpawnPoints.Length);
        Transform spawnPoint = SpawnPoints[spawnIndex];

        // Instantiate the player prefab at the chosen spawn point
        NetworkObject player = Instantiate(playerPrefabs[prefab], spawnPoint.position, spawnPoint.rotation).GetComponent<NetworkObject>();
        player.name = "Player " + clientId;
        // Ensure the player prefab is correctly networked across clients
        player.SpawnAsPlayerObject(clientId);
        if (prefab == 0)
        {
            SpawnPlayerClientRpc(clientId, spawnPoint.position);
        }
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
