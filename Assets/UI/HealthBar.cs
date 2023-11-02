using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image batteryOutline; // ���͸� �ܰ�
    public Image[] greenSegments;  // �ʷϻ� ���׸�Ʈ 5��
    public Image[] yellowSegments; // ����� ���׸�Ʈ 2��
    public Image[] redSegments;    // ������ ���׸�Ʈ 1��

    private int maxHealth = 100;  // �ִ� ü���� 100
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateBatteryDisplay();
    }

    // ü�� ���� �޼���
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

    // ü�� ȸ�� �޼���
    public void RecoverHealth(int recover)
    {
        currentHealth += recover;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        UpdateBatteryDisplay();
    }

    private void UpdateBatteryDisplay()
    {
        // ��� ���׸�Ʈ�� ���� ��Ȱ��ȭ
        foreach (var segment in greenSegments) segment.enabled = false;
        foreach (var segment in yellowSegments) segment.enabled = false;
        redSegments[0].enabled = false;

        // ü�¿� ���� ���׸�Ʈ�� Ȱ��ȭ
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
        // �÷��̾� ��� ó�� (���͸� �ܰ��� ���ܵΰ� �������� ��� ��Ȱ��ȭ)
        foreach (var segment in greenSegments) segment.enabled = false;
        foreach (var segment in yellowSegments) segment.enabled = false;
        foreach (var segment in redSegments) segment.enabled = false;

        // �߰������� �÷��̾� ĳ���͸� ��Ȱ��ȭ�ϰų� �ٸ� ��� ���� ó���� ���⿡ �߰� ����.
    }
}
