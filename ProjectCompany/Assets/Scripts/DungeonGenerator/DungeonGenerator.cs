using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

public class DungeonGenerator : NetworkBehaviour
{
    private struct Connection
    {
        public Transform ConnectionLocation;
        public int ConnectionPriority;

        public Connection (Transform transform, int value)
        {
            ConnectionLocation = transform;
            ConnectionPriority = value;
        }
    }




    [Header("Prefabs")]
    [SerializeField] private List<GameObject> roomPrefabs;
    [SerializeField] private GameObject startingRoomPrefab;
    [SerializeField] private List<GameObject> itemPrefabs;
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private GameObject generatedItemsParent;
    [SerializeField] private GameObject generatedEnemiesParent;

    [Header("Generation Options")]
    [SerializeField] private int maxRooms;
    [SerializeField] private int itemCount;
    [SerializeField] private int enemyCount;

    [Header("Seed")]
    [SerializeField] private bool useRandomSeed = true;
    [SerializeField] private int seed = 0;

    [Header("Generated objects")]
    [SerializeField] private List<Room> placedRooms = new List<Room>();
    [SerializeField] private List<GameObject> placedEnemies = new List<GameObject>();
    [SerializeField] private List<Connection> availableConnections = new List<Connection>();
    [SerializeField] private List<Transform> availableItemSpawns = new List<Transform>();
    [SerializeField] private List<Transform> availableEnemySpawns = new List<Transform>();


