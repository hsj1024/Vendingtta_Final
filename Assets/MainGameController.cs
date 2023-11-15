using UnityEngine;

public class MainGameController : MonoBehaviour
{
    public GameObject boss1;
    public GameObject boss2;
    public GameObject boss3;

    private GhostAttack ghostAttack;
    private BossFlowerThrow bossFlowerThrow;
    private FakeAttackSkill fakeAttackSkill;

    private float nextSkillTime = 7f; // ���� ��ų Ȱ��ȭ �ð�
    private float skillInterval = 5f; // ��ų Ȱ��ȭ ����

    private BossController bossController;

    private void Start()
    {
        bossController = BossController.Instance; // BossController �ν��Ͻ��� �����ɴϴ�.

        ghostAttack = boss1.GetComponent<GhostAttack>();
        bossFlowerThrow = boss2.GetComponent<BossFlowerThrow>();
        fakeAttackSkill = boss3.GetComponent<FakeAttackSkill>();

        // ù ��° ������ ��ų �Ϸ� �̺�Ʈ ����
        ghostAttack.OnSkillCompleted += OnFirstBossSkillCompleted;

        // �� ��° ������ ������ �� ��Ȱ��ȭ
        boss2.SetActive(false);

        // �� ��° ������ ������ �� ��Ȱ��ȭ
        boss3.SetActive(false);
    }

    private void Update()
    {
        if (ShouldActivateSkill())
        {
            // BossController�� ü���� Ȯ���Ͽ� ��ų Ȱ��ȭ
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
        // boss3�� FakeAttackSkill�� �������� ó���մϴ�.
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
        // boss3�� FakeAttackSkill�� �������� ó���մϴ�.
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
        // ���� Ŭ���� ���� ���� ��...
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
