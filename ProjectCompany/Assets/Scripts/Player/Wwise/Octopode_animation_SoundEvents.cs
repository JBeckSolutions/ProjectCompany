using System;
using UnityEngine;

public class Octopode_animation_SoundEvents : MonoBehaviour
{
    [Header("Tentacle Lift")]
    public WwiseCallIndex Tentacle_1;
    public WwiseCallIndex Tentacle_2;
    public WwiseCallIndex Tentacle_3;
    public WwiseCallIndex Tentacle_4;
    public WwiseCallIndex Tentacle_5;
    public WwiseCallIndex Tentacle_6;
    public WwiseCallIndex Tentacle_7;
    public WwiseCallIndex Tentacle_8;
    
    private short TentacleIndexLift = 0;
    private WwiseCallIndex[] TentacleList;
    
    private short TentacleIndexStep = 0;
    public bool PlayerStunned = false;
    public void Awake()
    {
        TentacleList = new WwiseCallIndex[8]
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

        if (PlayerStunned) return;
        TentacleList[TentacleIndexLift].PostWwiseEvent("Lift");
    }
    
    public void WwisePostTentacleStep()
    {
        TentacleIndexStep++;
        TentacleIndexStep %= 8;
        
        if (PlayerStunned) return;
        TentacleList[TentacleIndexStep].PostWwiseEvent("Step");
    }
    
}
