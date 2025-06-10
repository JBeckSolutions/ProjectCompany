using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The code of the this script was written by: Beck Jonas
/// </summary>
public class Room : MonoBehaviour
{
    [Tooltip("Points where other rooms can be connected to this one.")]
    public List<Transform> ConnectionPoints = new List<Transform>();

    [Tooltip("Possible spawn positions for items in this room.")]
    public List<Transform> ItemSpawns = new List<Transform>();

    [Tooltip("Possible spawn positions for enemies in this room.")]
    public List<Transform> EnemySpawns = new List<Transform>();
}
