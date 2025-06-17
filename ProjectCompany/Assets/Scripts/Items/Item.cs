using Unity.Netcode;
using UnityEngine;
/// <summary>
/// The code of the this script was written by: Beck Jonas
/// </summary>
public class Item : NetworkBehaviour
{
    public NetworkVariable<bool> PickupAble = new NetworkVariable<bool>(true);
    public string itemName = "Item";
    public int itemValue = 10;
    public Sprite InventoryImage;
    public int ItemWeight = 1;
    public Wwise_Item_Behaviour Wwise_Item_Behaviour; //This is the Wwise Item Behaviour script that handles the sound effects for the item

    [SerializeField] private GameObject model;
    [SerializeField] private Collider itemCollider;

    [ServerRpc(RequireOwnership = false)]
    public virtual void PickUpServerRpc(NetworkObjectReference PlayerReference) //Server picks the item up for the client
    {
        if (PlayerReference.TryGet(out NetworkObject Player))
        {
            
            if (this.OwnerClientId != Player.OwnerClientId)
            {
                this.GetComponent<NetworkObject>().ChangeOwnership(Player.OwnerClientId);
            }
            
            PickupAble.Value = false;
            this.transform.SetParent(Player.transform);
            

            Vector3 handWorldPosition = Player.transform.GetComponent<PlayerInventoryManager>().playerHand.position;
            Vector3 localHandPosition = Player.transform.InverseTransformPoint(handWorldPosition);

            this.transform.localPosition = localHandPosition;
            this.transform.localRotation = Quaternion.identity;
            this.gameObject.tag = "PickedUp";
            Wwise_Item_Behaviour.PlayItemPickupSound();
            SyncLocalPositionToClientsClientRpc(localHandPosition);
        }
    }
    [ClientRpc]
    private void SyncLocalPositionToClientsClientRpc(Vector3 localHandPosition) //Syncs the position to the clients
    {
        this.transform.localPosition = localHandPosition;
        this.transform.localRotation = Quaternion.identity;
        this.gameObject.tag = "PickedUp";
    }
    [ServerRpc]
    public virtual void DropServerRpc(Vector3 position) //Server drops the item at the specefied position and sets the item parent to null
    {
        PickupAble.Value = true;
        if (GameObject.Find("GeneratedItems(Clone)"))
        {
            this.transform.SetParent(GameObject.Find("GeneratedItems(Clone)").transform);
        }
        else
        {
            this.transform.SetParent(null);
        }
        this.transform.position = position;
        this.gameObject.tag = "Untagged";
        Wwise_Item_Behaviour.PlayItemDropSound();
        DropClientRpc(position);
    }
    [ClientRpc]
    public virtual void DropClientRpc(Vector3 position) //Syncs the position to the clients
    {
        if (GameObject.Find("GeneratedItems(Clone)"))
        {
            this.transform.SetParent(GameObject.Find("GeneratedItems(Clone)").transform);
        }
        else
        {
            this.transform.SetParent(null);
        }
        this.transform.position = position;
        this.gameObject.tag = "Untagged";
    }
    [ServerRpc]
    public void ToggleVisibilityServerRpc(bool state)   //Server send all clients the message to run the ToggleVisibilityClientRpc function
    {
        ToggleVisibilityClientRpc(state);
    }
    [ClientRpc]
    private void ToggleVisibilityClientRpc(bool state)  //Toggles visibility on all clients
    {
        model.SetActive(state);
        if (state)
            Wwise_Item_Behaviour.UnmuteItemIdleSound();
        else
            Wwise_Item_Behaviour.MuteItemIdleSound();
    }
}
