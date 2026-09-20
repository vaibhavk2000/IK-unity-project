using UnityEngine;

public class BossArenaTrigger : MonoBehaviour
{
    [SerializeField] private string bossDisplayName = "VALEN'S BANE";
    [SerializeField] private BossHealthUIBridge bossUIBridge;
    [SerializeField] private GameObject arenaFogWall;

    private bool isTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered && other.CompareTag("Player"))
        {
            isTriggered = true;

            if (arenaFogWall != null) arenaFogWall.SetActive(true);

            if (bossUIBridge != null)
            {
                bossUIBridge.ShowBossUI(bossDisplayName);
            }
        }
    }
}
