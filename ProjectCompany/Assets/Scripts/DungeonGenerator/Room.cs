using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Tooltip("Points where other rooms can be connected to this one.")]
    public List<Transform> ConnectionPoints = new List<Transform>();

    [Tooltip("Possible spawn positions for items in this room.")]
    public List<Transform> ItemSpawns = new List<Transform>();

    [Tooltip("Possible spawn positions for enemies in this room.")]
    public List<Transform> EnemySpawns = new List<Transform>();
}
