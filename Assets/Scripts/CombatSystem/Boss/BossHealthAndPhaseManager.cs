using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealthAndPhaseManager : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 1000f;
    [SerializeField] private float currentHealth;

    [Header("Phase Configurations")]
    [SerializeField] private List<BossPhase> bossPhases;
    private int currentPhaseIndex = 0;

    [Header("References")]
    [SerializeField] private Animator animator;

    private bool isInvulnerable = false;
    private bool isTransitioning = false;

    public static event Action<float, float> OnBossHealthChanged;
    public static event Action<string> OnBossPhaseChanged;
    public static event Action OnBossDefeated;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        OnBossHealthChanged?.Invoke(currentHealth, maxHealth);
        if (bossPhases.Count > 0)
        {
            OnBossPhaseChanged?.Invoke(bossPhases[0].phaseName);
        }
    }

    public void TakeDamage(float amount)
    {
        if (isInvulnerable || isTransitioning || currentHealth <= 0) return;

        currentHealth = Mathf.Max(0f, currentHealth - amount);
        OnBossHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            ExecuteBossDeath();
            return;
        }

        CheckPhaseTransition();
    }

    private void CheckPhaseTransition()
    {
        int nextPhaseIndex = currentPhaseIndex + 1;

        if (nextPhaseIndex < bossPhases.Count)
        {
            float healthRatio = currentHealth / maxHealth;
            if (healthRatio <= bossPhases[nextPhaseIndex].healthThresholdNormalized)
            {
                StartCoroutine(ExecutePhaseTransition(bossPhases[nextPhaseIndex], nextPhaseIndex));
            }
        }
    }

    private IEnumerator ExecutePhaseTransition(BossPhase newPhase, int newPhaseIndex)
    {
        isTransitioning = true;
        isInvulnerable = true;
        currentPhaseIndex = newPhaseIndex;

        if (animator != null && !string.IsNullOrEmpty(newPhase.transitionAnimationTrigger))
        {
            animator.SetTrigger(newPhase.transitionAnimationTrigger);
        }

        if (newPhase.phaseTransitionSFX != null && CombatAudioManager.Instance != null)
        {
            CombatAudioManager.Instance.Play3DSFX(newPhase.phaseTransitionSFX, transform.position);
        }

        if (newPhase.phaseVFX != null)
        {
            Instantiate(newPhase.phaseVFX, transform.position, Quaternion.identity, transform);
        }

        OnBossPhaseChanged?.Invoke(newPhase.phaseName);

        yield return new WaitForSeconds(newPhase.transitionInvulnerabilityDuration);

        isInvulnerable = false;
        isTransitioning = false;
    }

    private void ExecuteBossDeath()
    {
        if (animator != null) animator.SetTrigger("Die");
        OnBossDefeated?.Invoke();
        this.enabled = false;
    }

    public bool IsInvulnerable() => isInvulnerable;
    public bool IsTransitioning() => isTransitioning;
}
