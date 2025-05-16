using UnityEngine;

public class WwiseSetState : MonoBehaviour
{
    public AK.Wwise.State State;

    public void SetState()
    {
        State.SetValue();
    }
}
