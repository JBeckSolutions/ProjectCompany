using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class InteractableObject : NetworkBehaviour
{
    public string ObjectName;
    public NetworkVariable<bool> interactable = new NetworkVariable<bool>(true);
    //Base class for items that stay in the world when interacted with
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
