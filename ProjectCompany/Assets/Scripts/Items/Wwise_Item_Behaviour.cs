using UnityEngine;

public class Wwise_Item_Behaviour : MonoBehaviour
{
    public AK.Wwise.Event WwiseEventItemPickup;
    public AK.Wwise.Event WwiseEventItemDrop;
    public AK.Wwise.Event WwiseEventItemIdle;
    public AK.Wwise.Event WwiseEventItemIdleStop;
    
    public void PlayItemPickupSound()
    {
        WwiseEventItemPickup.Post(gameObject);
    }
    
    public void PlayItemDropSound()
    {
        WwiseEventItemDrop.Post(gameObject);
    }
    
    public void UnmuteItemIdleSound()
    {
        WwiseEventItemIdle.Post(gameObject);
    }
    
    public void MuteItemIdleSound()
    {
        WwiseEventItemIdleStop.Post(gameObject);
    }
}
