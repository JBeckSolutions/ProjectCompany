using UnityEngine;
using AK.Wwise;  // Make sure you have Wwise namespace

public class LampSoundController : MonoBehaviour
{
    [SerializeField] private AK.Wwise.Event playEvent; 

    private uint playingID = 0;
    private int playersInRange = 0;

    private int playerLayer;

    private void Awake()
    {
        playerLayer = LayerMask.NameToLayer("Player");
    }

    private void Start()
    {
        playingID = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            playersInRange++;
            if (playersInRange <= 1 && playingID == 0)
            {
                playingID = playEvent.Post(gameObject);
                Debug.Log("Lamp sound started");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            playersInRange--;
            if (playersInRange <= 0)
            {
                playersInRange = 0;

                if (playingID != 0)
                {
                    AkSoundEngine.StopPlayingID(playingID, 0);
                    playingID = 0;
                }

                Debug.Log("Lamp sound stopped");
            }
        }
    }
}