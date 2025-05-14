using UnityEngine;

public class Octopode_animation_SoundEvents : MonoBehaviour
{
    public AK.Wwise.Event WwiseEvent_Octopode_Tentacle_Lift;
    public AK.Wwise.Event WwiseEvent_Octopode_Tentacle_Step;

    public void Post_WwiseEvent_Octopode_Tentacle_Lift()
    {
        WwiseEvent_Octopode_Tentacle_Lift.Post(gameObject);
    }
    
    public void Post_WwiseEvent_Octopode_Tentacle_Step()
    {
        WwiseEvent_Octopode_Tentacle_Step.Post(gameObject);
    }
    
}
