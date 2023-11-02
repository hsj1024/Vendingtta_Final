using UnityEngine;

public class MainGameController : MonoBehaviour
{
    public GameObject boss1;
    public GameObject boss2;
    public GameObject boss3;

    private GhostAttack ghostAttack;
    private BossFlowerThrow bossFlowerThrow;
    private FakeAttackSkill boss3Skill;

    private void Start()
    {
        ghostAttack = boss1.GetComponent<GhostAttack>();
        bossFlowerThrow = boss2.GetComponent<BossFlowerThrow>();
        boss3Skill = boss3.GetComponent<FakeAttackSkill>();

        // 첫 번째 보스의 스킬 완료 이벤트 구독
        ghostAttack.OnSkillCompleted += OnFirstBossSkillCompleted;

        // 두 번째 보스는 시작할 때 비활성화
        boss2.SetActive(false);

        // 세 번째 보스는 시작할 때 비활성화
        boss3.SetActive(false);
    }

    private void OnFirstBossSkillCompleted()
    {
        // 첫 번째 보스 비활성화
        boss1.SetActive(false);

        // 두 번째 보스 활성화
        boss2.SetActive(true);

        // 두 번째 보스의 스킬 완료 이벤트 구독
        bossFlowerThrow.OnSkillCompleted += OnSecondBossSkillCompleted;
    }

    private void OnSecondBossSkillCompleted()
    {
        // 두 번째 보스 비활성화
        boss2.SetActive(false);

        // 세 번째 보스 활성화
        boss3.SetActive(true);

        // 세 번째 보스의 스킬 완료 이벤트 구독
        boss3Skill.OnSkillCompleted += OnThirdBossSkillCompleted;
    }

    private void OnThirdBossSkillCompleted()
    {
        // 세 번째 보스 비활성화
        boss3.SetActive(false);

        // 게임 클리어 로직 실행 등...
    }
}
