using UnityEngine;

public class EnemyJumper :EnemyBase
{
    [Header("Faster when in sight")]
    [SerializeField] private float sprintSpeedMultiplier = 2.0f;
    [SerializeField] private float walkingSpeedMultiplier = 1.5f;

    protected override float GetChaseSpeed()
    {
        bool visibleToPlayer = IsSeenByAnyPlayer();
        if (visibleToPlayer)
        {
            // For example: boost speed when being watched
            return base.sprintSpeed * sprintSpeedMultiplier;
        }
        return base.sprintSpeed;
    }
}
