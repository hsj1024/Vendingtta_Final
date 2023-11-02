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
            Debug.Log("모든 보스를 이겼습니다!");
        }
    }

    private void BossDefeated()
    {
        Debug.Log("보스를 이겼습니다!");
        ActivateNextBoss();
    }
}
