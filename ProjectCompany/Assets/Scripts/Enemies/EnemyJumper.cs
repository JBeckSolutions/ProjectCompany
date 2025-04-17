using UnityEngine;

public class EnemyJumper :EnemyBase
{
    [Header("Faster when in sight")]
    [SerializeField] private float sprintSpeedMultiplier = 2.0f;
    [SerializeField] private float walkingSpeedMultiplier = 1.5f;

    protected override float GetChaseSpeed()
    {
        if (playerSeenThisFrame)
        {
            return base.sprintSpeed * sprintSpeedMultiplier;
        }
        return base.sprintSpeed;
    }

    public override void ChooseNewDestination(Vector3? Destination = null)
    {
        // Call base method first to preserve core functionality
        base.ChooseNewDestination(Destination);

        // Apply walking speed boost if patrolling while player is seen
        if (playerSeenThisFrame && currentState == EnemyState.Patrolling)
        {
            agent.speed = walkingSpeed * walkingSpeedMultiplier;
        }
    }
}
