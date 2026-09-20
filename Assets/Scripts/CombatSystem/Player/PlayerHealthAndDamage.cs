using UnityEngine;

public struct AttackDetails
{
    public float damage;
    public float postureDamage;
    public Vector3 attackerPosition;
    public Transform attackerTransform;
    public bool isUnparryable;
}

public class PlayerHealthAndDamage : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private PlayerDodgeSystem dodgeSystem;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeAttack(AttackDetails details, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (dodgeSystem != null && dodgeSystem.IsInvincible())
        {
            Debug.Log("DODGED! Attack negated by I-Frames.");
            return;
        }

        currentHealth -= details.damage;
        Debug.Log($"Player took {details.damage} damage. Current HP: {currentHealth}");

        if (BloodVFXManager.Instance != null)
        {
            BloodVFXManager.Instance.SpawnDirectionalBlood(hitPoint, hitDirection);
        }
    }
}
