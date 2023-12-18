using UnityEngine;
using System;

using System.Collections;

public class MainGameController : MonoBehaviour
{
    public GameObject boss1;
    public GameObject boss2;
    public GameObject boss3;
    public GameObject boss4; // FlowerRain 스킬을 가진 보스

    
    private GhostAttack ghostAttack;
    private BossFlowerThrow bossFlowerThrow;
    private FakeAttackSkill fakeAttackSkill;
    private FlowerRain flowerRain; // FlowerRain 스킬 스크립트 참조

    private float nextSkillTime = 7f; // 다음 스킬 활성화 시간
    private float skillInterval = 1f; // 스킬 활성화 간격

    private BossController bossController;
    public Transform leftPosition; // 왼쪽 위치
    public Transform rightPosition; // 오른쪽 위치

    private bool fakeAttackActivatedAt20 = false; // 체력 20 이하일 때 FakeAttackSkill 발동 여부
    private bool fakeAttackActivatedAt50 = false; // 체력 50 이상일 때 FakeAttackSkill 발동 여부
    private GameObject activeSkill;
    private bool isFakeAttackActive = false;
    public Camera mainCamera; // 메인 게임 카메라
    public Vector3 bossViewportPosition = new Vector3(0.5f, 0.5f, 10f); // Z 값은 카메라로부터 보스까지의 거리입니다.

