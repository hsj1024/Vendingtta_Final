using UnityEngine;

public class BossController : MonoBehaviour
{
    public delegate void BossDefeatedDelegate();
    public event BossDefeatedDelegate onBossDefeated;

    public void DefeatBoss()
    {
        // 이 함수는 보스를 패배시키는 함수입니다.
        // 실제 게임에서는 보스의 체력이 0이 되었을 때 이 함수를 호출할 수 있습니다.
        onBossDefeated?.Invoke();
        gameObject.SetActive(false);
    }
}
