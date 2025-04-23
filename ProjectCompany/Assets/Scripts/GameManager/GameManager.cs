using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    //Manages the state of the game

    public static GameManager Singelton;
    public DropOffAreaManager DropOffAreaManager;
    public DungeonGenerator MapGenerator;
    public NetworkVariable<int> ItemValueLastRound = new NetworkVariable<int>(0);
    public List<PlayerState> PlayerStates;

    [SerializeField] float maxRoundTimer = 900f;
    [SerializeField] float currentTimeInLvl;

    [Header("EnemySpawning")]
    [SerializeField] private AnimationCurve spawnRateOverTime;
    [SerializeField] private float minSpawnInterval = 10f;
    [SerializeField] private float maxSpawnInverval = 45f;

    [Header("ItemSpawning")]
    public NetworkVariable<int> Quota = new NetworkVariable<int>(0);
    public NetworkVariable<int> playerDeaths = new NetworkVariable<int>(0);
    [SerializeField] private int currentDay = 1;

    [Header("Dungeon Size")]
    public NetworkVariable<int> DungeonSize = new NetworkVariable<int>(0);
    [SerializeField] private int minRooms = 50;
    [SerializeField] private int maxRooms = 750;
    [SerializeField] private int QuotaForMaxSize = 10000;
    
    

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

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            SetQuotaAndDungeonSize();
        }
    }

    private void Update()
    {
        if (RoundRunning)
        {
            currentTimeInLvl += Time.deltaTime;
        }
    }
    [ServerRpc]
    public void StartRoundServerRpc()   //Starts a round (Only the host can start it)
    {
        Debug.Log("Quota to win: " + Quota);
        NetworkManager.Singleton.SceneManager.LoadScene("DungeonGenerator", LoadSceneMode.Additive);
    }
    [ServerRpc(RequireOwnership = false)]
    public void EndRoundServerRpc() //Ends a round
    {
        RoundRunning = false;
        ItemValueLastRound.Value = DropOffAreaManager.ItemValue;
        DropOffAreaManager = null;
        NetworkManager.Singleton.SceneManager.LoadScene("Lobby", UnityEngine.SceneManagement.LoadSceneMode.Single);
        if (ItemValueLastRound.Value >= Quota.Value)
        {
            Debug.Log("You won!");
            currentDay += 1;
            SetQuotaAndDungeonSize();
        }
        else
        {
            Debug.Log("Game over. You lost!");
        }
    }
    [ServerRpc]
    public void LvlStartingServerRpc()
    {
        StartCoroutine(LvlRunning());
        RoundRunning = true;
    }

    public void SetQuotaAndDungeonSize()
    {
        float deathBonus = 1f + (0.1f * playerDeaths.Value);
        Quota.Value = (int)(currentDay * 1.3 * deathBonus * 200);


        float quotaNormalized = Mathf.Clamp01((float)Quota.Value / QuotaForMaxSize);
        DungeonSize.Value = Mathf.RoundToInt(Mathf.Lerp(minRooms, maxRooms, quotaNormalized) * Random.Range(0.8f, 1.2f));
    }

    private IEnumerator LvlRunning()
    {
        currentTimeInLvl = 0;

        while (RoundRunning)
        {
            currentTimeInLvl += Time.deltaTime;

            float t = Mathf.Clamp01(currentTimeInLvl / maxRoundTimer);

            float curveValue = spawnRateOverTime.Evaluate(t);

            float spawnInterval = Mathf.Lerp(maxSpawnInverval, minSpawnInterval, curveValue);

            yield return new WaitForSeconds(spawnInterval);

            MapGenerator.SpawnEnemyServerRpc();
        }
    }

}
