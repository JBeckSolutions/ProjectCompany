using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class PlayerDead : NetworkBehaviour
{
    private int watchingIndex = 0;
    private List<PlayerState> alivePlayers;

    public override void OnNetworkSpawn()
    {
        alivePlayers = new List<PlayerState>();

        foreach (var player in GameManager.Singelton.PlayerStates)
        {
            if (player.PlayerAlive.Value == true)
            {
                alivePlayers.Add(player);
            }
        }
    }
    void Update()
    {
        if (alivePlayers[watchingIndex] == null)
        {
            ChangePlayerToWatch(0);
        }
        else
        {
            alivePlayers[watchingIndex].model.SetActive(false);
            this.transform.position = alivePlayers[watchingIndex].playerCamera.transform.position;
            this.transform.rotation = alivePlayers[watchingIndex].playerCamera.transform.rotation;
        }
    }

    public void ChangePlayerToWatch(int direction)
    {
        alivePlayers = new List<PlayerState>();

        foreach (var player in GameManager.Singelton.PlayerStates)
        {
            if (player.PlayerAlive.Value == true)
            {
                alivePlayers.Add(player);
            }
        }

        watchingIndex += direction;

        if (watchingIndex < 0)
        {
            watchingIndex = alivePlayers.Count - 1;
        }
        if (watchingIndex >= alivePlayers.Count)
        {
            watchingIndex = 0;
        }
    }
}
