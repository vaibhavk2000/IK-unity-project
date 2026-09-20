using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    [Header("Impact Feedback Tuning")]
    [SerializeField] private float lightHitstopDuration = 0.05f;
    [SerializeField] private float heavyHitstopDuration = 0.12f;
    [SerializeField] private float lightShakeForce = 0.8f;
    [SerializeField] private float heavyShakeForce = 2.5f;

    [Header("Audio Configurations")]
    [SerializeField] private SoundEffectSO swingSFX;
    [SerializeField] private SoundEffectSO fleshImpactSFX;
    [SerializeField] private SoundEffectSO armorClashSFX;

    [SerializeField] private LayerMask parryableLayers;
    public bool isHeavyAttack = false;

    public void AE_PlaySwingSFX()
    {
        if (CombatAudioManager.Instance != null && swingSFX != null)
        {
            CombatAudioManager.Instance.Play3DSFX(swingSFX, transform.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Vector3 hitPoint = other.ClosestPoint(transform.position);
        Vector3 hitNormal = (transform.position - hitPoint).normalized;

        if (other.CompareTag("Enemy"))
        {
            float duration = isHeavyAttack ? heavyHitstopDuration : lightHitstopDuration;
            float force = isHeavyAttack ? heavyShakeForce : lightShakeForce;

            if (ImpactFeedbackManager.Instance != null)
            {
                ImpactFeedbackManager.Instance.TriggerImpact(duration, force);
            }

            if (CombatAudioManager.Instance != null && fleshImpactSFX != null)
            {
                CombatAudioManager.Instance.Play3DSFX(fleshImpactSFX, hitPoint);
            }

            var boss = other.GetComponent<BossHealthAndPhaseManager>();
            if (boss != null)
            {
                boss.TakeDamage(25f);
            }
        }

        if (((1 << other.gameObject.layer) & parryableLayers) != 0)
        {
            if (CombatVFXManager.Instance != null)
            {
                CombatVFXManager.Instance.SpawnClashSparks(hitPoint, hitNormal);
            }

            if (CombatAudioManager.Instance != null && armorClashSFX != null)
            {
                CombatAudioManager.Instance.Play3DSFX(armorClashSFX, hitPoint);
            }
        }
    }
}
