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
    [SerializeField] protected float _rage = 0;
    [SerializeField] protected float maxRage = 5;
    [SerializeField] protected int layerMaskSeenByPlayerCheck;
    [SerializeField] protected PlayerState lastSeenPlayer;

    private float rage
    {
        get { return _rage; }
        set { _rage = Mathf.Clamp(value, 0f, 100f); }
    }

    protected override void Start()
    {
        base.Start();

        int playerLayer = LayerMask.NameToLayer("Player");
        int groundLayer = LayerMask.NameToLayer("Ground");
        int itemLayer = LayerMask.NameToLayer("Item");

        layerMaskSeenByPlayerCheck = ~((1 << playerLayer) | (1 << groundLayer) | (1 << itemLayer));

        agent.updateRotation = false;
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
        var (seenByPlayer, playerSeenBy) = IsSeenByPlayer();

        if (seenByPlayer && timeUntilNextAction > 0)
        {
            currentState = EnemyState.Hiding;
            validNewPosition = false;
        }
        else if(timeUntilNextAction > 0)
        {
            return;
        }

        if (playerSeen)
        {

            lastSeenPlayer = player;

            playerSeenThisFrame = true;
            if (rage < 30)
            {
                currentState = EnemyState.Watching;
            }
            else if (rage > 30 && rage < 95)
            {
                currentState = EnemyState.Watching;
            }
            else if (rage > 95)
            {
                currentState = EnemyState.Chasing;
            }
        }
        else
        {
            playerSeenThisFrame = false;
            player = null;
        }

        if (currentState == EnemyState.Hiding)
        {
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
                agent.speed = walkingSpeed; // Set speed to walking speed when player is lost
                currentState = EnemyState.Idle;
                validNewPosition = false;
                timeUntilNextAction = Random.Range(0, 10);
            }
        }

        if (currentState == EnemyState.Watching)
        {
            if (playerSeen)
            {
                agent.SetDestination(transform.position);
            }
            else if (lastSeenPlayer != null)
            {
                agent.SetDestination(lastSeenPlayer.transform.position);
            }

            if (seenByPlayer)
            {
                rage += 10;
                currentState = EnemyState.Hiding;
                validNewPosition = false;
            }
            else
            {
                rage += 1 * Time.deltaTime;
            }

        }

        if (currentState == EnemyState.Stalking)
        {

        }

        if (currentState == EnemyState.Chasing)
        {
            if (playerSeenThisFrame && player != null)
            {
                agent.speed = sprintSpeed;  // Set speed to sprint when player is chased
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

    protected virtual (bool, PlayerState) IsSeenByPlayer()
    {
        float closestDistance = float.MaxValue;
        PlayerState closestSeeingPlayer = null;

        foreach (var player in playerList)
        {
            if (player == null) continue;

            Camera playerCamera = player.transform.Find("Camera").GetComponent<Camera>();

            if (playerCamera == null) continue;

            Vector3 viewportPoint = playerCamera.WorldToViewportPoint(transform.position);

            bool inFront = viewportPoint.z > 0f;
            bool inHorizontalView = viewportPoint.x >= 0f && viewportPoint.x <= 1f;
            bool inVerticalView = viewportPoint.y >= 0f && viewportPoint.y <= 1f;

            if (inFront && inHorizontalView && inVerticalView)
            {
                Vector3 origin = playerCamera.transform.position;
                Vector3 direction = (transform.position - origin).normalized;
                float distance = Vector3.Distance(origin, transform.position);

                RaycastHit hit;
                if (!Physics.Raycast(origin, direction, out hit, distance, layerMaskSeenByPlayerCheck, QueryTriggerInteraction.Ignore) || hit.transform == transform)
                {
                    if (distance < closestDistance)
                    {
                        closestSeeingPlayer = player;
                        closestDistance = distance;
                    }
                }
            }
        }

        return (closestSeeingPlayer != null, closestSeeingPlayer);
    }
}
