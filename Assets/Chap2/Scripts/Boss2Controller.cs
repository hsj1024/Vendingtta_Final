using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boss2Controller : MonoBehaviour
{
    public GameObject cablePrefab; // 인스펙터에서 설정할 수 있는 케이블 프리팹
    public GameObject ballRainHitAreaPrefab;

    public GameObject transparentWall; // 투명 벽에 대한 참조
    private bool isEnhancedSkillActive = false; // 강화 스킬 활성화 여부
    private bool hasEnhancedSkillActivatedOnce = false; // 강화 스킬이 한 번이라도 활성화되었는지
    private bool isSkillActive = false; // 현재 스킬이 실행 중인지 여부를 나타내는 플래그
    public GameObject[] EnhancedSpawnPoints; // 강화 스킬에 사용될 스폰 포인트 배열


    public float attackDelay = 2f; // 공격 간격
    public Transform player;
    //public float offsetToPlayer; // 플레이어에 대한 보스의 오프셋
    public GameObject[] spawnPoints; // 발사 위치 배열
    public float moveSpeed = 5f; // 보스의 이동 속도
    public float cableSpawnDelay = 1f; // 케이블이 재생성되기까지의 지연 시간

    private float lastAttackTime;
    private int lastSpawnIndex = -1;

    public GameObject targetPoint; // 타겟 포인트 객체
    public float offset = 10f; // 카메라 오른쪽 끝에서의 오프셋

    // 키보드 공격
    public GameObject obstaclePrefab; // 장애물 프리팹
    public GameObject PophitAreaPrefab; // 피격 범위 프리팹
    public GameObject CablehitAreaPrefab; // 피격 범위 프리팹

    // 파편 뭉치 비
    public Transform fragmentSpawnPoint; // 키보드 파편 뭉치 스폰 위치
    public float fragmentSpawnDelay = 1.5f; // 스킬 발동 지연 시간
    public float fragmentRadius = 5f; // 플레이어 주변 키보드 파편의 반경

    public GameObject keyboardFragmentPrefab; // 키보드 파편 뭉치 프리팹

    public GameObject nonMotionSpritePrefab; // 비 모션 스프라이트 프리팹

    public void RespawnCable()
    {
        StartCoroutine(SpawnCableAfterDelay());
    }

    // 지연 시간 후 케이블 오브젝트를 생성하는 코루틴
    private IEnumerator SpawnCableAfterDelay()
    {
        yield return new WaitForSeconds(cableSpawnDelay);
        SpawnCable();
    }

    void Update()
    {
        if (Time.time > lastAttackTime + attackDelay && !isSkillActive)
        {
            // 강화 스킬이 활성화된 경우
            if (isEnhancedSkillActive)
            {
                ActivateEnhancedCableSkill();
                hasEnhancedSkillActivatedOnce = true;
                isEnhancedSkillActive = false;
            }
            else if (hasEnhancedSkillActivatedOnce)
            {
                // 무작위로 일반 스킬 또는 강화 스킬 실행
                if (Random.value > 0.5f)
                {
                    ActivateEnhancedCableSkill();
                }
                else
                {
                    ExecuteNormalSkillLogic();
                }
            }
            else
            {
                // 일반 스킬 실행
                ExecuteNormalSkillLogic();
            }
            lastAttackTime = Time.time;
        }
    }

    void ExecuteNormalSkillLogic()
    {
        // 평소 스킬 로직
        if (Random.value > 0.2f)
        {
            AttackPlayer();
        }
        else if (Random.value > 0.5f)
        {
            LaunchObstacleAttack();
        }
        else
        {
            StartCoroutine(DelayedLaunchKeyboardFragment());
        }
    }

    
    private void UpdateTargetPointPosition()
    {
        // 카메라 뷰포트의 오른쪽 끝 y축 정중앙에 타겟 포인트 위치 설정
        Vector3 viewportPoint = new Vector3(1, 0.5f, offset); // offset은 카메라에서 타겟 포인트까지의 거리
        Vector3 targetPosition = Camera.main.ViewportToWorldPoint(viewportPoint);

        targetPoint.transform.position = targetPosition;
    }

    IEnumerator DelayedLaunchKeyboardFragment()
    {
        yield return new WaitForSeconds(fragmentSpawnDelay);

        // 키보드 파편 뭉치 스킬 발동
        LaunchKeyboardFragment();
    }

    void LaunchKeyboardFragment()
    {
        // 카메라 이동에 따라 스폰 위치를 카메라의 정중앙 상단으로 설정
        Vector3 cameraTopCenter = new Vector3(Camera.main.transform.position.x,
                                              Camera.main.transform.position.y + Camera.main.orthographicSize,
                                              Camera.main.transform.position.z);
        GameObject fragment = Instantiate(keyboardFragmentPrefab, cameraTopCenter, Quaternion.identity);

        // 키보드 파편 뭉치가 나간 직후 플레이어 위치에 피격 범위를 표시하고 1.5초 후에 사라지게 함
        StartCoroutine(ShowHitAreaAndSpawnRainPrefab(player.transform.position));
    }



    IEnumerator ShowHitAreaAndSpawnRainPrefab(Vector3 targetPosition)
    {
        // 새로운 피격 범위 스프라이트 사용
        GameObject hitArea = Instantiate(ballRainHitAreaPrefab, targetPosition, Quaternion.identity);
        yield return new WaitForSeconds(1.5f);
        Destroy(hitArea);

        // 피격 범위 사라진 후 비 프리팹 생성
        Instantiate(nonMotionSpritePrefab, targetPosition, Quaternion.identity);
    }

    private void SpawnCable()
    {
        GameObject spawnPoint = ChooseSpawnPoint();
        GameObject newCable = Instantiate(cablePrefab, spawnPoint.transform.position, Quaternion.identity);
        cable cableScript = newCable.GetComponent<cable>();
        cableScript.Launch(Vector3.right, this.gameObject, this); // 직선 이동 방향을 Vector3.right로 설정
    }

    // 케이블 공격
    void AttackPlayer()
    {
        //Debug.Log("케이블 공격 실행");

        GameObject spawnPoint = ChooseSpawnPoint();
        Vector3 hitPosition = spawnPoint.transform.position;

        // 피격 범위 스프라이트 생성
        GameObject hitArea = Instantiate(CablehitAreaPrefab, hitPosition, Quaternion.identity);
        if (hitArea != null)
        {
            //Debug.Log("피격 범위 스프라이트 생성됨: " + hitArea.name);
        }
        else
        {
            //Debug.LogError("피격 범위 스프라이트 생성 실패!");
        }

        // 피격 범위 스프라이트가 나타나는 시간 후에 케이블 생성
        StartCoroutine(DelayedSpawnCable(spawnPoint, 0.5f));

        // 피격 범위 스프라이트 제거
        Destroy(hitArea, 0.5f);
    }

    IEnumerator DelayedSpawnCable(GameObject spawnPoint, float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject newCable = Instantiate(cablePrefab, spawnPoint.transform.position, Quaternion.identity);
        newCable.SetActive(false); // 케이블을 비활성화 상태로 생성

        // 필요한 설정 후 케이블 활성화
        cable cableScript = newCable.GetComponent<cable>();
        cableScript.Launch(Vector3.right, this.gameObject, this); // 직선 이동 방향을 Vector3.right로 설정
        newCable.SetActive(true); // 케이블 활성화
    }




    // 키보드 장애물 공격
    void LaunchObstacleAttack()
    {
        // 플레이어의 현재 위치를 기준으로 피격 범위 설정
        Vector3 playerPosition = player.transform.position;

        for (int i = 0; i < 3; i++) // 3개의 피격 범위 생성
        {
            // 플레이어 주변의 랜덤한 위치 생성
            Vector3 hitPosition = playerPosition + new Vector3(Random.Range(0f, 5f), 0f, Random.Range(-3f, 3f));

            GameObject hitArea = Instantiate(PophitAreaPrefab, hitPosition, Quaternion.identity);
            Destroy(hitArea, 0.5f); // 0.5초 후에 피격 범위 제거

            StartCoroutine(SpawnObstacle(hitPosition, 1f)); // 장애물 생성

        }
    }


    System.Collections.IEnumerator SpawnObstacle(Vector3 position, float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject obstacle = Instantiate(obstaclePrefab, position, Quaternion.identity); // 장애물 생성
        SpriteRenderer renderer = obstacle.GetComponent<SpriteRenderer>(); // 또는 MeshRenderer

        // 1초 동안 페이드 아웃
        float fadeDuration = 3f;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            Color color = renderer.material.color;
            color.a = Mathf.Lerp(1, 0, t / fadeDuration);
            renderer.material.color = color;
            yield return null;
        }

        Destroy(obstacle); // 장애물 파괴
    }

    GameObject ChooseSpawnPoint()
    {
        lastSpawnIndex = (lastSpawnIndex + 1) % spawnPoints.Length;
        return spawnPoints[lastSpawnIndex];
    }

    
    // 케이블과 부딫혔을 때 보스와 플레이어간의 거리 가깝게 조정
    public void OnCableHit()
    {
        // 플레이어와 보스의 거리를 조절하는 로직
        // 예시: 보스를 플레이어에게 조금 더 가깝게 이동시킵니다.
        Vector3 newPosition = transform.position;
        newPosition.x = Mathf.Lerp(newPosition.x, player.position.x, 0.1f); // X축으로 조금 이동
        transform.position = newPosition;
    }

    // 올가미 강화 스킬

    // 투명 벽 콜라이더 생성 위와 닿으면 강화스킬 실행
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided with: " + other.gameObject.tag);

        if (other.CompareTag("TransparentWall"))
        {
            Debug.Log("TransparentWall hit!");
            isEnhancedSkillActive = true;
        }
    }



    public void ActivateEnhancedCableSkill()
    {
        isSkillActive = true; // 스킬 실행 중 표시

        // 강화 스킬 스폰 포인트 중 무작위로 2개 선택
        List<int> selectedIndexes = new List<int>();
        while (selectedIndexes.Count < 2)
        {
            int randomIndex = Random.Range(0, EnhancedSpawnPoints.Length);
            if (!selectedIndexes.Contains(randomIndex))
            {
                selectedIndexes.Add(randomIndex);
            }
        }

        // 선택된 스폰 포인트에서 케이블 발사
        foreach (int index in selectedIndexes)
        {
            Vector3 spawnPosition = EnhancedSpawnPoints[index].transform.position;
            CreateHitAreaAt(spawnPosition);
            StartCoroutine(DelayedLaunchCableAt(spawnPosition, 0.5f));
        }

        StartCoroutine(ResetSkillActiveFlag());
    }

    IEnumerator DelayedLaunchCableAt(Vector3 position, float delay)
{
    yield return new WaitForSeconds(delay);
    LaunchCableAt(position);
}

