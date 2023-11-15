using UnityEngine;

public class MainGameController : MonoBehaviour
{
    public GameObject boss1;
    public GameObject boss2;
    public GameObject boss3;

    private GhostAttack ghostAttack;
    private BossFlowerThrow bossFlowerThrow;
    private FakeAttackSkill fakeAttackSkill;

    private float nextSkillTime = 7f; // 다음 스킬 활성화 시간
    private float skillInterval = 5f; // 스킬 활성화 간격

    private BossController bossController;

    private void Start()
    {
        bossController = BossController.Instance; // BossController 인스턴스를 가져옵니다.

        ghostAttack = boss1.GetComponent<GhostAttack>();
        bossFlowerThrow = boss2.GetComponent<BossFlowerThrow>();
        fakeAttackSkill = boss3.GetComponent<FakeAttackSkill>();

        // 첫 번째 보스의 스킬 완료 이벤트 구독
        ghostAttack.OnSkillCompleted += OnFirstBossSkillCompleted;

        // 두 번째 보스는 시작할 때 비활성화
        boss2.SetActive(false);

        // 세 번째 보스는 시작할 때 비활성화
        boss3.SetActive(false);
    }

    private void Update()
    {
        if (ShouldActivateSkill())
        {
            // BossController의 체력을 확인하여 스킬 활성화
            if (bossController.currentBossHealth <= 50)
            {
                ActivateEnhancedSkill();
            }
            else
            {
                ActivateRandomSkill();
            }
        }
    }

    private void ActivateRandomSkill()
    {
        if (boss1.activeSelf)
        {
            ghostAttack.ActivateRandomSkill();
        }
        else if (boss2.activeSelf)
        {
            bossFlowerThrow.ActivateRandomSkill();
        }
        // boss3은 FakeAttackSkill로 마지막에 처리합니다.
    }

    private void ActivateEnhancedSkill()
    {
        if (boss1.activeSelf)
        {
            ghostAttack.ActivateEnhancedSkill();
        }
        else if (boss2.activeSelf)
        {
            bossFlowerThrow.ActivateEnhancedSkill();
        }
        // boss3은 FakeAttackSkill로 마지막에 처리합니다.
    }

    private void OnFirstBossSkillCompleted()
    {
        boss1.SetActive(false);
        boss2.SetActive(true);
        bossFlowerThrow.OnSkillCompleted += OnSecondBossSkillCompleted;
    }

    private void OnSecondBossSkillCompleted()
    {
        boss2.SetActive(false);
        boss3.SetActive(true);
        fakeAttackSkill.OnSkillCompleted += OnThirdBossSkillCompleted;
    }

    private void OnThirdBossSkillCompleted()
    {
        boss3.SetActive(false);
        // 게임 클리어 로직 실행 등...
    }

    private bool ShouldActivateSkill()
    {
        if (Time.time >= nextSkillTime)
        {
            nextSkillTime = Time.time + skillInterval;
            return true;
        }
        return false;
    }
}
