using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Unity.IO.LowLevel.Unsafe;

public class EnemyBase : NetworkBehaviour
{
    private enum EnemyState
    {
        Idle,
        Patrolling,
        Chasing
    }

    [SerializeField] private EnemyState currentState = EnemyState.Idle;
    [SerializeField] private float MaxTimeUntilNextAction = 2;          //Max time the enemy can stay in the idle State
    [SerializeField] private float timeUntilNextAction;                 //How long the enemy will stay in the Idle State
    [SerializeField] private float maxDistance = 10f;                   //Maximum distance it can move with one Patrol
    [SerializeField] private bool validNewPosition = false;             //Tracks if the Enemy has a valid path to move towards to when in the patrolling state

    [Header("Movement")]
    [SerializeField] protected float walkingSpeed = 3.5f;    //Speed when the enemy is walking
    [SerializeField] protected float sprintSpeed = 3.5f;     //Speed when the enemy is sprinting

    [Header("Detection")]
    [SerializeField] private List<PlayerState> playerList;      //List of all players in the game
    [SerializeField] private float viewRadius = 10f;
    [UnityEngine.Range(0, 360)]
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private bool playerSeenThisFrame = false;  
    [SerializeField] private int layerMask;                     //Layer Mask that will be ignored in the Raycast check so it doesnt collide with the "Enemy" Layer
    [Header("Attack")]
    [SerializeField] private float attackRange = 2f;    //How far the player has to be for the attack to Start
    [SerializeField] private float attackCoooldown = 1; //Cooldown of the Attack
    [SerializeField] private float timeUntilNextAttack; //Time until the next Attack can happen
    [SerializeField] private float timeStunnedAfterAttack = 1f; //Time how long the enemy cant move after an attack
    [SerializeField] private float timeUntilStunOver;           //Counts down until the enemy can move again
    [SerializeField] private AbilityHitbox hitbox;           //Script that is used by the Attack to check what is being hit
    [SerializeField] private bool canMoveWhileAttacking = false;    //Can the enemy Move while attacking? (might be unneeded)
    [SerializeField] private bool isAttacking = false; //Is the attack finished? (might be unneeded)

    [SerializeField] private NavMeshAgent agent;

    private void Start()
    {
        layerMask = ~LayerMask.GetMask("Enemy");
    }

    public override void OnNetworkSpawn()
    {
        playerList = GameManager.Singelton.PlayerStates;
    }

    protected virtual float GetChaseSpeed()
    {
        return sprintSpeed;
    }

    public virtual void Update()
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

    public virtual void TargetsToHitAndAttack()
    {
        hitbox.GetPlayersToHit((players) =>
        {
            Attack(players);
        });
    }

    public virtual void Attack(List<PlayerState> Targets)
    {
        foreach (var player in Targets)
        {
            Debug.Log("Attack hit ClientId: " + player.OwnerClientId);
        }

        isAttacking = false;
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
