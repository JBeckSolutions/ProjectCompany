using System;
using UnityEngine;

public class Octopode_animation_SoundEvents : MonoBehaviour
{
    public WwiseEventPost Tentacle_1;
    public WwiseEventPost Tentacle_2;
    public WwiseEventPost Tentacle_3;
    public WwiseEventPost Tentacle_4;
    public WwiseEventPost Tentacle_5;
    public WwiseEventPost Tentacle_6;
    public WwiseEventPost Tentacle_7;
    public WwiseEventPost Tentacle_8;
    
    private short TentacleIndexLift = 0;
    private short TentacleIndexStep = 0;
    
    private WwiseEventPost[] TentacleList;
    public void Awake()
    {
        TentacleList = new WwiseEventPost[8]
        {
            Tentacle_5,
            Tentacle_6,
            Tentacle_4,
            Tentacle_1,
            Tentacle_2,
            Tentacle_8,
            Tentacle_3,
            Tentacle_7,
        };
    }

    public void WwisePostTentacleLift()
    {
        TentacleIndexLift++;
        TentacleIndexLift %= 8;
        
        TentacleList[TentacleIndexLift].PostWwiseEvent();
    }
    
    public void WwisePostTentacleStep()
    {
        TentacleIndexStep++;
        TentacleIndexStep %= 8;
        
        TentacleList[TentacleIndexStep].PostWwiseEvent();
    }
    
    
}
