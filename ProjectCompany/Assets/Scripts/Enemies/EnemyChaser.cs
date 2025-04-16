using UnityEngine;

public class EnemyChaser : EnemyBase
{
    [Header("Stops when in sight.")]
    [SerializeField] private float speedMultiplier = 0.0f; // Multiplier to increase chase speed

    // Override the chasing speed

    private Renderer enemyRenderer;

    private void Awake()
    {
        enemyRenderer = GetComponent<Renderer>();
    }

    protected override float GetChaseSpeed()
    {
        // Check if a Renderer exists and if the enemy is visible
        if (enemyRenderer != null && enemyRenderer.isVisible)
        {
            // The enemy is seen by a camera, so run faster
            return sprintSpeed * speedMultiplier;
        }
        else
        {
            // Not visible – use normal chase speed
            return sprintSpeed;
        }
    }

}
