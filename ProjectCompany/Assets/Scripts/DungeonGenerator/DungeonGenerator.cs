using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;
using Random = System.Random;   // Uses the c# Random class since the one from unity is not deterministic across multiple frames

public class DungeonGenerator : NetworkBehaviour
{
    private struct Connection   // Struct to store the connection location and the priority of their door pieces
    {
        public Transform ConnectionLocation;
        public int ConnectionPriority;

        public Connection (Transform transform, int value)
        {
            ConnectionLocation = transform; // Saves the location
            ConnectionPriority = value;     // Saves the priority
        }
    }
    [Header("Prefabs")]
    [SerializeField] private GameObject startingRoomPrefab; // Starting room of the dungeon
    [SerializeField] private List<GameObject> roomPrefabs;  // List of rooms that can be placed by the generation process
    [SerializeField] private List<GameObject> itemPrefabs;  // List of items the generation process can spawn
    [SerializeField] private List<GameObject> enemyPrefabs; // List of enemies the generation process can spawn
    [SerializeField] private GameObject generatedItemsParent;   // Parent of all items that will be generated
    [SerializeField] private GameObject generatedEnemiesParent; // Parent of all enemies that will be generated

    [Header("Generation Options")]
    [SerializeField] private int maxRooms;      // How many rooms can spawn
    [SerializeField] private int maxItemCount;  // How many items can spawn
    [SerializeField] private int maxEnemyCount; // How many enemies can spawn
    [UnityEngine.Range(0f, 100f)]
    [SerializeField] private float doorSpawnChanceForLoops = 50f; // Chance out of 100 to spawn a door

    [Header("Seed")]
    [SerializeField] private bool useRandomSeed = true; // Toggles if a random seed should be used
    [SerializeField] private int seed = 0;              // Seed value for generation

    [Header("Generated objects")]
    [SerializeField] private List<Room> placedRooms = new List<Room>();                         // List of all rooms that were placed down
    [SerializeField] private List<GameObject> placedEnemies = new List<GameObject>();           // List of all enemies that were spawned
    [SerializeField] private List<Connection> availableConnections = new List<Connection>();    // List of all room connections that are placed down and avaliable (not used)
    [SerializeField] private List<Transform> availableItemSpawns = new List<Transform>();       // List of all possible item spawns that are placed down and avaliable (not used)
    [SerializeField] private List<Transform> availableEnemySpawns = new List<Transform>();      // List of all possible enemy spawns that are placed down and avaliable (not used)


