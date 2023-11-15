using System.Collections.Generic;
using UnityEngine;
using System;


public class MainGameController : MonoBehaviour
{
    public GameObject boss1;
    public GameObject boss2;
    public GameObject boss3;

    private GhostAttack ghostAttack;
    private BossFlowerThrow bossFlowerThrow;
    private FakeAttackSkill fakeAttackSkill;

    private float nextSkillTime = 3f; // ���� ��ų Ȱ��ȭ �ð�
    private float skillInterval = 5f; // ��ų Ȱ��ȭ ����

    private BossSkillController bossSkillController;

    // ���� ü�� ������ ���� ���� �߰�
    private int bossHealth;

    private void Start()
    {

        // ü�� �ʱ�ȭ �� �̺�Ʈ ����
        bossHealth = 100;
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
            // ü���� 50 ������ �� ��ȭ�� ��ų Ȱ��ȭ, �ƴϸ� ���� ��ų Ȱ��ȭ
            if (bossHealth <= 50)
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
        // ���� Ȱ��ȭ�� ������ ��ų�� �����ϰ� Ȱ��ȭ
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
        fakeAttackSkill.OnSkillCompleted += OnThirdBossSkillCompleted;
    }

    private void OnThirdBossSkillCompleted()
    {
        // �� ��° ���� ��Ȱ��ȭ
        boss3.SetActive(false);

        // ���� Ŭ���� ���� ���� ��...
    }

    private bool ShouldActivateSkill()
    {
        // ���� �ð��� ���� ��ų �ð��� �Ѿ����� Ȯ��
        if (Time.time >= nextSkillTime)
        {
            nextSkillTime = Time.time + skillInterval;
            return true;
        }
        return false;
    }


}
