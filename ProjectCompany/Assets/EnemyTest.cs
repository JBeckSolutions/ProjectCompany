using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTest : NetworkBehaviour
{
    private enum EnemyState
    {
        Idle,
        Patrolling,
        Chasing
    }

    [SerializeField] private EnemyState currentState = EnemyState.Idle;
    [SerializeField] private float timeUntilNextAction;
    [SerializeField] private float maxDistance = 10f;   //Maximum distance it can move with one Patrol
    [SerializeField] private bool validNewPosition = false;

    [SerializeField] NavMeshAgent agent;

    private void Update()
    {
        if (!IsServer) return;

        timeUntilNextAction -= Time.deltaTime;

        if (currentState == EnemyState.Idle)
        {
            if (timeUntilNextAction <= 0)
            {
                currentState = EnemyState.Patrolling;
            }
        }

        if (currentState == EnemyState.Patrolling)
        {
            if (!validNewPosition)
            {
                ChooseNewDestination();
            }

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                currentState = EnemyState.Idle;
                validNewPosition = false;
                timeUntilNextAction = Random.Range(0, 2);
            }
        }
        
    }
    private void ChooseNewDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * maxDistance;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, maxDistance, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            validNewPosition = true;
        }
    }


}
