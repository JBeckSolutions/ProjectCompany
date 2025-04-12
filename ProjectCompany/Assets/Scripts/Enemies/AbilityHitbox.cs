using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Rendering.VirtualTexturing;

public class AbilityHitbox : MonoBehaviour
{
    [SerializeField] private Collider hitbox;   //Referent to the Collider used for the attack
    [SerializeField] private List<PlayerState> playersToHit;    
    private void Start()
    {
        hitbox.enabled = false;
    }

    public void GetPlayersToHit(System.Action<List<PlayerState>> callback)  //Starts the Coroutine
    {
        //Debug.Log("Starting Coroutine");
        StartCoroutine(GetPlayersToHitRoutine(callback));
    }

    private IEnumerator GetPlayersToHitRoutine(System.Action<List<PlayerState>> callback)   
    {
        attackStart();  //Enables the collider
        yield return new WaitForFixedUpdate();  //Waits for the next Physics update
        List<PlayerState> result = new List<PlayerState>(playersToHit); //Copies the list in a result variable
        attackEnd();    //Deaktivates the collider and empties the playersToHit list

        //Debug.Log("Invoking Callback");
        callback?.Invoke(result);   //Invokes the callback with the result list
    }

    private void attackStart()  //enables the HItbox
    {
        hitbox.enabled = true;
    }
    private void attackEnd()    //disables the hitbox and resets the list
    {
        playersToHit = new List<PlayerState>();
        hitbox.enabled = false;
    }

    private void OnTriggerEnter(Collider other) //Adds the player to the list if its caught entering the hitbox while its active
    {
        if (other.GetComponent<PlayerState>())
        {
            if (playersToHit.Contains(other.GetComponent<PlayerState>())) return;

            playersToHit.Add(other.GetComponent<PlayerState>());
        }    
    }

    private void OnTriggerStay(Collider other)  //Adds the player to the list if its caught inside the hitbox while its active
    {
        if (other.GetComponent<PlayerState>())
        {
            if (playersToHit.Contains(other.GetComponent<PlayerState>())) return;

            playersToHit.Add(other.GetComponent<PlayerState>());
        }
    }
}
