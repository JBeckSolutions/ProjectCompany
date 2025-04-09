using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class SetSpawns : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] bool shouldTriggerSpawn = false;

    private void Start()
    {

        PlayerSpawnManager.Singelton.SpawnPoints = this.spawnPoints;

        if (shouldTriggerSpawn)
        {
            PlayerSpawnManager.Singelton.TeleportLocalPlayer();
        }
    }


}
