using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
/// <summary>
/// The code of the this script was written by: Beck Jonas
/// </summary>
public class GameUi : MonoBehaviour
{
    [SerializeField] List<GameObject> Menus;
    
    public AK.Wwise.Bank Bank_InGameMenuUI;
    
    public void CloseAllMenus()
    {
        Bank_InGameMenuUI.Unload();
        
        foreach (var menu in Menus)
        {
            menu.SetActive(false);
        }
    }

    public void OpenMenu(string MenuToOpen)
    {
        Bank_InGameMenuUI.Load();
        foreach (var menu in Menus)
        {
            if (MenuToOpen == menu.name)
            {
                menu.SetActive(true);
            }
        }
    }
    public void LockCurser()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void UnlockCurser()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void EnablePlayerControlls()
    {
        PlayerController localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject.GetComponent<PlayerController>();
        if (localPlayer != null)
        {
            localPlayer.controllsEnabled = true;
            return;
        }

        DeadPlayerController localDeadPlayer = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject.GetComponent<DeadPlayerController>();

        if (localDeadPlayer != null)
        {
            localPlayer.controllsEnabled = true;
            return;
        }

    }
}