    public override void OnNetworkSpawn()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            GenerateDungeonServerRpc();
        }
    }

    [ServerRpc]
    private void GenerateDungeonServerRpc() //Generates a seed on the server
    {
        Debug.Log("Server Rpc called");
        if (useRandomSeed)
        {
            Random CreateSeed = new Random();
            seed = CreateSeed.Next(int.MinValue, int.MaxValue);
        }
        GenerareDungeonClientRpc(seed); //Starts a client rpc to generate a dungeon with the seed created from this function
    }
    [ClientRpc]
    private void GenerareDungeonClientRpc(int seed)
    {
        Debug.Log("Client Rpc called");
        this.seed = seed;
        StartCoroutine(GenerateDungeon());  //Starts a Corutine that handles generating the dungeon
    }


    public IEnumerator GenerateDungeon()
    {
        Random Random = new Random(seed);
        Debug.Log("Dungeon Seed: " + seed);

        //places starting room at origin
        GameObject start = Instantiate(startingRoomPrefab, Vector3.zero, Quaternion.identity);  
        Room startRoom = start.GetComponent<Room>();
        placedRooms.Add(startRoom); //Adds the starting room to the list
        availableConnections.AddRange(startRoom.ConnectionPoints.Select(item => new Connection(item, -1)));

        for (int i = 0; i < maxRooms; i++)
        {
            if (availableConnections.Count == 0) break;

            // Select a random connection point to attach a room
            int randomConnectionIndex = Random.Next(availableConnections.Count);                        
            Transform connectionPoint = availableConnections[randomConnectionIndex].ConnectionLocation; 

            // Choose and instantiate a random room
            int randomRoomPrefabIndex = Random.Next(0, roomPrefabs.Count);                      
            GameObject roomPrefab = roomPrefabs[randomRoomPrefabIndex];                                 
            GameObject newRoom = Instantiate(roomPrefab);           
            Room roomComponent = newRoom.GetComponent<Room>();

            if (roomComponent.ConnectionPoints.Count == 0)                                              
            {
                Destroy(newRoom);
                continue;
            }

            // Align entrance with the selected connection
            int randomEntrance = Random.Next(0, roomComponent.ConnectionPoints.Count);                  //Picks which connection point of the room should be used as an entrance
            Transform newRoomEntrance = roomComponent.ConnectionPoints[randomEntrance];
            Quaternion targetRotation = Quaternion.LookRotation(-connectionPoint.forward, newRoomEntrance.up);  //saves the -z rotation of the chosen connection for this room

            Quaternion rotationDelta = targetRotation * Quaternion.Inverse(newRoomEntrance.rotation);           //Multiplies the inverse of the entrance position with the -z rotation of the connection so that the z axis of the entrance and the connection look against each other
            newRoom.transform.rotation = rotationDelta * newRoom.transform.rotation;                            //Rotates the room based on the calculated rotation

            Vector3 offset = connectionPoint.position - newRoomEntrance.position;                               //Calculates how the entrance of the room has to move for both connections to overlap
            newRoom.transform.position += offset;                                                               //Moves the room based on the calculated result

            yield return new WaitForFixedUpdate();                                                              //Ensure collider is updated

            if (IsOverlapping(newRoom))                                                                        
            {
                Destroy(newRoom);
                continue;
            }
                
            placedRooms.Add(roomComponent);                                                                                                         
            availableConnections.AddRange((roomComponent.ConnectionPoints.Select(item => new Connection(item, randomRoomPrefabIndex))));    

            // Determine which room should place its door at the connection point
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

            // Remove used connections
            availableConnections.RemoveAll(item => item.ConnectionLocation == connectionPoint);
            availableConnections.RemoveAll(item => item.ConnectionLocation == newRoomEntrance);

        }


        Debug.Log("Room Placements complete");
        Debug.Log("placed " + placedRooms.Count + " romms");


        // Checks if there are overlapping connections after the generation finishes and decides if they should be a door or stay as a wall
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
                    double roll = Random.NextDouble() * 100.0;

                    if (roll > doorSpawnChanceForLoops)
                    {
                        availableConnections.RemoveAll(item => item.ConnectionLocation == connectionToCompare);
                        availableConnections.RemoveAll(item => item.ConnectionLocation == connection);

                        possibleDoorsRemoved += 1;

                        i = -1;
                        break;
                    }
                    else
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

        // Parent generated rooms to a single object
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
            // Generate navmesh
            NavMeshSurface navMesh = generatedLevelParent.AddComponent<NavMeshSurface>();
            navMesh.collectObjects = CollectObjects.Children;
            navMesh.BuildNavMesh();

            // Instantiate parent for items
            GameObject generatedItemParent = Instantiate(this.generatedItemsParent, Vector3.zero, Quaternion.identity);
            generatedItemParent.GetComponent<NetworkObject>().Spawn(true);

            // Spawn items
            int itemsSpawned = 0;
            for (int i = 0; i < maxItemCount; i++)
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

            // Instantiate parent for enemies
            GameObject generatedEnemiesParent = Instantiate(this.generatedEnemiesParent, Vector3.zero, Quaternion.identity);
            generatedEnemiesParent.GetComponent<NetworkObject>().Spawn(true);

            // Spawn enemies
            int enemiesSpawned = 0;
            for (int i = 0; i < maxEnemyCount; i++)
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

    private bool IsOverlapping(GameObject room) //Checks if rooms overlap each other
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
