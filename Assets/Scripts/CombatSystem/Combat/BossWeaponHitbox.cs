using UnityEngine;

public class BossWeaponHitbox : MonoBehaviour
{
    [SerializeField] private Collider weaponCollider;
    [SerializeField] private LayerMask playerLayer;

    private float currentDamage = 20f;
    private float currentPostureDamage = 10f;
    private bool isUnparryable = false;

    private void Awake()
    {
        if (weaponCollider != null) weaponCollider.enabled = false;
    }

    public void AE_EnableWeaponHitbox()
    {
        if (weaponCollider != null) weaponCollider.enabled = true;
    }

    public void AE_DisableWeaponHitbox()
    {
        if (weaponCollider != null) weaponCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            AttackDetails details = new AttackDetails
            {
                damage = currentDamage,
                postureDamage = currentPostureDamage,
                attackerPosition = transform.position,
                attackerTransform = transform.root,
                isUnparryable = isUnparryable
            };

            var playerHit = other.GetComponent<PlayerHealthAndDamage>();
            if (playerHit != null)
            {
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                playerHit.TakeAttack(details, hitPoint, -transform.forward);
            }

            weaponCollider.enabled = false;
        }
    }
}
