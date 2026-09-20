using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthUIBridge : MonoBehaviour
{
    [Header("UI Component References")]
    [SerializeField] private GameObject bossUIPanel;
    [SerializeField] private Image primaryHealthBar;
    [SerializeField] private Image damageCatchupBar;
    [SerializeField] private TextMeshProUGUI bossNameText;
    [SerializeField] private TextMeshProUGUI bossPhaseText;

    [Header("Animation Settings")]
    [SerializeField] private float catchupDelay = 0.5f;
    [SerializeField] private float catchupSpeed = 2f;

    private float targetFillAmount = 1f;
    private Coroutine catchupCoroutine;

    private void OnEnable()
    {
        BossHealthAndPhaseManager.OnBossHealthChanged += UpdateHealthUI;
        BossHealthAndPhaseManager.OnBossPhaseChanged += UpdatePhaseUI;
        BossHealthAndPhaseManager.OnBossDefeated += HideBossUI;
    }

    private void OnDisable()
    {
        BossHealthAndPhaseManager.OnBossHealthChanged -= UpdateHealthUI;
        BossHealthAndPhaseManager.OnBossPhaseChanged -= UpdatePhaseUI;
        BossHealthAndPhaseManager.OnBossDefeated -= HideBossUI;
    }

    public void ShowBossUI(string bossName)
    {
        if (bossUIPanel != null) bossUIPanel.SetActive(true);
        if (bossNameText != null) bossNameText.text = bossName;
    }

    private void UpdateHealthUI(float currentHP, float maxHP)
    {
        targetFillAmount = Mathf.Clamp01(currentHP / maxHP);

        if (primaryHealthBar != null)
        {
            primaryHealthBar.fillAmount = targetFillAmount;
        }

        if (catchupCoroutine != null) StopCoroutine(catchupCoroutine);
        catchupCoroutine = StartCoroutine(AnimateDamageCatchupBar());
    }

    private IEnumerator AnimateDamageCatchupBar()
    {
        yield return new WaitForSeconds(catchupDelay);

        while (damageCatchupBar != null && Mathf.Abs(damageCatchupBar.fillAmount - targetFillAmount) > 0.001f)
        {
            damageCatchupBar.fillAmount = Mathf.Lerp(damageCatchupBar.fillAmount, targetFillAmount, catchupSpeed * Time.deltaTime);
            yield return null;
        }

        if (damageCatchupBar != null) damageCatchupBar.fillAmount = targetFillAmount;
    }

    private void UpdatePhaseUI(string phaseName)
    {
        if (bossPhaseText != null)
        {
            bossPhaseText.text = phaseName;
        }
    }

    private void HideBossUI()
    {
        if (bossUIPanel != null)
        {
            bossUIPanel.SetActive(false);
        }
    }
}
