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

    private float nextSkillTime = 3f; // 다음 스킬 활성화 시간
    private float skillInterval = 5f; // 스킬 활성화 간격

    private BossSkillController bossSkillController;

    // 보스 체력 관리를 위한 변수 추가
    private int bossHealth;

    private void Start()
    {

        // 체력 초기화 및 이벤트 구독
        bossHealth = 100;
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
            // 체력이 50 이하일 때 강화된 스킬 활성화, 아니면 랜덤 스킬 활성화
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
        // 현재 활성화된 보스의 스킬을 랜덤하게 활성화
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
        fakeAttackSkill.OnSkillCompleted += OnThirdBossSkillCompleted;
    }

    private void OnThirdBossSkillCompleted()
    {
        // 세 번째 보스 비활성화
        boss3.SetActive(false);

        // 게임 클리어 로직 실행 등...
    }

    private bool ShouldActivateSkill()
    {
        // 현재 시간이 다음 스킬 시간을 넘었는지 확인
        if (Time.time >= nextSkillTime)
        {
            nextSkillTime = Time.time + skillInterval;
            return true;
        }
        return false;
    }


}
