using UnityEngine;
/// <summary>
/// The code of the this script was written by: Beck Jonas
/// </summary>
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
