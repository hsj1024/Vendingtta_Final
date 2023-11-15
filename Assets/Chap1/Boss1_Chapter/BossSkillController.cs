using UnityEngine;
using System.Collections;
using static GhostAttack;
//using Skills; // Skill Ŭ������ ��ġ�� ���ӽ����̽�


public class BossSkillController : MonoBehaviour
{
    private GhostAttack ghostAttack;
    private BossFlowerThrow bossFlowerThrow;
    private FakeAttackSkill fakeAttackSkill;
    private BossController bossController;

    private void Start()
    {
        bossController = GetComponent<BossController>();
    }

    public void ActivateRandomSkill(BossSkillType skillType)
    {
        switch (skillType)
        {
            case BossSkillType.GhostAttack:
                ghostAttack.ActivateRandomSkill();
                break;
            case BossSkillType.BossFlowerThrow:
                bossFlowerThrow.ActivateRandomSkill();
                break;
            case BossSkillType.FakeAttackSkill:
                fakeAttackSkill.ActivateRandomSkill();
                break;
        }
    }

    private void ExecuteSkill(Skill skill)
    {
        skill.Activate(); // ��ų ����
    }
}
