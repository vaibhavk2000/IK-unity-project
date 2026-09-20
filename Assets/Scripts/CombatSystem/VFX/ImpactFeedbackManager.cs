using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class ImpactFeedbackManager : MonoBehaviour
{
    public static ImpactFeedbackManager Instance { get; private set; }

    [Header("Cinemachine Impulse Reference")]
    [SerializeField] private CinemachineImpulseSource impulseSource;

    private bool isHitstopping = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void TriggerImpact(float hitstopDuration, float shakeForce)
    {
        GenerateCameraShake(shakeForce);

        if (hitstopDuration > 0f && !isHitstopping)
        {
            StartCoroutine(ExecuteHitstop(hitstopDuration));
        }
    }

    public void GenerateCameraShake(float forceMultiplier = 1.0f)
    {
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulseWithForce(forceMultiplier);
        }
    }

    private IEnumerator ExecuteHitstop(float duration)
    {
        isHitstopping = true;
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1.0f;
        isHitstopping = false;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1.0f;
    }
}
