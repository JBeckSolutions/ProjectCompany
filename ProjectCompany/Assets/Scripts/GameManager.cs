using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    //Manages the state of the game

    public static GameManager Singelton;
    public DropOffAreaManager DropOffAreaManager;
    [SerializeField] private List<Item> itemsDroppedOff;
    public float Quota;
    public List<PlayerState> PlayerStates;

    private bool RoundRunning = false;

    private void Awake()
    {
        if (Singelton == null)
        {
            Singelton = this;
        }
        else
        {
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(this);
    }
    [ServerRpc]
    public void StartRoundServerRpc()   //Starts a round (Only the host can start it)
    {
        NetworkManager.Singleton.SceneManager.LoadScene("TestLevel", UnityEngine.SceneManagement.LoadSceneMode.Single);
        RoundRunning = true;
    }
    [ServerRpc(RequireOwnership = false)]
    public void EndRoundServerRpc() //Ends a round
    {
        RoundRunning = false;
        itemsDroppedOff = DropOffAreaManager.ItemList;
        DropOffAreaManager = null;
        NetworkManager.Singleton.SceneManager.LoadScene("NetcodeLobbyRunning", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