    public override void OnNetworkSpawn()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            GenerateDungeonServerRpc();
        }
    }

    [ServerRpc]
    private void GenerateDungeonServerRpc()
    {
        Debug.Log("Server Rpc called");
        if (useRandomSeed)
        {
            Random CreateSeed = new Random();
            seed = CreateSeed.Next(int.MinValue, int.MaxValue);
        }
        GenerareDungeonClientRpc(seed);
    }
    [ClientRpc]
    private void GenerareDungeonClientRpc(int seed)
    {
        Debug.Log("Client Rpc called");
        this.seed = seed;
        StartCoroutine(GenerateDungeon());
    }


    public IEnumerator GenerateDungeon()
    {
        Random Random = new Random(seed);
        Debug.Log("Dungeon Seed: " + seed);

        GameObject start = Instantiate(startingRoomPrefab, Vector3.zero, Quaternion.identity);

        Room startRoom = start.GetComponent<Room>();
        placedRooms.Add(startRoom);
        availableConnections.AddRange(startRoom.ConnectionPoints.Select(item => new Connection(item, -1)));

        for (int i = 0; i < maxRooms; i++)
        {
            if (availableConnections.Count == 0) break;

            int randomConnectionIndex = Random.Next(availableConnections.Count);
            Transform connectionPoint = availableConnections[randomConnectionIndex].ConnectionLocation;

            int randomRoomPrefabIndex = Random.Next(0, roomPrefabs.Count);
            GameObject roomPrefab = roomPrefabs[randomRoomPrefabIndex];
            GameObject newRoom = Instantiate(roomPrefab);

            Room roomComponent = newRoom.GetComponent<Room>();
            if (roomComponent.ConnectionPoints.Count == 0)
            {
                Destroy(newRoom);
                continue;
            }

            int randomEntrance = Random.Next(0, roomComponent.ConnectionPoints.Count);
            Transform newRoomEntrance = roomComponent.ConnectionPoints[randomEntrance];
            Quaternion targetRotation = Quaternion.LookRotation(-connectionPoint.forward, newRoomEntrance.up);

            Quaternion rotationDelta = targetRotation * Quaternion.Inverse(newRoomEntrance.rotation);
            newRoom.transform.rotation = rotationDelta * newRoom.transform.rotation;

            Vector3 offset = connectionPoint.position - newRoomEntrance.position;
            newRoom.transform.position += offset;


            //newRoom.transform.rotation = targetRotation;
            //Vector3 offset = newRoom.transform.position - newRoomEntrance.transform.position;
            //newRoom.transform.position = connectionPoint.position;
            //newRoom.transform.localPosition += offset;

            yield return new WaitForFixedUpdate();

            if (IsOverlapping(newRoom))
            {
                Destroy(newRoom);
                continue;
            }

            placedRooms.Add(roomComponent);
            availableConnections.AddRange((roomComponent.ConnectionPoints.Select(item => new Connection(item, randomRoomPrefabIndex))));

            if (availableConnections.FirstOrDefault(item => item.ConnectionLocation == connectionPoint).ConnectionPriority <= availableConnections.FirstOrDefault(item => item.ConnectionLocation == newRoomEntrance).ConnectionPriority)
            {
                connectionPoint.Find("Wall").gameObject.SetActive(false);
                connectionPoint.Find("Door").gameObject.SetActive(true);

                newRoomEntrance.Find("Wall").gameObject.SetActive(false);
                newRoomEntrance.Find("Door").gameObject.SetActive(false);
            }
            else
            {
                connectionPoint.Find("Wall").gameObject.SetActive(false);
                connectionPoint.Find("Door").gameObject.SetActive(false);

                newRoomEntrance.Find("Wall").gameObject.SetActive(false);
                newRoomEntrance.Find("Door").gameObject.SetActive(true);
            }

            availableConnections.RemoveAll(item => item.ConnectionLocation == connectionPoint);
            availableConnections.RemoveAll(item => item.ConnectionLocation == newRoomEntrance);

        }


        Debug.Log("Room Placements complete");
        Debug.Log("placed " + placedRooms.Count + " romms");

        int doorsAdded = 0;
        int possibleDoorsRemoved = 0;

        for (int i = 0; i < availableConnections.Count; i++)
        {
            Transform connection = availableConnections[i].ConnectionLocation.transform;

            for (int j = i + 1; j < availableConnections.Count; j++)
            {
                Transform connectionToCompare = availableConnections[j].ConnectionLocation.transform;
                
                if (connection.transform.position == connectionToCompare.transform.position)
                {
                    int randomDoor = Random.Next(0, 3);

                    //Debug.Log(randomDoor);

                    if (randomDoor <= 1)
                    {
                        availableConnections.RemoveAll(item => item.ConnectionLocation == connectionToCompare);
                        availableConnections.RemoveAll(item => item.ConnectionLocation == connection);

                        possibleDoorsRemoved += 1;

                        i = -1;
                        break;
                    }
                    else if (randomDoor > 1)
                    {
                        if (availableConnections.FirstOrDefault(item => item.ConnectionLocation == connection).ConnectionPriority <= availableConnections.FirstOrDefault(item => item.ConnectionLocation == connectionToCompare).ConnectionPriority)
                        {
                            connection.Find("Wall").gameObject.SetActive(false);
                            connection.Find("Door").gameObject.SetActive(true);

                            connectionToCompare.Find("Wall").gameObject.SetActive(false);
                            connectionToCompare.Find("Door").gameObject.SetActive(false);
                        }
                        else
                        {
                            connection.Find("Wall").gameObject.SetActive(false);
                            connection.Find("Door").gameObject.SetActive(false);

                            connectionToCompare.Find("Wall").gameObject.SetActive(false);
                            connectionToCompare.Find("Door").gameObject.SetActive(true);
                        }

                        availableConnections.RemoveAll(item => item.ConnectionLocation == connectionToCompare);
                        availableConnections.RemoveAll(item => item.ConnectionLocation == connection);

                        doorsAdded += 1;

                        i = -1;
                        break;
                    }
                }
            }
        }

        Debug.Log("Added " + doorsAdded + " doors");
        Debug.Log("Removed " + possibleDoorsRemoved + " possible doors");

        //Generate Empty for parenting rooms
        GameObject generatedLevelParent = new GameObject("GeneratedLevel");
        generatedLevelParent.transform.position = Vector3.zero;

        foreach (var room in placedRooms)
        {
            room.transform.SetParent(generatedLevelParent.transform);
            room.gameObject.isStatic = true;
            Transform[] childTransforms = room.GetComponentsInChildren<Transform>();
            foreach (Transform childTransform in childTransforms)
            {
                childTransform.gameObject.isStatic = true;
            }
            availableItemSpawns.AddRange(room.ItemSpawns);
            availableEnemySpawns.AddRange(room.EnemySpawns);
        }

        if (NetworkManager.Singleton.IsServer)
        {
            //Navmesh generation
            NavMeshSurface navMesh = generatedLevelParent.AddComponent<NavMeshSurface>();
            navMesh.collectObjects = CollectObjects.Children;
            navMesh.BuildNavMesh();

            //Item Spawning

            //Spawn parent for items
            GameObject generatedItemParent = Instantiate(this.generatedItemsParent, Vector3.zero, Quaternion.identity);
            generatedItemParent.GetComponent<NetworkObject>().Spawn(true);

            //Genrate and spawn the items
            int itemsSpawned = 0;
            for (int i = 0; i < itemCount; i++)
            {
                if (availableItemSpawns.Count <= 0) break;

                int randomItemSpawnpoint = Random.Next(0, availableItemSpawns.Count);
                int randomItem = Random.Next(0, itemPrefabs.Count);
                float randomRotation = (float)(0.0 + (360.0 - 0.0) * Random.NextDouble());
                Quaternion Rotation = Quaternion.Euler(0f, randomRotation, 0f);
                GameObject newItem = Instantiate(itemPrefabs[randomItem], availableItemSpawns[randomItemSpawnpoint].position, Rotation);
                newItem.GetComponent<NetworkObject>().Spawn(true);
                newItem.transform.SetParent(generatedItemParent.transform);
                availableItemSpawns.Remove(availableItemSpawns[randomItemSpawnpoint]);
                itemsSpawned += 1;
            }
            Debug.Log(itemsSpawned + " Items Spawned");

            //Enemy Spawning

            //Spawn parent for Enemies
            GameObject generatedEnemiesParent = Instantiate(this.generatedEnemiesParent, Vector3.zero, Quaternion.identity);
            generatedEnemiesParent.GetComponent<NetworkObject>().Spawn(true);

            //Genrate and spawn the Enemies
            int enemiesSpawned = 0;
            for (int i = 0; i < enemyCount; i++)
            {
                if (availableEnemySpawns.Count <= 0) break;

                int randomEnemySpawnpoint = Random.Next(0, availableEnemySpawns.Count);
                int randomEnemy = Random.Next(0, enemyPrefabs.Count);
                float randomRotation = (float)(0.0 + (360.0 - 0.0) * Random.NextDouble());
                Quaternion Rotation = Quaternion.Euler(0f, randomRotation, 0f);
                GameObject newEnemy = Instantiate(enemyPrefabs[randomEnemy], availableEnemySpawns[randomEnemySpawnpoint].position, Rotation);
                newEnemy.GetComponent<NetworkObject>().Spawn(true);
                newEnemy.transform.SetParent(generatedEnemiesParent.transform);
                availableEnemySpawns.Remove(availableEnemySpawns[randomEnemySpawnpoint]);
                enemiesSpawned += 1;
            }
            Debug.Log(enemiesSpawned + " Enemies spawned");
        }

        PlayerSpawnManager.Singelton.TeleportLocalPlayer();

    }

    private bool IsOverlapping(GameObject room)
    {
        Collider[] colliders = Physics.OverlapBox(
            room.transform.position,
            room.GetComponent<BoxCollider>().size / 2,
            room.transform.rotation);

        foreach (var Collider in colliders)
        {
            if (Collider.isTrigger)
            {
                if (Collider.transform != room.transform)
                {
                    //Debug.Log(Collider.transform.name);
                    return true;
                }
            }
        }
        return false;
    }


}
