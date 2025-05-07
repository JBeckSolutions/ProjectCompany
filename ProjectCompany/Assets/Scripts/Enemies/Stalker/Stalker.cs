using System.Collections.Generic;
using UnityEngine;

public class Stalker : EnemyBase
{
    protected enum EnemyState
    {
        Idle,
        Patrolling,
        Stalking,
        Watching,
        Chasing,
        Hiding

    }

    [SerializeField] protected EnemyState currentState = EnemyState.Idle;
    [SerializeField] protected float stalkSpeed = 3f;
    [SerializeField] protected float _rage = 0;
    [SerializeField] protected float maxRage = 5;
    [SerializeField] protected PlayerState lastSeenPlayer;
    [SerializeField] protected LayerMask layerMaskSeenByPlayerCheck;
    [SerializeField] protected bool recentlyLookedAt = false;
    [SerializeField] protected bool lookedAtLastFrame = false;
    [SerializeField] protected bool receneltyLookedAtResetThisFrame = false;
    [SerializeField] protected bool killedTarget = false;
    [SerializeField] protected float _timeRecentlyLookedAtReset;

    private float rage
    {
        get { return _rage; }
        set { _rage = Mathf.Clamp(value, 0f, 100f); }
    }

    private float timeRecentlyLookedAtReset
    {
        get { return _timeRecentlyLookedAtReset; }
        set { _timeRecentlyLookedAtReset = Mathf.Clamp(value, 0f, 3f); }
    }


    protected override void Start()
    {
        base.Start();

        int enemyLayer = LayerMask.NameToLayer("Player");
        int itemLayer = LayerMask.NameToLayer("Item");
        int propLayer = LayerMask.NameToLayer("Prop");
        int roomLayer = LayerMask.NameToLayer("Room");

        layerMaskSeenByPlayerCheck = ~((1 << enemyLayer) | (1 << itemLayer) | (1 << propLayer) | (1 << roomLayer));
        agent.updateRotation = false;
    }

