using System.Collections.Generic;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Kidnapper : EnemyBase
{
    [SerializeField] List<PlayerState> playersGettingCarried;
    [SerializeField] private Transform carryPosition;
    [SerializeField] private Transform dropPosition;
    [SerializeField] private float attackChance = 0.05f;
    protected override void Update()
    {
        if (!IsServer) return;

        //Counts the cooldowns down
        timeUntilNextAction -= Time.deltaTime;
        timeUntilNextAttack -= Time.deltaTime;
        timeUntilStunOver -= Time.deltaTime;

        if (isAttacking)
        {
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                isAttacking = false;
                currentState = EnemyState.Idle;
            }
            return;
        }

        if (timeUntilStunOver >= 0) return; //checks if it is able to act again after an attack

        var (playerSeen, player) = CanSeePlayer(playerSeenThisFrame);

        //Check if Enemy sees a player

        if (playerSeen && timeUntilNextAttack <= 0 && Random.Range(0f, 1f) <= attackChance)
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
            agent.speed = walkingSpeed;

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

    protected override void Attack(List<PlayerState> Targets)
    {


        if (Targets.Count == 0)
        {
            isAttacking = false;
            return;
        }
        playersGettingCarried = Targets;
        validNewPosition = false;
        ChooseNewDestination(null, 130f, true);
        agent.speed = sprintSpeed;
        DisablePlayerControlsServerRpc();
        StartCoroutine(CarryPlayers());
    }

    protected IEnumerator CarryPlayers()
    {
        while (isAttacking)
        {
            updatePlayerPositionsServerRpc(carryPosition.position);
            yield return null;
        }
        updatePlayerPositionsServerRpc(dropPosition.position);
        EnablePlayerControlsServerRpc();
        timeUntilNextAttack = attackCoooldown;


    }

    [ServerRpc]
    protected void DisablePlayerControlsServerRpc()
    {
        foreach (var player in playersGettingCarried)
        {
            player.DisableClientControlsAndGravityClientRpc();
        }
    }
    [ServerRpc]
    protected void EnablePlayerControlsServerRpc()
    {
        foreach (var player in playersGettingCarried)
        {
            player.EnableClientControlsAndGravityClientRpc();
        }
    }
    [ServerRpc]
    protected void updatePlayerPositionsServerRpc(Vector3 target)
    {
        foreach (var player in playersGettingCarried)
        {
            player.SetPlayerPositionClientRpc(target);
        }
    }
}
