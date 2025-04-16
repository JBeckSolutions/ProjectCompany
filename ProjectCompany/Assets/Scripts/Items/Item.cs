using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Item : NetworkBehaviour
{
    public NetworkVariable<bool> PickupAble = new NetworkVariable<bool>(true);
    public string itemName = "Item";
    public int itemValue = 10;
    public Texture2D InventoryImage;

    [SerializeField] private GameObject model;
    [SerializeField] private Collider itemCollider;
    [SerializeField] private int ItemWeight = 1;

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

            SyncLocalPositionToClientsClientRpc(localHandPosition);
        }
    }
    [ClientRpc]
    private void SyncLocalPositionToClientsClientRpc(Vector3 localHandPosition) //Syncs the position to the clients
    {
        this.transform.localPosition = localHandPosition;
        this.transform.localRotation = Quaternion.identity;
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
        itemCollider.enabled = state;
    }
}
