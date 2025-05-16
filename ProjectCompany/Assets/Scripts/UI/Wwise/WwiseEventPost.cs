using UnityEngine;

public class WwiseEventPost : MonoBehaviour
{
    public AK.Wwise.Event WwiseEvent;
    public void PostWwiseEvent()
    {
        if (WwiseEvent != null)
        {
            WwiseEvent.Post(gameObject);
        }
        else
        {
            Debug.LogWarning("Wwise Event is not assigned.");
        }
    }
}
