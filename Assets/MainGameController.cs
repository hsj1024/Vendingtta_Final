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

        // ù ��° ������ ��ų �Ϸ� �̺�Ʈ ����
        ghostAttack.OnSkillCompleted += OnFirstBossSkillCompleted;

        // �� ��° ������ ������ �� ��Ȱ��ȭ
        boss2.SetActive(false);

        // �� ��° ������ ������ �� ��Ȱ��ȭ
        boss3.SetActive(false);
    }

    private void OnFirstBossSkillCompleted()
    {
        // ù ��° ���� ��Ȱ��ȭ
        boss1.SetActive(false);

        // �� ��° ���� Ȱ��ȭ
        boss2.SetActive(true);

        // �� ��° ������ ��ų �Ϸ� �̺�Ʈ ����
        bossFlowerThrow.OnSkillCompleted += OnSecondBossSkillCompleted;
    }

    private void OnSecondBossSkillCompleted()
    {
        // �� ��° ���� ��Ȱ��ȭ
        boss2.SetActive(false);

        // �� ��° ���� Ȱ��ȭ
        boss3.SetActive(true);

        // �� ��° ������ ��ų �Ϸ� �̺�Ʈ ����
        boss3Skill.OnSkillCompleted += OnThirdBossSkillCompleted;
    }

    private void OnThirdBossSkillCompleted()
    {
        // �� ��° ���� ��Ȱ��ȭ
        boss3.SetActive(false);

        // ���� Ŭ���� ���� ���� ��...
    }
}