void LaunchCableAt(Vector3 position)
{
    GameObject newCable = Instantiate(cablePrefab, position, Quaternion.identity);
    cable cableScript = newCable.GetComponent<cable>();
    cableScript.Launch(Vector3.right, this.gameObject, this); // 직선 이동 방향을 Vector3.right로 설정

    }


    void CreateHitAreaAt(Vector3 position)
    {
        // 피격 범위 스프라이트 생성
        GameObject hitArea = Instantiate(CablehitAreaPrefab, position, Quaternion.identity);
        Destroy(hitArea, 0.5f); // 0.5초 후에 피격 범위 제거
    }
    


    
    IEnumerator ResetSkillActiveFlag()
    {
        // 스킬 실행 후 정리
        yield return new WaitForSeconds(1f);
        isSkillActive = false;

        // 임시로 생성된 targetDummy 객체가 있다면 파괴
        GameObject targetDummy = GameObject.Find("TargetDummy");
        if (targetDummy != null)
        {
            Destroy(targetDummy);
        }
    }



    Vector3 GetRandomPosition()
    {
        // 화면 내에서 무작위 위치 계산
        float randomX = Random.Range(0f, Screen.width);
        float randomY = Random.Range(0f, Screen.height);
        Vector3 randomScreenPosition = new Vector3(randomX, randomY, 0);

        // 스크린 좌표를 월드 좌표로 변환
        Vector3 randomWorldPosition = Camera.main.ScreenToWorldPoint(randomScreenPosition);
        return randomWorldPosition;
    }


}