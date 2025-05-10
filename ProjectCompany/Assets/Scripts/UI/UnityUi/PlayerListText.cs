using System;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerListText : NetworkBehaviour
{
    [SerializeField] private TMP_Text textUi;
    [SerializeField] private TMP_Text clientTextUi;
    public NetworkList<FixedString128Bytes> playerNames = new NetworkList<FixedString128Bytes>();

    public override void OnNetworkSpawn()
    {
        if (playerNames == null)
        {
            playerNames = new NetworkList<FixedString128Bytes>();
        }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
        }
        
        playerNames.OnListChanged += OnListChanged;
    }

    public override void OnNetworkDespawn()
    {
        playerNames.OnListChanged -= OnListChanged;

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
        }

        base.OnDestroy();

    }

    private void OnClientConnectedCallback(ulong clientId)
    {
        if (playerNames == null) return;
        playerNames.Add($"Player {clientId}");
    }

    private void OnClientDisconnectCallback(ulong clientId)
    {
        if (playerNames == null) return;
        playerNames.Remove($"Player {clientId}");
    }

    private void OnListChanged(NetworkListEvent<FixedString128Bytes> change)
    {
        UpdateUi();
    }

    public void UpdateUi()
    {

        if (playerNames == null) return;
        

        if (IsServer)
        {
            textUi.text = "";
            foreach (var name in playerNames)
            {
                textUi.text += name.ToString() + "\n";
            }
        }
        else if (IsClient)
        {
            clientTextUi.text = "";
            foreach (var name in playerNames)
            {
                clientTextUi.text += name.ToString() + "\n";
            }
        }
    }
}