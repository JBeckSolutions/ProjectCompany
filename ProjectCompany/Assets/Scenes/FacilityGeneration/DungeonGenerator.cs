using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

public class DungeonGenerator : NetworkBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private List<GameObject> roomPrefabs;
    [SerializeField] private GameObject startingRoomPrefab;
    [SerializeField] private List<GameObject> itemPrefabs;
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private GameObject generatedItemsParent;

    [Header("Generation Options")]
    [SerializeField] private int maxRooms;
    [SerializeField] private int itemCount;

    [Header("Seed")]
    [SerializeField] private bool useRandomSeed = true;
    [SerializeField] private int seed = 0;

    [Header("Generated objects")]
    [SerializeField] private List<Room> placedRooms = new List<Room>();
    [SerializeField] private List<Transform> availableConnections = new List<Transform>();
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
        availableConnections.AddRange(startRoom.ConnectionPoints);

        for (int i = 0; i < maxRooms; i++)
        {
            if (availableConnections.Count == 0) break;

            Transform connectionPoint = availableConnections[Random.Next(availableConnections.Count)];
            GameObject roomPrefab = roomPrefabs[Random.Next(0, roomPrefabs.Count)];
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
            availableConnections.AddRange(roomComponent.ConnectionPoints);
            availableConnections.Remove(connectionPoint);
            availableConnections.Remove(newRoomEntrance);

            connectionPoint.gameObject.SetActive(false);
            newRoomEntrance.gameObject.SetActive(false);
        }


        Debug.Log("Room Placements complete");
        Debug.Log("placed " + placedRooms.Count + " romms");

        int doorsAdded = 0;
        int possibleDoorsRemoved = 0;

        for (int i = 0; i < availableConnections.Count; i++)
        {
            Transform connection = availableConnections[i];

            for (int j = i + 1; j < availableConnections.Count; j++)
            {
                Transform connectionToCompare = availableConnections[j];
                
                if (connection.transform.position == connectionToCompare.transform.position)
                {
                    int randomDoor = Random.Next(0, 3);

                    //Debug.Log(randomDoor);

                    if (randomDoor <= 1)
                    {
                        availableConnections.Remove(connectionToCompare);
                        availableConnections.Remove(connection);

                        possibleDoorsRemoved += 1;

                        i = -1;
                        break;
                    }
                    else if (randomDoor > 1)
                    {
                        availableConnections.Remove(connectionToCompare);
                        availableConnections.Remove(connection);
                        connection.gameObject.SetActive(false);
                        connectionToCompare.gameObject.SetActive(false);

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
            GameObject generatedItemParent = Instantiate(generatedItemsParent, Vector3.zero, Quaternion.identity);
            generatedItemParent.GetComponent<NetworkObject>().Spawn(true);

            //Genrate and spawn the items
            int itemsSpawned = 0;
            for (int i = 0; i < itemCount; i++)
            {
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
        }

        

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
