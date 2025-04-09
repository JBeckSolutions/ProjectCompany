using Unity.Netcode;
using UnityEngine;

public class InteractableObject : NetworkBehaviour
{
    //Base class for items that stay in the world when interacted with
    public virtual void Use()
    {

    }
}
