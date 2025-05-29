using Unity.Netcode;
using UnityEditor;
using UnityEngine;
/// <summary>
/// The code of the this script was written by: Beck Jonas
/// </summary>
public class InteractableObject : NetworkBehaviour
{
    //Base class for items that stay in the world when interacted with

    public string ObjectName;
    public NetworkVariable<bool> interactable = new NetworkVariable<bool>(true);
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            interactable.Value = true;
        }
    }
    public virtual void Use()
    {

    }
}