    protected virtual void Update()
    {
        if (!IsServer) return;

        //Counts the cooldowns down
        timeUntilNextAction -= Time.deltaTime;
        timeUntilNextAttack -= Time.deltaTime;
        timeUntilStunOver -= Time.deltaTime;
        timeRecentlyLookedAtReset -= Time.deltaTime;

        if (recentlyLookedAt && timeRecentlyLookedAtReset <= 0)
        {
            recentlyLookedAt = false;
            receneltyLookedAtResetThisFrame = true;
        }

        if (lastSeenPlayer != null)
        {
            Vector3 direction = (lastSeenPlayer.transform.position - transform.position).normalized;
            direction.y = 0f;

            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 20f);
            }
        }

        var (playerSeen, player) = CanSeePlayer(playerSeenThisFrame);
        var (seenByPlayer, playerSeenBy) = IsSeenByPlayer();

        if (playerSeen)
        {
            rage += 2.5f * Time.deltaTime;
            lastSeenPlayer = player;
            playerSeenThisFrame = true;
            if (currentState != EnemyState.Chasing && currentState != EnemyState.Hiding)
            {
                currentState = EnemyState.Watching;
            }
        }
        else
        {
            playerSeenThisFrame = false;
        }
        
        // Resets the Stalker when he killed a player
        if (killedTarget)
        {
            killedTarget = false;
            rage = 0;
            currentState = EnemyState.Hiding;
            validNewPosition = false;
        }

        // Handles Stalker beeing looked at
        if (seenByPlayer)
        {
            if (!recentlyLookedAt)
            {
                recentlyLookedAt = true;
                timeRecentlyLookedAtReset = float.MaxValue;
            }
            else if (recentlyLookedAt && lookedAtLastFrame)
            {
                recentlyLookedAt = true;
                timeRecentlyLookedAtReset = float.MaxValue;
                rage += 5 * Time.deltaTime;
            }
            //else if (recentlyLookedAt && currentState != EnemyState.Hiding)
            //{
            //    currentState = EnemyState.Chasing;
            //}

        }
        else if (currentState != EnemyState.Watching && currentState != EnemyState.Stalking && currentState != EnemyState.Chasing)
        {
            rage -= 1 * Time.deltaTime;
        }


        // Decides Stalker behaviour
        if (currentState == EnemyState.Hiding)
        {
            currentState = EnemyState.Hiding;
        }
        else if (currentState == EnemyState.Chasing)
        {
            currentState = EnemyState.Chasing;
        }
        else if (currentState == EnemyState.Idle)
        {
            currentState = EnemyState.Idle;
        }
        else if (currentState == EnemyState.Patrolling)
        {
            currentState = EnemyState.Patrolling;
        }
        else if (rage < 30)
        {
            currentState = EnemyState.Watching;
        }
        else if (rage > 30 && rage < 90)
        {
            currentState = EnemyState.Stalking;
        }
        else if (rage > 90)
        {
            currentState = EnemyState.Chasing;
        }


        // Behaviours
        if (currentState == EnemyState.Idle)
        {
            if (timeUntilNextAction <= 0)
            {
                currentState = EnemyState.Patrolling;
            }
        }

        if (currentState == EnemyState.Patrolling)
        {
            agent.speed = walkingSpeed;

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

        if (currentState == EnemyState.Hiding)
        {

            agent.speed = sprintSpeed;

            if (validNewPosition == false)
            {
                float distanceToRunAway = 120f;
                ChooseNewDestination(null, distanceToRunAway, true);
                if (validNewPosition == false)
                {
                    float valueReduction = 10f;
                    while (validNewPosition == false)
                    {
                        distanceToRunAway -= valueReduction;
                        ChooseNewDestination(null, distanceToRunAway, true);
                    }
                }
            }

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                currentState = EnemyState.Idle;
                validNewPosition = false;
                int patrolOrStalk = Random.Range(0, 10);

                if (patrolOrStalk <= 7)
                {
                    timeUntilNextAction = Random.Range(0, 2);
                    currentState = EnemyState.Idle;
                }
                else
                {
                    currentState = EnemyState.Watching;
                }
            }
        }

        if (currentState == EnemyState.Watching)
        {
            agent.speed = walkingSpeed;

            if (playerSeen)
            {
                agent.SetDestination(transform.position);
            }
            else if (lastSeenPlayer != null)
            {
                agent.SetDestination(lastSeenPlayer.transform.position);
            }

            if (receneltyLookedAtResetThisFrame)
            {
                validNewPosition = false;
                currentState = EnemyState.Hiding;
            }
        }

        if (currentState == EnemyState.Stalking)
        {
            agent.speed = stalkSpeed;

            if (seenByPlayer || recentlyLookedAt)
            {
                ChooseNewDestination(transform.position);
            }
            else
            {
                ChooseNewDestination(lastSeenPlayer.transform.position);
            }

            if (player != null && Vector3.Distance(transform.position, player.transform.position) < attackRange)
            {
                Debug.Log("Stalker Starting attack");
                TargetsToHitAndAttack();
            }
            
            if (receneltyLookedAtResetThisFrame)
            {
                validNewPosition = false;
                currentState = EnemyState.Hiding;
            }
        }

        if (currentState == EnemyState.Chasing)
        {
            agent.speed = sprintSpeed;

            if (playerSeenThisFrame && player != null)
            {
                ChooseNewDestination(player.transform.position);

                if (Vector3.Distance(transform.position, player.transform.position) < attackRange)
                {
                    Debug.Log("Stalker Starting attack");
                    TargetsToHitAndAttack();
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

        receneltyLookedAtResetThisFrame = false;
        lookedAtLastFrame = seenByPlayer;

    }

    protected virtual (bool, PlayerState) IsSeenByPlayer()
    {
        float closestDistance = float.MaxValue;
        PlayerState closestSeeingPlayer = null;

        foreach (var player in playerList)
        {
            if (player == null) continue;
            if (player.PlayerAlive.Value == false) continue;

            Camera playerCamera = player.transform.Find("Camera").GetComponent<Camera>();

            if (playerCamera == null) continue;

            Vector3 viewportPoint = playerCamera.WorldToViewportPoint(enemyHead.position); 

            bool inFront = viewportPoint.z > 0f;
            bool inHorizontalView = viewportPoint.x >= 0f && viewportPoint.x <= 1f;
            bool inVerticalView = viewportPoint.y >= 0f && viewportPoint.y <= 1f;

            if (inFront && inHorizontalView && inVerticalView)
            {
                Vector3 origin = player.transform.position;
                Vector3 direction = (transform.position - origin).normalized; 
                float distance = Vector3.Distance(origin, transform.position); 
                if (distance <= 10f)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(origin, direction, out hit, Mathf.Infinity, layerMaskSeenByPlayerCheck))
                    {
                        if (hit.transform == this || hit.transform.IsChildOf(this.transform))
                        {
                            // Stalker is seen
                            if (distance < closestDistance)
                            {
                                closestSeeingPlayer = player;
                                closestDistance = distance;
                            }
                        }
                    }
                }
            }
        }

        return (closestSeeingPlayer != null, closestSeeingPlayer);
    }


    protected override void Attack(List<PlayerState> Targets)
    {
        foreach (var player in Targets)
        {
            Debug.Log("Attack hit ClientId: " + player.OwnerClientId);
            player.TakeDamageServerRpc(attackDamage);
        }

        if (Targets.Count > 0)
        {
            killedTarget = true;
        }

        isAttacking = false;
    }
}

