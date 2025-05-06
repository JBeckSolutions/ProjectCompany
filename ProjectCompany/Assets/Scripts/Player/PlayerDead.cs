using Unity.Netcode;
using UnityEngine;

public class PlayerDead : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < GameManager.Singelton.PlayerStates.Count; i++)
        {
            if (GameManager.Singelton.PlayerStates[i].PlayerAlive.Value == true)
            {
                GameManager.Singelton.PlayerStates[i].model.SetActive(false);
                this.transform.position = GameManager.Singelton.PlayerStates[i].playerCamera.transform.position;
                this.transform.rotation = GameManager.Singelton.PlayerStates[i].playerCamera.transform.rotation;
            }
        }
    }
}
