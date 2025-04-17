using UnityEngine;

public class EnemyChaser : EnemyBase
{
    [Header("Stops when in sight.")]
    [SerializeField] private float sprintSpeedMultiplier = 0.0f;
    [SerializeField] private float walkingSpeedMultiplier = 0.0f;

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
