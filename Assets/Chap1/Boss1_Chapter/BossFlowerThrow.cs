using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossFlowerThrow : MonoBehaviour
{
    public GameObject flowerPrefab;
    public GameObject impactEffectPrefab; // 이펙트 프리팹 추가

    public Transform headTransform;
    public Camera mainCamera; // 메인 카메라
    public Vector3 headLocalPosition; // 카메라에 상대적인 헤드 위치


    public float flowerSpeed = 20f;
    public int numberOfFlowers = 3;
    public int enhancedNumberOfFlowers = 5;
    public float flowerSpacing = 1.0f;
    private bool skillEnhanced = false;
    private bool isThrowingFlowers = false;

    public Transform bossTransform; // 보스의 Transform
    public Vector3 headOffset; // headTransform과 보스 사이의 상대적 위치 오프셋
    private Vector3 someOffset; // 이 값을 적절히 설정해야 합니다.



    private BossController bossController;
    private Vector3 initialPlayerPosition;
    // 이벤트 선언
    public delegate void SkillCompleted();
    public event SkillCompleted OnNormalSkillCompleted;
    public event SkillCompleted OnEnhancedSkillCompleted;
    public enum BossSkillType
    {
        GhostAttack,
        BossFlowerThrow,
        FakeAttackSkill
    }
    private bool skillInProgress = false; // 스킬 진행 상태를 나타내는 플래그

    public bool IsSkillInProgress
    {
        get { return skillInProgress; }
    }
    /*public void ActivateEnhancedSkill()
    {
        // 강화된 스킬을 활성화하는 로직
        // 예: 더 많은 꽃을 생성하고 던지는 로직
        skillEnhanced = true;
        ThrowEnhancedFlowers();
    }
*/
    
    // 스킬 완료 이벤트를 MainGameController의 ActivateNextSkill 메서드에 연결
    private void Start()
    {
        bossController = BossController.Instance;
        initialPlayerPosition = PlayerPosition();

        // 이벤트 구독
        OnNormalSkillCompleted += FindObjectOfType<MainGameController>().ActivateNextSkill;
        OnEnhancedSkillCompleted += FindObjectOfType<MainGameController>().ActivateNextSkill;

        //ThrowFlowers();
        // 헤드 트랜스폼을 카메라의 자식으로 설정
        mainCamera = FindObjectOfType<MainGameController>().mainCamera;
        headOffset = new Vector3(0, 2, 1); // x, y, z 축에 대한 오프셋

        headTransform.SetParent(mainCamera.transform);
        headTransform.localPosition = headLocalPosition;

        // bossTransform을 현재 GameObject의 Transform으로 설정합니다.
        bossTransform = this.transform;

        // headTransform을 초기화합니다.
        UpdateHeadTransformPosition();
    }

    private void Update()
    {
        // `Update`에서 스킬을 직접 호출하지 않고, 플래그를 확인하여 필요한 메서드를 호출합니다.
        if (skillEnhanced && !isThrowingFlowers && !skillInProgress)
        {
            ActivateEnhancedSkill();
        }

        if (headTransform != null && bossTransform != null)
        {
            // headTransform의 위치를 보스의 위치에 맞춥니다.
            headTransform.position = bossTransform.position + someOffset;
        }
    }
    
    public void UpdateHeadTransformPosition()
    {
        if (headTransform != null && bossTransform != null)
        {
            // headTransform의 위치를 보스의 현재 위치에 맞춥니다.
            headOffset = new Vector3(0, 3, 0);
            headTransform.position = bossTransform.position + headOffset;
        }
    }
    

    public void ThrowFlowers()
    {
        if (!isThrowingFlowers && !skillEnhanced)
        {
            StartCoroutine(ThrowFlowersCoroutine());
        }
    }

    private IEnumerator ThrowFlowersCoroutine()
    {
        isThrowingFlowers = true;
        skillInProgress = true;

        for (int i = 0; i < numberOfFlowers; i++)
        {
            Vector3 flowerPosition = headTransform.position + Vector3.right * (i - (numberOfFlowers - 1) * 0.5f) * flowerSpacing;
            GameObject flower = Instantiate(flowerPrefab, flowerPosition, Quaternion.identity);

            // 페이드 인 애니메이션
            StartCoroutine(FadeInFlower(flower));

            // 꽃을 던지는 코루틴 호출
            StartCoroutine(LaunchFlowerAfterDelay(flower, 1.5f)); // 1.5초 후에 던지기
        }

        // 모든 꽃을 던진 후에 isThrowingFlowers 상태를 업데이트합니다.
        yield return new WaitForSeconds(3f);
        isThrowingFlowers = false;

        yield return new WaitForSeconds(0.5f);
        skillInProgress = false;
        OnNormalSkillCompleted?.Invoke();
    }

    private IEnumerator ThrowEnhancedFlowersCoroutine()
    {
        isThrowingFlowers = true;
        skillInProgress = true;
        int numPetals = enhancedNumberOfFlowers;
        float petalAngle = 360f / numPetals;
        List<GameObject> createdFlowers = new List<GameObject>();

        for (int i = 0; i < numPetals; i++)
        {
            float angle = i * petalAngle;
            Vector3 flowerDirection = Quaternion.Euler(0, 0, angle) * Vector3.up;
            Vector3 flowerPosition = headTransform.position + flowerDirection * flowerSpacing;
            GameObject flower = Instantiate(flowerPrefab, flowerPosition, Quaternion.identity);

            // 페이드 인 애니메이션
            StartCoroutine(FadeInFlower(flower));
            createdFlowers.Add(flower);

        }

        // 모든 꽃을 생성한 후 1초 대기
        yield return new WaitForSeconds(1f);

        // 이제 각 꽃을 순차적으로 던집니다.
        foreach (GameObject flower in createdFlowers)
        {
            StartCoroutine(LaunchFlowerAfterDelay(flower, 0.5f));  // 여기서 딜레이 조절 가능
            yield return new WaitForSeconds(0.5f);  // 다음 꽃 던지기 전 딜레이
        }

        // 강화된 꽃을 모두 던지고 난 후에 스킬 완료 이벤트를 발생시킵니다.
        isThrowingFlowers = false;
        skillInProgress = false;
        skillEnhanced = false;
        OnEnhancedSkillCompleted?.Invoke();
    }

    private IEnumerator FadeInFlower(GameObject flower)
    {
        Renderer flowerRenderer = flower.GetComponent<Renderer>();
        Material flowerMaterial = flowerRenderer.material;

        // 초기 투명도를 0으로 설정
        Color startColor = flowerMaterial.color;
        startColor.a = 0f;
        flowerMaterial.color = startColor;

        // 페이드 인 애니메이션
        float fadeInDuration = 0.5f;
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            Color newColor = flowerMaterial.color;
            newColor.a = alpha;
            flowerMaterial.color = newColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }


    private void ThrowEnhancedFlowers()
    {
        if (skillEnhanced)
        {
            StartCoroutine(ThrowEnhancedFlowersCoroutine());

        }

    }
    


    private Vector3 PlayerPosition()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            return playerObject.transform.position;
        }
        else
        {
            return Vector3.zero;
        }
    }

    private IEnumerator LaunchFlower(GameObject flower)
    {
        Vector3 playerPosition = PlayerPosition();
        Vector3 startPosition = flower.transform.position;
        Vector3 playerDirection = (playerPosition - startPosition).normalized;
        Rigidbody2D flowerRb = flower.GetComponent<Rigidbody2D>();
        flowerRb.velocity = playerDirection * flowerSpeed;

        float angle = Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg + 90f;
        flower.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // 이펙트가 발생할 때까지 대기합니다.
        while (Vector3.Distance(flower.transform.position, playerPosition) > 0.5f)
        {
            yield return null;
        }

        // 꽃이 목표 지점에 도달하면 이펙트 생성
        CreateImpactEffect(flower.transform.position);
        Destroy(flower); // 꽃 오브젝트 삭제 (필요한 경우)
    }



    private IEnumerator LaunchFlowerAfterDelay(GameObject flower, float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 playerPosition = PlayerPosition();
        Vector3 startPosition = flower.transform.position;
        Vector3 playerDirection = (playerPosition - startPosition).normalized;
        Rigidbody2D flowerRb = flower.GetComponent<Rigidbody2D>();
        flowerRb.velocity = playerDirection * flowerSpeed;

        float angle = Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg + 90f;
        flower.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // 이펙트가 발생할 때까지 대기합니다.
        while (Vector3.Distance(flower.transform.position, playerPosition) > 0.5f)
        {
            yield return null;
        }

        // 꽃이 목표 지점에 도달하면 이펙트 생성
        CreateImpactEffect(flower.transform.position);
        Destroy(flower); // 꽃 오브젝트 삭제 (필요한 경우)
    }


    // 이펙트 생성 함수
    private void CreateImpactEffect(Vector3 position)
    {
        if (impactEffectPrefab != null)
        {
            GameObject impactEffect = Instantiate(impactEffectPrefab, position, Quaternion.identity);
            // 이펙트가 3초 후에 파괴되도록 설정
            Destroy(impactEffect, 3f);
        }
    }


    private IEnumerator EnhanceSkillAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        skillEnhanced = true;

        // 현재 꽃을 던지고 있는 상태가 아니라면 바로 강화된 꽃 던지기 실행
        if (!isThrowingFlowers)
        {
            ThrowEnhancedFlowers();
        }

    }

    public bool IsSkillActivated()
    {
        return isThrowingFlowers;
    }

    // 일반 스킬 활성화 메서드
    public void ActivateNormalSkill()
    {
        
        if (!isThrowingFlowers && !skillEnhanced && !skillInProgress)
        {
            StartCoroutine(ThrowFlowersCoroutine());
        }
    }

    // 강화된 스킬 활성화 메서드
    
    public void ActivateEnhancedSkill()
    {
        if (!isThrowingFlowers && !skillInProgress)
        {
            skillInProgress = true; // 스킬이 시작될 때 플래그를 true로 설정

            StartCoroutine(ThrowEnhancedFlowersCoroutine());
        }
    }

    /*public void ActivateRandomSkill()
    {
        if (bossController.currentBossHealth < 50)
        {
            ActivateEnhancedSkill(); // 체력이 50 이하일 때 강화된 스킬 실행
        }
        else
        {
            ActivateNormalSkill(); // 그렇지 않으면 일반 스킬 실행
        }
    }*/

}
