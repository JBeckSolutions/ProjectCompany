using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class PlayerDead : NetworkBehaviour
{
    private int watchingIndex = 0;
    private List<PlayerState> alivePlayers;
    [SerializeField] private TMP_Text playerWatchText;
    [SerializeField] private GameObject deadPlayerUi;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
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

        if (!IsOwner)
        {
            deadPlayerUi.SetActive(false);
        }
    }
    void Update()
    {
        if (!IsOwner) return;
        if (watchingIndex >= alivePlayers.Count && alivePlayers[watchingIndex] == null)
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

        playerWatchText.text = ("Watching: " + alivePlayers[watchingIndex].name);
        
    }
}
