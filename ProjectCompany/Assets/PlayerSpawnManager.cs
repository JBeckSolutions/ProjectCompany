using UnityEngine;
using Unity.Netcode;

public class PlayerSpawnManager : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;
    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += ServerStart;
    }
    public void ServerStart()
    {
        Debug.Log("Test");
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
        int spawnIndex = (int)(clientId % (ulong)spawnPoints.Length);
        Transform spawnPoint = spawnPoints[spawnIndex];

        // Instantiate the player prefab at the chosen spawn point
        NetworkObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation).GetComponent<NetworkObject>();

        // Ensure the player prefab is correctly networked across clients
        player.SpawnAsPlayerObject(clientId);
    }
}
