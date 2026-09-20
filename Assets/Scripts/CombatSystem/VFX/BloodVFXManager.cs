using UnityEngine;

public class BloodVFXManager : MonoBehaviour
{
    public static BloodVFXManager Instance { get; private set; }

    [Header("Prefabs")]
    [SerializeField] private GameObject bloodSplatterPrefab;
    [SerializeField] private GameObject sparkClashPrefab;
    [SerializeField] private GameObject[] groundDecalPrefabs;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SpawnDirectionalBlood(Vector3 hitPoint, Vector3 hitDirection)
    {
        if (bloodSplatterPrefab == null) return;

        Quaternion bloodRotation = Quaternion.LookRotation(hitDirection);
        GameObject bloodInstance = Instantiate(bloodSplatterPrefab, hitPoint, bloodRotation);
        Destroy(bloodInstance, 2.5f);
    }

    public void SpawnClashSparks(Vector3 contactPoint, Vector3 contactNormal)
    {
        if (sparkClashPrefab == null) return;

        Quaternion sparkRotation = Quaternion.LookRotation(contactNormal);
        GameObject sparkInstance = Instantiate(sparkClashPrefab, contactPoint, sparkRotation);
        Destroy(sparkInstance, 1.5f);
    }

    public void SpawnGroundDecal(Vector3 position, Vector3 normal)
    {
        if (groundDecalPrefabs == null || groundDecalPrefabs.Length == 0) return;

        GameObject chosenDecal = groundDecalPrefabs[Random.Range(0, groundDecalPrefabs.Length)];
        Quaternion decalRotation = Quaternion.LookRotation(-normal);
        decalRotation *= Quaternion.Euler(0, 0, Random.Range(0f, 360f));

        GameObject decalInstance = Instantiate(chosenDecal, position + (normal * 0.01f), decalRotation);
        Destroy(decalInstance, 30f);
    }
}

public class CombatVFXManager : BloodVFXManager {}
