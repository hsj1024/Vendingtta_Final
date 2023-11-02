using System.Collections;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public GameObject[] bosses;
    private int currentBossIndex = 0;

    void Start()
    {
        ActivateNextBoss();
    }

    public void ActivateNextBoss()
    {
        if (currentBossIndex < bosses.Length)
        {
            bosses[currentBossIndex].SetActive(true);
            BossController bossController = bosses[currentBossIndex].GetComponent<BossController>();
            if (bossController != null)
            {
                bossController.onBossDefeated += BossDefeated;
            }
            currentBossIndex++;
        }
        else
        {
            Debug.Log("��� ������ �̰���ϴ�!");
        }
    }

    private void BossDefeated()
    {
        Debug.Log("������ �̰���ϴ�!");
        ActivateNextBoss();
    }
}
