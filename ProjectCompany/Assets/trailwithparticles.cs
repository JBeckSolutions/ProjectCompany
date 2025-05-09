using UnityEngine;

public class TrailWithParticles : MonoBehaviour
{
    public TrailRenderer trailRenderer; // The trail renderer attached to your object
    public ParticleSystem particleSystem; // The particle system to emit particles

    private void Start()
    {
        if (trailRenderer == null) trailRenderer = GetComponent<TrailRenderer>();
        if (particleSystem == null) particleSystem = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        // Ensure the particle system is following the object
        EmitParticlesAlongTrail();
    }

    private void EmitParticlesAlongTrail()
    {
        // Emit particles at each position along the trail path
        for (int i = 0; i < trailRenderer.positionCount; i++)
        {
            Vector3 position = trailRenderer.GetPosition(i);
            var main = particleSystem.main;
            main.startLifetime = 0.1f; // Adjust particle lifespan
            particleSystem.transform.position = position;
            particleSystem.Emit(1); // Emit one particle at the current trail position
        }
    }
}
