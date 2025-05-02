using UnityEngine;
using UnityEngine.AI;

public class EnemyJumper :EnemyBase
{
    protected enum state
    {
        chasing,
        searching,
        jumping
    }

    [SerializeField] protected float speed = 3f;
    [SerializeField] protected float jumpMultiplier = 10f;
    [SerializeField] protected float patrolRadius = 20f;
    [SerializeField] protected float visionRange = 25f;
    [SerializeField] protected float visionAngle = 90f;
    [SerializeField] protected int layerMaskSeenByPlayerCheck;

    [SerializeField] protected state action = state.searching;

    protected GameObject player;
    private Transform targetPlayer;

    protected override void Start()
    {
        base.Start();

        int playerLayer = LayerMask.NameToLayer("Player");
        int groundLayer = LayerMask.NameToLayer("Ground");
        int itemLayer = LayerMask.NameToLayer("Item");

        layerMaskSeenByPlayerCheck = ~((1 << playerLayer) | (1 << groundLayer) | (1 << itemLayer));
    }

    protected virtual void Update()
    {
        if (!IsServer) return;
        if (timeUntilNextAction >= 0)
        {
            timeUntilNextAction -= Time.deltaTime;
        }

        if (IsSeenByPlayer())
        {
            action = state.jumping;
            Jumping();
        }
        else
        {
            switch (action)
            {
                case state.searching:
                Searching();
                LookForPlayer();
                break;

                case state.chasing:
                Chasing();
                break;
            }
        }



    }
    protected void Searching()
    {
        agent.speed = speed;

        if (!validNewPosition)
        {
            ChooseNewDestination();
        }

        if (!agent.pathPending && agent.remainingDistance < 1f)
        {
            validNewPosition = false;
        }
    }

    private void LookForPlayer()
    {
        foreach (GameObject playerObj in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (playerObj == null) continue;

            Vector3 directionToPlayer = (playerObj.transform.position - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, playerObj.transform.position);

            if (distance <= visionRange)
            {
                Vector3 origin = transform.position + Vector3.up;
                if (!Physics.Raycast(origin, directionToPlayer, out RaycastHit hit, distance, layerMaskSeenByPlayerCheck)
                    || hit.transform == playerObj.transform)
                {
                    targetPlayer = playerObj.transform;
                    action = state.chasing;
                    break;
                }
            }
        }
    }

    protected void Chasing()
    {
        if (!targetPlayer) return;

        agent.speed = speed;
        agent.SetDestination(targetPlayer.position);

        float dist = Vector3.Distance(transform.position, targetPlayer.position);
        if (dist > visionRange * 1.5f)
        {
            action = state.searching;
            validNewPosition = false;
        }
    }

    protected void Jumping()
    {
        agent.speed = speed * jumpMultiplier;

    }

    protected void ChooseNewDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            validNewPosition = true;
        }

        timeUntilNextAction = Random.Range(0, MaxTimeUntilNextAction);
    }
    private bool IsSeenByPlayer()
    {
        foreach (var player in playerList)
        {
            if (player == null) continue;

            Camera playerCam = player.transform.Find("Camera").GetComponent<Camera>();
            if (playerCam == null) continue;

            Vector3 viewportPoint = playerCam.WorldToViewportPoint(transform.position);

            bool inFront = viewportPoint.z > 0;
            bool inViewHorizontally = viewportPoint.x >= 0f && viewportPoint.x <= 1f;
            bool inViewVertically = viewportPoint.y >= 0f && viewportPoint.y <= 1f;

            if (inFront && inViewHorizontally && inViewVertically)
            {
                Vector3 origin = playerCam.transform.position;
                Vector3 direction = (transform.position - origin).normalized;
                float distance = Vector3.Distance(origin, transform.position);

                if (!Physics.Raycast(origin, direction, out RaycastHit hit, distance, layerMaskSeenByPlayerCheck) || hit.transform == transform)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
