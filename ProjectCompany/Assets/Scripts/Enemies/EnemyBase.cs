using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class EnemyBase : NetworkBehaviour
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

    [Header("Detection")]
    [SerializeField] private List<PlayerState> playerList;
    [SerializeField] float viewRadius = 10f;
    [UnityEngine.Range(0, 360)]
    public float viewAngle = 90f;
    [SerializeField] bool playerSeenThisFrame = false;
    [SerializeField] int layerMask;


    [SerializeField] NavMeshAgent agent;

    private void Start()
    {
        layerMask = ~LayerMask.GetMask("Enemy");
    }

    public override void OnNetworkSpawn()
    {
        playerList = GameManager.Singelton.PlayerStates;
    }

    public virtual void Update()
    {
        if (!IsServer) return;

        var (playerSeen, player) = CanSeePlayer(playerSeenThisFrame);
        
        if (playerSeen)
        {
            currentState = EnemyState.Chasing;
            playerSeenThisFrame = true;
        }
        else
        {
            playerSeenThisFrame = false;
            player = null;
        }

        timeUntilNextAction -= Time.deltaTime;
        if (currentState == EnemyState.Chasing)
        {
            if (playerSeenThisFrame && player != null)
            {
                ChooseNewDestination(player.transform.position);
            }
            else 
            {
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    currentState = EnemyState.Idle;
                    validNewPosition = false;
                    timeUntilNextAction = Random.Range(0, 2);
                }
            }
        }

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
    public virtual void ChooseNewDestination(Vector3? Destination = null)
    {
        Vector3 nextDestination = Destination ?? (Random.insideUnitSphere * maxDistance + transform.position);

        if (NavMesh.SamplePosition(nextDestination, out NavMeshHit hit, maxDistance, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            validNewPosition = true;
        }
    }

    public virtual (bool, PlayerState) CanSeePlayer(bool PlayerSpotted)
    {
        float closestSeenPlayerDistance = float.MaxValue;
        PlayerState closestSeenPlayer = null;

        foreach (var player in playerList)
        {
            if (player == null) continue;

            Vector3 dirToPlayer = (player.transform.position - transform.position).normalized;
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (distanceToPlayer < viewRadius)
            {
                if (PlayerSpotted || Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
                {
                    if (!Physics.Raycast(transform.position, dirToPlayer, distanceToPlayer, layerMask))
                    {
                        if (closestSeenPlayerDistance > distanceToPlayer)
                        {
                            closestSeenPlayer = player;
                            closestSeenPlayerDistance = distanceToPlayer;
                        }
                    }
                }
            }
        }

        return (closestSeenPlayer != null, closestSeenPlayer);
    }

    private void OnDrawGizmos()
    {
        // Only visualize the view cone if the object is selected in the scene view
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, viewRadius);  // Draw the view radius (as a wire sphere)

        // Draw the view cone (using a frustum-like shape)
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward * viewRadius;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward * viewRadius;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary); // Left boundary
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary); // Right boundary

        // Optionally, you can draw the view cone arc as lines
        int segments = 10;
        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Lerp(-viewAngle / 2, viewAngle / 2, i / (float)segments);
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward * viewRadius;
            Gizmos.DrawLine(transform.position, transform.position + direction);
        }
    }


}
