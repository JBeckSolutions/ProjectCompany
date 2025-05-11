using Unity.Netcode;
using UnityEditor;
using UnityEngine;

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
