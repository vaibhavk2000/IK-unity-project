using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum BossAIState { Idle, Chasing, Telegraphing, Attacking, Recovering, Transitioning }

public class BossAIController : MonoBehaviour
{
    [Header("Target & Movement")]
    [SerializeField] private Transform playerTarget;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private BossHealthAndPhaseManager phaseManager;

    [Header("Attack Configurations")]
    [SerializeField] private List<BossAttackSO> availableAttacks;
    [SerializeField] private float combatLoopInterval = 0.2f;

    private BossAIState currentState = BossAIState.Idle;
    private BossAttackSO currentAttack;
    private int currentPhase = 1;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    private void Start()
    {
        if (playerTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTarget = player.transform;
        }

        StartCoroutine(BehaviorTreeLoop());
    }

    private void Update()
    {
        if (agent != null && animator != null)
        {
            animator.SetFloat(SpeedHash, agent.velocity.magnitude / agent.speed);
        }

        if (currentState == BossAIState.Telegraphing || currentState == BossAIState.Attacking)
        {
            RotateTowardsPlayer();
        }
    }

    private IEnumerator BehaviorTreeLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(combatLoopInterval);

            if (phaseManager != null && (phaseManager.IsTransitioning() || phaseManager.IsInvulnerable()))
            {
                if (agent.isActiveAndEnabled) agent.isStopped = true;
                continue;
            }

            if (currentState == BossAIState.Idle || currentState == BossAIState.Chasing)
            {
                EvaluateNextAction();
            }
        }
    }

    private void EvaluateNextAction()
    {
        if (playerTarget == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
        Vector3 dirToPlayer = (playerTarget.position - transform.position).normalized;
        dirToPlayer.y = 0;
        float angleToPlayer = Vector3.Angle(transform.forward, dirToPlayer);

        BossAttackSO selectedAttack = SelectBestAttack(distanceToPlayer, angleToPlayer);

        if (selectedAttack != null)
        {
            StartCoroutine(ExecuteAttackSequence(selectedAttack));
        }
        else
        {
            currentState = BossAIState.Chasing;
            if (agent.isActiveAndEnabled)
            {
                agent.isStopped = false;
                agent.SetDestination(playerTarget.position);
            }
        }
    }

    private BossAttackSO SelectBestAttack(float distance, float angle)
    {
        List<BossAttackSO> validAttacks = new List<BossAttackSO>();

        foreach (var attack in availableAttacks)
        {
            if (attack.IsReady(distance, angle, currentPhase))
            {
                validAttacks.Add(attack);
            }
        }

        if (validAttacks.Count == 0) return null;

        return validAttacks[Random.Range(0, validAttacks.Count)];
    }

    private IEnumerator ExecuteAttackSequence(BossAttackSO attack)
    {
        currentAttack = attack;
        currentState = BossAIState.Telegraphing;

        if (agent.isActiveAndEnabled) agent.isStopped = true;

        if (animator != null) animator.SetTrigger(attack.animationTrigger);
        attack.lastExecutionTime = Time.time;

        yield return new WaitForSeconds(attack.telegraphDuration);

        currentState = BossAIState.Attacking;
        yield return new WaitForSeconds(0.8f);

        currentState = BossAIState.Recovering;
        yield return new WaitForSeconds(attack.recoveryDuration);

        currentState = BossAIState.Idle;
        currentAttack = null;
    }

    private void RotateTowardsPlayer()
    {
        if (playerTarget == null) return;

        Vector3 direction = (playerTarget.position - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 8f * Time.deltaTime);
        }
    }

    public void SetPhase(int newPhase)
    {
        currentPhase = newPhase;
    }
}
