using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class EnemyBase : NetworkBehaviour
{
    protected enum EnemyState
    {
        Idle,
        Patrolling,
        Chasing
    }

    [SerializeField] protected EnemyState currentState = EnemyState.Idle;
    [SerializeField] protected float MaxTimeUntilNextAction = 2;          //Max time the enemy can stay in the idle State
    [SerializeField] protected float timeUntilNextAction;                 //How long the enemy will stay in the Idle State
    [SerializeField] protected float maxDistance = 10f;                   //Maximum distance it can move with one Patrol
    [SerializeField] protected bool validNewPosition = false;             //Tracks if the Enemy has a valid path to move towards to when in the patrolling state

    [Header("Movement")]
    [SerializeField] protected float walkingSpeed = 3.5f;    //Speed when the enemy is walking
    [SerializeField] protected float sprintSpeed = 3.5f;     //Speed when the enemy is sprinting

    [Header("Detection")]
    [SerializeField] protected List<PlayerState> playerList;      //List of all players in the game
    [SerializeField] protected float viewRadius = 10f;
    [UnityEngine.Range(0, 360)]
    [SerializeField] protected float viewAngle = 90f;
    [SerializeField] protected bool playerSeenThisFrame = false;
    [SerializeField] protected int layerMask;                     //Layer Mask that will be ignored in the Raycast check so it doesnt collide with the "Enemy" Layer
    [Header("Attack")]
    [SerializeField] protected float attackRange = 2f;    //How far the player has to be for the attack to Start
    [SerializeField] protected float attackCoooldown = 1; //Cooldown of the Attack
    [SerializeField] protected float timeUntilNextAttack; //Time until the next Attack can happen
    [SerializeField] protected float timeStunnedAfterAttack = 1f; //Time how long the enemy cant move after an attack
    [SerializeField] protected float timeUntilStunOver;           //Counts down until the enemy can move again
    [SerializeField] protected AbilityHitbox hitbox;           //Script that is used by the Attack to check what is being hit
    [SerializeField] protected bool canMoveWhileAttacking = false;    //Can the enemy Move while attacking? (might be unneeded)
    [SerializeField] protected bool isAttacking = false; //Is the attack finished? (might be unneeded)

    [SerializeField] protected NavMeshAgent agent;

    protected virtual void Start()
    {
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int groundLayer = LayerMask.NameToLayer("Ground");
        int itemLayer = LayerMask.NameToLayer("Item");

        layerMask = ~((1 << enemyLayer) | (1 << groundLayer) | (1 << itemLayer));
    }

    public override void OnNetworkSpawn()
    {
        playerList = GameManager.Singelton.PlayerStates;
    }
    
    protected virtual float GetChaseSpeed()
    {
        return sprintSpeed;
    }
    protected virtual void Update()
    {
        if (!IsServer) return;

        //Counts the cooldowns down
        timeUntilNextAction -= Time.deltaTime;
        timeUntilNextAttack -= Time.deltaTime;
        timeUntilStunOver -= Time.deltaTime;

        if (isAttacking && canMoveWhileAttacking == false) return;  //checks if enemy is still attacking and cant move while attacking
        if (timeUntilStunOver >= 0) return; //checks if it is able to act again after an attack

        var (playerSeen, player) = CanSeePlayer(playerSeenThisFrame);

        //Check if Enemy sees a player

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

        if (currentState == EnemyState.Chasing)
        {
            if (playerSeenThisFrame && player != null)
            {
                agent.speed = GetChaseSpeed();  // Set speed to sprint when player is chased
                ChooseNewDestination(player.transform.position);
                if (timeUntilNextAttack <= 0 && Vector3.Distance(transform.position, player.transform.position) < attackRange)
                {
                    Debug.Log("Starting attack");

                    if (canMoveWhileAttacking == false)
                    {
                        agent.SetDestination(transform.position);   //Stops the enemy to Attack
                    }
                    isAttacking = true;
                    TargetsToHitAndAttack();
                    timeUntilNextAttack = attackCoooldown;
                    timeUntilStunOver = timeStunnedAfterAttack;
                }
            }
            else
            {
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    agent.speed = walkingSpeed; // Set speed to walking speed when player is lost
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
                timeUntilNextAction = Random.Range(0, MaxTimeUntilNextAction);
            }
        }

    }
    protected virtual void ChooseNewDestination(Vector3? Destination = null, float? CustomDistance = null, bool goFar = false)
    {
        float distance = CustomDistance ?? maxDistance;

        Vector3 nextDestination = Destination ?? (Random.insideUnitSphere * distance + transform.position);
        if (goFar == false)
        {
            if (NavMesh.SamplePosition(nextDestination, out NavMeshHit hit, distance, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                validNewPosition = true;
            }
        }
        else
        {
            Vector3 basePosition = transform.position;

            for (int i = 0; i < 300; i++)
            {
                Vector3 randomDirection = Random.insideUnitSphere * distance;
                randomDirection.y = 0; // Keep it horizontal
                Vector3 potentialPosition = Destination ?? (basePosition + randomDirection);

                if (NavMesh.SamplePosition(potentialPosition, out NavMeshHit hit, 5f, NavMesh.AllAreas))
                {
                    NavMeshPath path = new NavMeshPath();
                    if (agent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete)
                    {
                        float pathLength = GetPathLength(path);

                        if (pathLength >= distance * 0.8f)
                        {
                            agent.SetDestination(hit.position);
                            validNewPosition = true;
                            return;
                        }
                    }
                }
            }

            Debug.LogWarning("Could not find long path");
        }
        
    }

    protected virtual (bool, PlayerState) CanSeePlayer(bool PlayerSpotted)
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
                    RaycastHit hit;
                    if (!Physics.Raycast(transform.position, dirToPlayer, out hit, distanceToPlayer, layerMask, QueryTriggerInteraction.Ignore))
                    {
                        if (closestSeenPlayerDistance > distanceToPlayer)
                        {
                            closestSeenPlayer = player;
                            closestSeenPlayerDistance = distanceToPlayer;
                        }
                    }
                    else
                    {
                        //Debug.Log("Ray hit: " + hit.collider.name);
                    }
                }
            }
        }

        return (closestSeenPlayer != null, closestSeenPlayer);
    }

    protected virtual void TargetsToHitAndAttack()
    {
        hitbox.GetPlayersToHit((players) =>
        {
            Attack(players);
        });
    }

    protected virtual void Attack(List<PlayerState> Targets)
    {
        foreach (var player in Targets)
        {
            Debug.Log("Attack hit ClientId: " + player.OwnerClientId);
        }

        isAttacking = false;
    }

    private float GetPathLength(NavMeshPath path)
    {
        float length = 0f;

        for (int i = 1; i < path.corners.Length; i++)
        {
            length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        }

        return length;
    }

    protected void OnDrawGizmos()
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