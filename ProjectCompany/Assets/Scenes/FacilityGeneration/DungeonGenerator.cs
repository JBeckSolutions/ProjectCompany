using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> roomPrefabs;
    [SerializeField] private GameObject startingRoomPrefab;

    [SerializeField] private int maxRooms;

    [SerializeField] private bool useRandomSeed = true;
    [SerializeField] private int seed = 0;

    [SerializeField] private List<Room> placedRooms = new List<Room>();
    [SerializeField] private List<Transform> availableConnections = new List<Transform>();

    private void Start()
    {
        if (useRandomSeed)
        {
            seed = Random.Range(int.MinValue, int.MaxValue);
        }

        Random.InitState(seed);
        Debug.Log("Dungeon Seed: " + seed);

        StartCoroutine(GenerateDungeon());
    }

    public IEnumerator GenerateDungeon()
    {
        GameObject start = Instantiate(startingRoomPrefab, Vector3.zero, Quaternion.identity);
        Room startRoom = start.GetComponent<Room>();
        placedRooms.Add(startRoom);
        availableConnections.AddRange(startRoom.connectionPoints);

        for (int i = 0; i < maxRooms; i++)
        {
            if (availableConnections.Count == 0) break;

            Transform connectionPoint = availableConnections[Random.Range(0, availableConnections.Count)];
            GameObject roomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Count)];
            GameObject newRoom = Instantiate(roomPrefab);

            Room roomComponent = newRoom.GetComponent<Room>();
            if (roomComponent.connectionPoints.Count == 0)
            {
                Destroy(newRoom);
                continue;
            }

            Transform newRoomEntrance = roomComponent.connectionPoints[0];
            Quaternion targetRotation = Quaternion.LookRotation(connectionPoint.forward, Vector3.up);
            newRoom.transform.rotation = targetRotation;
            Vector3 offset = newRoom.transform.position - newRoomEntrance.transform.position;
            newRoom.transform.position = connectionPoint.position;
            newRoom.transform.localPosition += offset;

            if (IsOverlapping(newRoom))
            {
                Destroy(newRoom);
                continue;
            }

            placedRooms.Add(roomComponent);
            availableConnections.AddRange(roomComponent.connectionPoints);
            availableConnections.Remove(connectionPoint);
            connectionPoint.gameObject.SetActive(false);
            availableConnections.Remove(newRoomEntrance);

            yield return new WaitForFixedUpdate();
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
                    Debug.Log(Collider.transform.name);
                    return true;
                }
            }
        }
        return false;
    }
}