    IEnumerator FadeIn(GameObject objectToFade, float duration)
    {
        SpriteRenderer spriteRenderer = objectToFade.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // 초기 투명도를 0으로 설정합니다.
            Color color = spriteRenderer.color;
            color.a = 0;
            spriteRenderer.color = color;

            // 페이드 인 동안의 시간을 추적합니다.
            float counter = 0;

            while (counter < duration)
            {
                counter += Time.deltaTime;
                // alpha 값을 점차 증가시켜 페이드 인 효과를 만듭니다.
                color.a = Mathf.Lerp(0, 1, counter / duration);
                spriteRenderer.color = color;

                // 다음 프레임까지 대기합니다.
                yield return null;
            }
        }
    }
    IEnumerator FadeOut(GameObject objectToFade, float duration, Action onComplete = null)
    {
        SpriteRenderer spriteRenderer = objectToFade.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            float counter = 0;
            Color startColor = spriteRenderer.color;

            while (counter < duration)
            {
                counter += Time.deltaTime;
                float alpha = Mathf.Lerp(1, 0, counter / duration);
                Color color = spriteRenderer.color;
                color.a = alpha;
                spriteRenderer.color = color;
                yield return null;
            }

            objectToFade.SetActive(false);
            onComplete?.Invoke(); // 콜백 호출
        }
    }




    // 보스를 카메라 뷰포트에 따라 동적 위치에 활성화하는 메서드
    // 보스를 카메라 뷰포트 내에서 랜덤한 위치에 활성화하는 메서드
    // 보스를 카메라 뷰포트 내에서 수평선상 랜덤한 위치에 활성화하는 메서드
    private void ActivateBossAtViewportPosition(GameObject boss)
    {
        // 뷰포트의 X 좌표만 랜덤하게 설정합니다.
        // 예를 들어, X는 0.1에서 0.9 사이로 설정하여 화면 좌우에 위치하도록 합니다.
        float randomX = UnityEngine.Random.Range(0.1f, 0.9f);
        Vector3 randomViewportPosition = new Vector3(randomX, bossViewportPosition.y, bossViewportPosition.z);

        // 랜덤 뷰포트 좌표를 월드 좌표로 변환합니다.
        Vector3 worldPosition = mainCamera.ViewportToWorldPoint(randomViewportPosition);

        // 보스를 랜덤 월드 좌표에 활성화합니다.
        boss.transform.position = worldPosition;
        boss.SetActive(true);

        // 페이드 인 코루틴을 시작합니다.
        StartCoroutine(FadeIn(boss, 1f)); // 1초 동안 페이드 인

        // BossFlowerThrow 클래스의 headTransform 위치를 갱신합니다.
        if (bossFlowerThrow != null)
        {
            bossFlowerThrow.UpdateHeadTransformPosition();
        }
    }


    private void Start()
    {
        bossController = BossController.Instance;


        // 모든 보스를 초기에 비활성화
        boss1.SetActive(false);
        boss2.SetActive(false);
        boss3.SetActive(false);
        boss4.SetActive(false);

        // 첫 번째 보스 (GhostAttack) 이벤트 구독
        ghostAttack = boss1.GetComponent<GhostAttack>();
        ghostAttack.OnNormalSkillCompleted += OnGhostAttackNormalSkillCompleted;
        //ghostAttack.OnEnhancedSkillCompleted += OnGhostAttackEnhancedSkillCompleted;

        // 두 번째 보스 (BossFlowerThrow) 이벤트 구독
        bossFlowerThrow = boss2.GetComponent<BossFlowerThrow>();
        bossFlowerThrow.OnNormalSkillCompleted += OnBossFlowerThrowNormalSkillCompleted;
        bossFlowerThrow.OnEnhancedSkillCompleted += OnBossFlowerThrowEnhancedSkillCompleted;

        // 세 번째 보스 (FakeAttackSkill) 이벤트 구독
        fakeAttackSkill = boss3.GetComponent<FakeAttackSkill>();
        fakeAttackSkill.OnNormalSkillCompleted += OnBossFakeAttackNormalSkillCompleted;

        // 게임 시작 시 첫 번째 스킬 활성화
        // 네 번째 보스 (FlowerRain) 이벤트 구독
        flowerRain = boss4.GetComponent<FlowerRain>();
        flowerRain.OnNormalSkillCompleted += OnFlowerRainNormalSkillCompleted; // 필요한 경우

        ActivateNextSkill();
    }


    private void OnGhostAttackNormalSkillCompleted()
    {
        boss1.SetActive(false); // Boss1 비활성화
        activeSkill = null;
        nextSkillTime = Time.time + skillInterval; // 다음 스킬 활성화 시간 설정
        ActivateNextSkill(); // FadeOut이 완료된 후 다음 스킬 활성화
        
    }


    /*private void OnGhostAttackEnhancedSkillCompleted()
    {
        boss1.SetActive(false); // Boss1 비활성화
        activeSkill = null;
        // 다음 스킬 활성화 로직 추가
        Debug.Log("다음스킬 활성화");
        ActivateNextSkill();
    }*/

    private void OnBossFlowerThrowNormalSkillCompleted()
    {

        StartCoroutine(FadeOut(boss2, 1f)); // 1초 동안 페이드 아웃 수행 후 비활성화

        //boss2.SetActive(false);
        activeSkill = null;
        // 다음 스킬 활성화 로직 추가
        nextSkillTime = Time.time + skillInterval; // 다음 스킬 활성화 시간 업데이트
        
        ActivateNextSkill();
    }

    private void OnBossFlowerThrowEnhancedSkillCompleted()
    {


        //boss2.SetActive(false);
        StartCoroutine(FadeOut(boss2, 1f)); // 1초 동안 페이드 아웃 수행 후 비활성화

        activeSkill = null;

        // 다음 스킬 활성화 로직 추가
        nextSkillTime = Time.time + skillInterval; // 다음 스킬 활성화 시간 업데이트
       
        ActivateNextSkill();
    }
    private void OnBossFakeAttackNormalSkillCompleted()
    {
        

        //boss3.SetActive(false);
        isFakeAttackActive = false; // FakeAttackSkill 완료

        // FakeAttackSkill이 다시 활성화될 수 있도록 플래그 재설정
        if (bossController.currentBossHealth == 50)
        {
            fakeAttackActivatedAt50 = false;
        }
        nextSkillTime = Time.time + skillInterval; // 다음 스킬 활성화 시간 업데이트
        StartCoroutine(FadeOut(boss3, 2f));
        ActivateNextSkill();
    }


    /*private void ActivateFakeAttackSkill()
    {
        boss3.SetActive(true);
        fakeAttackSkill.ActivateNormalSkill();
        fakeAttackActivatedAt50 = true;
        isFakeAttackActive = true; // FakeAttackSkill 활성화
    }*/


    private void OnFlowerRainNormalSkillCompleted()
    {


        //boss4.SetActive(false);
        StartCoroutine(FadeOut(boss4, 1f)); // 1초 동안 페이드 아웃 수행 후 비활성화

        activeSkill = null;
        
        // 다음 스킬 활성화 로직 추가
        nextSkillTime = Time.time + skillInterval; // 다음 스킬 활성화 시간 업데이트
        
        ActivateNextSkill();
    }
    

    private void Update()
    {
        if (ShouldActivateSkill() && activeSkill == null && !IsAnySkillInProgress())
        {
            ActivateNextSkill();
        }
    }
    private bool IsAnySkillInProgress()
    {
        // 각 스킬 스크립트의 진행 상태를 확인
        return ghostAttack.IsSkillInProgress || flowerRain.IsSkillInProgress || 
               bossFlowerThrow.IsSkillInProgress;
    }
    public void ActivateNextSkill()
    {
        // 현재 활성화된 스킬이 없고 FakeAttackSkill이 활성화되지 않았을 때만 새로운 스킬 활성화
        if (Time.time >= nextSkillTime && activeSkill == null && !isFakeAttackActive)
        {
            if (bossController.currentBossHealth > 50)
            {
                //Debug.Log("active next skill log");
                ActivateRandomNormalSkill();
            }
            else if (bossController.currentBossHealth == 50 && !fakeAttackActivatedAt50)
            {
                //boss3.SetActive(true); // 보스3 활성화
                ActivateBossAtViewportPosition(boss3);
                fakeAttackSkill.ActivateNormalSkill();
                fakeAttackActivatedAt50 = true;
                isFakeAttackActive = true; // FakeAttackSkill 활성화
            }
            else if (bossController.currentBossHealth == 20 && !fakeAttackActivatedAt20)
            {
                //boss3.SetActive(true); // 보스3 활성화
                ActivateBossAtViewportPosition(boss3);

                fakeAttackSkill.ActivateNormalSkill();
                fakeAttackActivatedAt20 = true;
                isFakeAttackActive = true; // FakeAttackSkill 활성화
            }
            else if (bossController.currentBossHealth < 50)
            {
                //Debug.Log("보스 체력 50 미만");
                ActivateRandomMixedSkill();
            }
            // 다음 스킬 활성화를 바로 호출하지 않고 스킬 간격을 더 줄입니다.
            nextSkillTime = Time.time + skillInterval;
        }
    }

    public void OnEnable()
    {
        Die.PlayerDiedEvent += StopGameActivities;
    }

    private void OnDisable()
    {
        Die.PlayerDiedEvent -= StopGameActivities;
    }

    private void StopGameActivities()
    {
        // 여기에 게임 활동 중지 로직 구현
        this.enabled = false;
    }
    private void ActivateRandomNormalSkill()
    {
        
        if (activeSkill == null && !isFakeAttackActive) // FakeAttackSkill이 활성화되지 않은 경우에만
        {
            float randomValue = UnityEngine.Random.value;
            if (randomValue < 0.3f) // 30% 확률로 FlowerRain
            {
                ActivateBossAtViewportPosition(boss4);
                flowerRain.ActivateNormalSkill();
                activeSkill = boss4;
            }
            else if (randomValue < 0.5f) // 추가 20% 확률로 BossFlowerThrow
            {
                //boss2.SetActive(true);
                ActivateBossAtViewportPosition(boss2);

                bossFlowerThrow.ActivateNormalSkill();
                activeSkill = boss2;
            }
            else if (randomValue < 0.8f) // 추가 30% 확률로 GhostAttack
            {
                //boss1.SetActive(true);
                ActivateBossAtViewportPosition(boss1);

                ghostAttack.ActivateNormalSkill();
                Debug.Log("고스트어택 normal skill 호출됨");
                activeSkill = boss1;
            }
        }

        


    }
    /*else
        {
            if (bossController.currentBossHealth == 50 && !fakeAttackActivatedAt50)
            {
                boss3.SetActive(true); // 보스3 활성화
                fakeAttackSkill.ActivateNormalSkill();
                fakeAttackActivatedAt50 = true;

                // 활성화된 스킬을 추적 변수에 할당
                activeSkill = boss3;
            }
        }*/


    private void ActivateRandomMixedSkill()
    {
        if (activeSkill == null && !isFakeAttackActive)
        {
            float randomValue = UnityEngine.Random.value;

            if (randomValue < 0.2f) // 20% 확률로 FlowerRain 일반 스킬
            {
                ActivateBossAtViewportPosition(boss4);
                flowerRain.ActivateNormalSkill();
                activeSkill = boss4;
                Debug.Log("FlowerRain (Normal) activated");
            }
            else if (randomValue < 0.4f) // 20% 확률로 BossFlowerThrow 일반 스킬
            {
                ActivateBossAtViewportPosition(boss2);
                bossFlowerThrow.ActivateNormalSkill();
                activeSkill = boss2;
                //Debug.Log("BossFlowerThrow (Normal) activated");
            }
            else if (randomValue < 0.6f) // 20% 확률로 BossFlowerThrow 강화 스킬
            {
                //boss2.SetActive(true);
                ActivateBossAtViewportPosition(boss2);

                bossFlowerThrow.ActivateEnhancedSkill();
                activeSkill = boss2;
                //Debug.Log("BossFlowerThrow (Enhanced) activated");
            }
            else if (randomValue < 0.9f) // 20% 확률로 GhostAttack 일반 스킬
            {
                //yield return new WaitForSeconds(1f);
                ActivateBossAtViewportPosition(boss1);
                ghostAttack.ActivateNormalSkill();
                activeSkill = boss1;
                //Debug.Log("GhostAttack (Normal) activated");
            }
            // 추가로 다른 스킬이나 기본 동작을 추가할 수 있습니다.
        }
    }




    private bool ShouldActivateSkill()
    {
        return Time.time >= nextSkillTime;
    }




}
