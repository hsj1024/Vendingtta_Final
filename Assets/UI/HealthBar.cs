using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image batteryOutline; // 배터리 외각
    public Image[] greenSegments;  // 초록색 세그먼트 5개
    public Image[] yellowSegments; // 노란색 세그먼트 2개
    public Image[] redSegments;    // 빨간색 세그먼트 1개

    private int maxHealth = 100;  // 최대 체력은 100
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateBatteryDisplay();
    }

    // 체력 감소 메서드
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            PlayerDeath();
        }
        UpdateBatteryDisplay();
    }

    // 체력 회복 메서드
    public void RecoverHealth(int recover)
    {
        currentHealth += recover;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        UpdateBatteryDisplay();
    }

    private void UpdateBatteryDisplay()
    {
        // 모든 세그먼트를 먼저 비활성화
        foreach (var segment in greenSegments) segment.enabled = false;
        foreach (var segment in yellowSegments) segment.enabled = false;
        redSegments[0].enabled = false;

        // 체력에 따라 세그먼트를 활성화
        if (currentHealth > 80)
        {
            for (int i = 0; i < 5; i++) greenSegments[i].enabled = true;
        }
        else if (currentHealth > 60)
        {
            for (int i = 0; i < 4; i++) greenSegments[i].enabled = true;
        }
        else if (currentHealth > 40)
        {
            for (int i = 0; i < 3; i++) greenSegments[i].enabled = true;
        }
        else if (currentHealth > 20)
        {
            for (int i = 0; i < 2; i++) yellowSegments[i].enabled = true;
        }
        else if (currentHealth > 0)
        {
            redSegments[0].enabled = true;
        }
    }


    private void PlayerDeath()
    {
        // 플레이어 사망 처리 (배터리 외각만 남겨두고 나머지는 모두 비활성화)
        foreach (var segment in greenSegments) segment.enabled = false;
        foreach (var segment in yellowSegments) segment.enabled = false;
        foreach (var segment in redSegments) segment.enabled = false;

        // 추가적으로 플레이어 캐릭터를 비활성화하거나 다른 사망 관련 처리를 여기에 추가 가능.
    }
}
