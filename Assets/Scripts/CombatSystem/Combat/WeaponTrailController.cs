using UnityEngine;

public class WeaponTrailController : MonoBehaviour
{
    [SerializeField] private TrailRenderer trailRenderer;

    private void Awake()
    {
        if (trailRenderer != null)
        {
            trailRenderer.emitting = false;
        }
    }

    public void AE_EnableTrail()
    {
        if (trailRenderer != null)
        {
            trailRenderer.Clear();
            trailRenderer.emitting = true;
        }
    }

    public void AE_DisableTrail()
    {
        if (trailRenderer != null)
        {
            trailRenderer.emitting = false;
        }
    }
}
