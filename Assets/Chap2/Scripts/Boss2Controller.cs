using UnityEngine;
using System.Collections;

public class Boss2Controller : MonoBehaviour
{
    public GameObject cablePrefab; // 인스펙터에서 설정할 수 있는 케이블 프리팹


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

        // 현재 시간이 마지막 공격 시간 + 지연 시간보다 큰 경우에만 공격을 수행
        if (Time.time > lastAttackTime + attackDelay)
        {
            if (Random.value > 0.5f) // 50% 확률로 케이블 공격 선택
            {
                //Debug.Log("케이블 공격");
                AttackPlayer(); // 케이블 공격 수행

            }
            else
            {
                LaunchObstacleAttack(); // 장애물 공격 수행

            }
            lastAttackTime = Time.time; // 마지막 공격 시간 업데이트
        }

        UpdateTargetPointPosition();

    }
    private void UpdateTargetPointPosition()
    {
        // 카메라 뷰포트의 오른쪽 끝 y축 정중앙에 타겟 포인트 위치 설정
        Vector3 viewportPoint = new Vector3(1, 0.5f, offset); // offset은 카메라에서 타겟 포인트까지의 거리
        Vector3 targetPosition = Camera.main.ViewportToWorldPoint(viewportPoint);

        targetPoint.transform.position = targetPosition;
    }


    private void SpawnCable()
    {
        GameObject spawnPoint = ChooseSpawnPoint();
        GameObject newCable = Instantiate(cablePrefab, spawnPoint.transform.position, Quaternion.identity);
        cable cableScript = newCable.GetComponent<cable>();
        cableScript.Launch(targetPoint, spawnPoint, this); // 올바른 returnPoint를 전달
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
        cableScript.Launch(targetPoint, spawnPoint, this);
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

    /*Vector3 CalculateTargetPosition()
    {
        Vector3 playerPosition = player.transform.position;
        float yOffset;

        switch (player.GetComponent<PlayerController>().playerPosition)
        {
            case PlayerController.PlayerPosition.Top:
                yOffset = 1.0f; // 상단부 위치의 Y 오프셋
                break;
            case PlayerController.PlayerPosition.Middle:
                yOffset = 0.0f; // 중간부 위치의 Y 오프셋
                break;
            case PlayerController.PlayerPosition.Bottom:
                yOffset = -1.0f; // 하단부 위치의 Y 오프셋
                break;
            default:
                yOffset = 0.0f; // 기본값은 중간부로 설정
                break;
        }

        return new Vector3(playerPosition.x, playerPosition.y + yOffset, playerPosition.z);
    }*/
    // 케이블과 부딫혔을 때 보스와 플레이어간의 거리 가깝게 조정
    public void OnCableHit()
    {
        // 플레이어와 보스의 거리를 조절하는 로직
        // 예시: 보스를 플레이어에게 조금 더 가깝게 이동시킵니다.
        Vector3 newPosition = transform.position;
        newPosition.x = Mathf.Lerp(newPosition.x, player.position.x, 0.1f); // X축으로 조금 이동
        transform.position = newPosition;
    }
}