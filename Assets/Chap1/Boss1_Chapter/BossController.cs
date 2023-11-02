using UnityEngine;

public class BossController : MonoBehaviour
{
    public delegate void BossDefeatedDelegate();
    public event BossDefeatedDelegate onBossDefeated;

    public void DefeatBoss()
    {
        // �� �Լ��� ������ �й��Ű�� �Լ��Դϴ�.
        // ���� ���ӿ����� ������ ü���� 0�� �Ǿ��� �� �� �Լ��� ȣ���� �� �ֽ��ϴ�.
        onBossDefeated?.Invoke();
        gameObject.SetActive(false);
    }
}
