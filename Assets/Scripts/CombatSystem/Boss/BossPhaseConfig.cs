using UnityEngine;

[System.Serializable]
public class BossPhase
{
    public string phaseName = "Phase 1";
    [Range(0f, 1f)] public float healthThresholdNormalized = 1.0f;
    public string transitionAnimationTrigger = "Phase2Transition";
    public float transitionInvulnerabilityDuration = 3.5f;
    public GameObject phaseVFX;
    public SoundEffectSO phaseTransitionSFX;
}

[CreateAssetMenu(fileName = "NewBossAttack", menuName = "Boss AI/Boss Attack Configuration")]
public class BossAttackSO : ScriptableObject
{
    public string attackName = "Overhead Slam";
    public string animationTrigger = "Attack_OverheadSlam";

    public float minDistance = 0f;
    public float maxDistance = 4f;
    [Range(0f, 360f)] public float maxAngleFromForward = 45f;

    public float telegraphDuration = 0.5f;
    public float recoveryDuration = 1.2f;
    public float cooldown = 5f;

    public float damage = 40f;
    public float postureDamage = 25f;
    public bool isUnparryable = false;

    public int minimumPhase = 1;

    [System.NonSerialized] public float lastExecutionTime = -999f;

    public bool IsReady(float currentDistance, float currentAngle, int currentPhase)
    {
        if (currentPhase < minimumPhase) return false;
        if (Time.time < lastExecutionTime + cooldown) return false;
        if (currentDistance < minDistance || currentDistance > maxDistance) return false;
        if (currentAngle > maxAngleFromForward) return false;

        return true;
    }
}
