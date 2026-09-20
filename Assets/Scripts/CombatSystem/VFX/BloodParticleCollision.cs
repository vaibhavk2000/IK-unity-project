using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodParticleCollision : MonoBehaviour
{
    [SerializeField] private ParticleSystem bloodParticleSystem;
    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

    private void OnParticleCollision(GameObject other)
    {
        if (bloodParticleSystem == null) return;

        int numCollisionEvents = bloodParticleSystem.GetCollisionEvents(other, collisionEvents);

        for (int i = 0; i < numCollisionEvents; i++)
        {
            Vector3 collisionPoint = collisionEvents[i].intersection;
            Vector3 collisionNormal = collisionEvents[i].normal;

            if (BloodVFXManager.Instance != null)
            {
                BloodVFXManager.Instance.SpawnGroundDecal(collisionPoint, collisionNormal);
            }
        }
    }
}
