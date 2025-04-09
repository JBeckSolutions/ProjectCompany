using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<Transform> ConnectionPoints = new List<Transform>();
    public List<Transform> ItemSpawns = new List<Transform>();
    public List<Transform> EnemySpawns = new List<Transform>();
}
