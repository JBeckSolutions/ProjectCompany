using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class PlayerDead : NetworkBehaviour
{
    private int watchingIndex = 0;
    private List<PlayerState> alivePlayers = new List<PlayerState>();
    [SerializeField] private TMP_Text playerWatchText;
    [SerializeField] private GameObject deadPlayerUi;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            ChangePlayerToWatch(0);
        }

        if (!IsOwner)
        {
            deadPlayerUi.SetActive(false);
        }
    }
    void Update()
    {
        if (!IsOwner) return;
        if (alivePlayers.Count == 0 || watchingIndex >= alivePlayers.Count)
        {
            ChangePlayerToWatch(0);
        }
        else
        {
            this.transform.position = alivePlayers[watchingIndex].playerCamera.transform.position;
            this.transform.rotation = alivePlayers[watchingIndex].playerCamera.transform.rotation;
        }
    }

    public void ChangePlayerToWatch(int direction)
    {
        if (alivePlayers.Count > 0 && watchingIndex >= 0 && watchingIndex < alivePlayers.Count)
        {
            if (alivePlayers[watchingIndex] != null)
            {
                alivePlayers[watchingIndex].model.SetActive(true);
            }
        }

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
        alivePlayers[watchingIndex].model.SetActive(false);
    }
}
