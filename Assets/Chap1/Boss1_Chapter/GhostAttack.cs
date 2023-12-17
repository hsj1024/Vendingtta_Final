using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Pool;
using Unity.Mathematics;
using System.Linq;

public class GhostAttack : MonoBehaviour
{
    private MainGameController mainGameController;

    public GameObject ghost1Prefab;
    public GameObject ghost2Prefab;
    public Transform player;

    public Transform westSpawnPoint;
    public Transform northSpawnPoint;
    public Transform eastSpawnPoint;
    private bool northSpawned = false;

    public int numberOfGhostsPerType = 1;
    public int numberOfGhostsPerGroup = 3;
    public float yOffset = 1f;
    public float spawnDelay = 2f;


    public float skillActivationTime = 9f;
    public float ghostPauseTime = 1.0f;

    private Camera mainCamera;
    private List<GameObject> northGhosts = new List<GameObject>();
    private List<GameObject> westGhosts = new List<GameObject>();
    private List<GameObject> eastEnhancedGhosts = new List<GameObject>();
    private List<GameObject> northEnhancedGhosts = new List<GameObject>();
    private bool westSpawned = false;
    private bool westGhostsSpawnCompleted = false; // 서쪽 몬스터 스폰 완료 여부

    private bool northEnhancedSpawned = false;
    private bool skillTimerStarted = false;

    public Transform eastEndPosition;
    public Transform northEndPosition;
    public float westGhostSpacing = 1.0f;

    //스킬 완료 변수
    public delegate void SkillCompleted();
    public event SkillCompleted OnNormalSkillCompleted;
    public event SkillCompleted OnEnhancedSkillCompleted;


    public delegate void AttackCompletedHandler();
    public event AttackCompletedHandler AttackCompleted;
    public float ghostMoveSpeed = 15f;

    public List<Transform> northSpawnPoints; // 북쪽 몬스터 스폰 위치 리스트
    public List<Transform> westSpawnPoints; // 서쪽 몬스터 스폰 위치 리스트
    public List<Transform> eastSpawnPoints; // 서쪽 몬스터 스폰 위치 리스트

    private BossController bossController;

    private int currentSkillGroup = 0; // 현재 실행 중인 스킬 그룹 인덱스

    private int westGhostIndex = 0; // 서쪽 몬스터 스폰 위치 인덱스
    //private bool northSkillActivated = false; // 북쪽 스킬이 활성화되었는지 여부

    private bool isFirstSkill = true; // 처음 스킬을 나타내는 변수

    private float enhancedGhostMoveSpeed = 10.0f; // EnhancedNorthGhosts 전용 이동 속도
                                                  // 강화된 북쪽 고스트 스폰 위치 리스트
    public List<Transform> enhancedNorthSpawnPoints;
    public bool normalSkillCompleted = false;  // 일반 스킬 끝났는지 확인
    public float timeBetweenSkills = 4.0f; // 북쪽과 서쪽 스킬 간의 대기 시간 (초 단위)
    // 강화된 동쪽 고스트 스폰 위치 리스트
    public List<Transform> enhancedEastSpawnPoints;

    public List<Transform> northMonsterSpawnPoints; // 북쪽 몬스터 스폰 위치 리스트
    public List<Transform> westMonsterSpawnPoints; // 서쪽 몬스터 스폰 위치 리스트

    public List<Transform> northHitRangeSpawnPoints; // 북쪽 피격 범위 스폰 위치 리스트
    public List<Transform> westHitRangeSpawnPoints; // 서쪽 피격 범위 스폰 위치 리스트
    public GameObject northHitRangePrefab;
    public GameObject westHitRangePrefab;

    public bool skillActivated = false;
    private bool skillInProgress = false; // 스킬 진행 상태를 나타내는 플래그
                                          // 피격 범위가 생성된 카메라 위치를 저장할 필드 추가
    private Vector3 savedCameraPositionForNorthGhosts;
    private Vector3 savedCameraPositionForWestGhosts;
    public bool IsSkillInProgress
    {
        get { return skillInProgress; }
    }



    private void Start()
    {
        mainCamera = Camera.main;
        bossController = BossController.Instance;
        mainGameController = FindObjectOfType<MainGameController>();


        /*// 북쪽 스폰 포인트를 카메라의 자식으로 설정하고 상대적 위치를 정의
        northSpawnPoint.SetParent(mainCamera.transform, false);
        northSpawnPoint.localPosition = new Vector3(0, 10, 15); // 카메라로부터 위로 10, 앞으로 15의 위치

        // 서쪽 스폰 포인트도 카메라의 자식으로 설정하고 상대적 위치를 정의
        westSpawnPoint.SetParent(mainCamera.transform, false);
        westSpawnPoint.localPosition = new Vector3(-10, 0, 15); // 카메라로부터 왼쪽으로 10, 앞으로 15의 위치*/

        // 카메라의 자식으로 피격 범위 스폰 포인트 설정
        foreach (Transform spawnPoint in northHitRangeSpawnPoints)
        {
            spawnPoint.SetParent(mainCamera.transform, false);
            // 여기에 상대적 위치 정의
            //spawnPoint.localPosition = new Vector3(0, 10, 15);
        }

        // 서쪽 피격 범위 스폰 포인트에 대해서도 동일하게 적용
        foreach (Transform spawnPoint in westHitRangeSpawnPoints)
        {
            spawnPoint.SetParent(mainCamera.transform, false);

            // spawnPoint.localPosition = new Vector3(-10, 0, 15);
        }

        savedCameraPositionForNorthGhosts = mainCamera.transform.position;
        savedCameraPositionForWestGhosts = mainCamera.transform.position;
    }



    private void OnDestroy()
    {

    }



    private void Update()
    {
        if (!skillActivated)
        {
            if (bossController.currentBossHealth > 50)
            {
                // 체력이 50 이상일 때의 스킬 활성화 로직
                StartCoroutine(ActivateNormalSkill());
                skillActivated = true; // 스킬이 활성화되었음을 나타냄
            }
            else
            {
                StartCoroutine(ActivateNormalSkill());
                skillActivated = true;

            }
        }

    }




    // 피격 범위 메소드
    private IEnumerator ShowHitRangeAndSpawnGhosts(List<Transform> hitRangeSpawnPoints, IEnumerator spawnGhostsCoroutine, GameObject hitRangePrefab)
    {
        // 피격 범위 표시
        foreach (var spawnPoint in hitRangeSpawnPoints)
        {
            ShowHitRangeAt(spawnPoint, hitRangePrefab);
        }

        // 피격 범위 표시 후 일정 시간 기다림
        yield return new WaitForSeconds(0.5f);

        // 고스트 스폰
        yield return StartCoroutine(spawnGhostsCoroutine);
    }

    // 피격 범위 위치를 저장할 리스트
    private List<Vector3> hitRangePositions = new List<Vector3>();

    // 피격 범위 생성 로직 수정
    private void ShowHitRangeAt(Transform spawnPoint, GameObject hitRangePrefab)
    {
        Vector3 hitRangePosition = mainCamera.transform.TransformPoint(spawnPoint.localPosition);
        hitRangePositions.Add(hitRangePosition); // 위치 저장
        GameObject hitRangeInstance = Instantiate(hitRangePrefab, hitRangePosition, Quaternion.identity);

        StartCoroutine(FadeHitRange(hitRangeInstance));
    }

    private IEnumerator FadeHitRange(GameObject hitRange)
    {
        SpriteRenderer renderer = hitRange.GetComponent<SpriteRenderer>();
        if (renderer == null)
        {
            yield break;
        }

        float maxAlpha = 0.4f; // 최대 알파 값
        float duration = 0.5f;

        // 페이드 인
        for (float t = 0; t < maxAlpha; t += Time.deltaTime / duration)
        {
            Color newColor = renderer.color;
            newColor.a = t;
            renderer.color = newColor;
            yield return null;
        }

        // 유지
        yield return new WaitForSeconds(0.5f);

        // 페이드 아웃
        for (float t = maxAlpha; t > 0; t -= Time.deltaTime / duration)
        {
            Color newColor = renderer.color;
            newColor.a = t;
            renderer.color = newColor;
            yield return null;
        }

        Destroy(hitRange);
    }







    private bool AllGhostsOutOfScreen(List<GameObject> ghosts)
    {
        // 모든 고스트가 화면 밖으로 나갔는지 확인
        foreach (GameObject ghost in ghosts)
        {
            if (ghost != null)
            {
                Vector3 screenPosition = mainCamera.WorldToViewportPoint(ghost.transform.position);

                // 화면 밖으로 완전히 나가지 않았으면 false 반환
                if (screenPosition.x > 0 && screenPosition.x < 1 && screenPosition.y > 0 && screenPosition.y < 1)
                {
                    return false;
                }
            }
        }

        // 모든 고스트가 화면 밖으로 나갔으면 true 반환
        return true;
    }













    // 몬스터를 스폰하고 이동시킴
    /* private IEnumerator SpawnGhosts(List<Transform> spawnPoints, Vector3 direction, GameObject prefab, List<GameObject> list, int numberOfGhosts)
     {
         foreach (var spawnPoint in spawnPoints)
         {
             Vector3 spawnPosition = spawnPoint.position;

             GameObject ghost = Instantiate(prefab, spawnPosition, Quaternion.identity);
             list.Add(ghost);

             StartCoroutine(MoveGhost(ghost, direction, ghostMoveSpeed));
             yield return new WaitForSeconds(spawnDelay);
         }
     }*/
    // 임의적인 수정 카메라 이동때문에
    // 몬스터 스폰 로직 수정
    // 고스트 스폰 로직 수정
    private IEnumerator SpawnGhosts(List<GameObject> list, GameObject prefab, int numberOfGhosts, Vector3 direction)
    {
        if (hitRangePositions.Count == 0)
        {
            yield break; // 피격 범위 위치가 없으면 함수 종료
        }

        foreach (var hitRangePosition in hitRangePositions)
        {
            GameObject ghost = Instantiate(prefab, hitRangePosition, Quaternion.identity);
            list.Add(ghost);

            // 고스트 이동 로직 ...
            StartCoroutine(MoveGhost(ghost, direction, ghostMoveSpeed));

            yield return new WaitForSeconds(spawnDelay); // 이동 로직 후 일정 시간 대기
        }
        hitRangePositions.Clear(); // 스폰 후 목록을 비움
    }



    /*private IEnumerator SpawnNorthGhosts()
    {
        int numberOfGroups = 1;
        float xOffsetMultiplier = 2.0f; // X축 간격을 조절할 변수

        for (int j = 0; j < numberOfGroups; j++)
        {
            List<Transform> spawnPoints = new List<Transform>();

            // 피격 범위를 모든 스폰 지점에 대해 한 번에 표시
            foreach (Transform spawnPoint in northSpawnPoints)
            {

                ShowHitRangeAt(spawnPoint.position, northHitRangePrefab);
            }

            // 피격 범위 표시 후 일정 시간 기다림
            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < numberOfGhostsPerGroup; i++)
            {
                int xOffset = i * (int)(xOffsetMultiplier); // X축 간격을 xOffset으로 지정
                int yOffset = 0; // Y축 간격은 0으로 지정하여 세로 정렬을 없앰

                spawnPoints.Add(northSpawnPoints[i]); // 스폰 지점을 리스트에 추가
            }
            yield return new WaitForSeconds(0.5f);

            yield return SpawnGhosts(spawnPoints, Vector3.down, ghost1Prefab, northGhosts, numberOfGhostsPerGroup);
            //yield return new WaitForSeconds(spawnDelay);
        }
        Debug.Log("Spawned north ghosts");
    }*/
    // 북쪽 고스트 스폰 로직 예시
    private IEnumerator SpawnNorthGhosts()
    {

        hitRangePositions.Clear(); // 리스트 초기화

        // 피격 범위를 모든 스폰 지점에 대해 한 번에 표시
        foreach (Transform spawnPoint in northHitRangeSpawnPoints)
        {
            ShowHitRangeAt(spawnPoint, northHitRangePrefab);
        }

        // 피격 범위 표시 후 일정 시간 기다림
        yield return new WaitForSeconds(0.5f);

        // 고스트 스폰
        yield return StartCoroutine(SpawnGhosts(northGhosts, ghost1Prefab, 1, Vector3.down));
    }






    /*private IEnumerator SpawnWestGhosts()
    {
        *//*if (1)
        {*//*
        westSpawned = true; // 서쪽 몬스터 그룹 스폰 시작됨을 표시
        for (int i = 0; i < numberOfGhostsPerGroup; i++)
            {
                Transform spawnPoint = westSpawnPoints[westGhostIndex];

                ShowHitRangeAt(spawnPoint.position, westHitRangePrefab);

                yield return new WaitForSeconds(0.5f); // 피격 범위 표시 시간

                // 몬스터 스폰
                List<Transform> spawnPoints = new List<Transform> { spawnPoint };
                yield return SpawnGhosts(spawnPoints, Vector3.right, ghost2Prefab, westGhosts, numberOfGhostsPerGroup);

                westGhostIndex = (westGhostIndex + 1) % westSpawnPoints.Count;

                // 마지막 몬스터가 아닐 경우, 다음 몬스터 스폰 전 딜레이
                if (i < numberOfGhostsPerGroup - 1)
                {
                    yield return new WaitForSeconds(spawnDelay);
                }
            }
        *//*}*//*

        // 모든 서쪽 몬스터가 스폰 완료되었음을 나타내는 플래그 설정
        //yield return new WaitForSeconds(1f);
        westGhostsSpawnCompleted = true;
    }*/

    // SpawnWestGhosts 함수의 오버로드 버전
    private IEnumerator SpawnWestGhosts()
    {
        westSpawned = true;

        for (int i = 0; i < 3; i++)
        {
            Transform spawnPoint = westSpawnPoints[i % westSpawnPoints.Count];
            ShowHitRangeAt(spawnPoint, westHitRangePrefab);

            // 피격 범위 표시 후 고스트 스폰
            yield return new WaitForSeconds(0.5f); // 피격 범위 표시 시간

            GameObject ghost = Instantiate(ghost2Prefab, hitRangePositions[i], Quaternion.identity);
            westGhosts.Add(ghost);

            // 고스트 이동
            StartCoroutine(MoveGhost(ghost, Vector3.right, ghostMoveSpeed));

            // 다음 고스트 스폰 전에 일정 시간 대기
            yield return new WaitForSeconds(spawnDelay);
        }

        westGhostsSpawnCompleted = true;
    }








    // 몬스터 이동 관리
    private IEnumerator MoveGhost(GameObject ghost, Vector3 direction, float moveSpeed)
    {
        float cameraFarPlane = Camera.main.farClipPlane;
        Vector3 viewportEdge = direction.x > 0 ? new Vector3(1, 0.5f, cameraFarPlane) : new Vector3(0, 0.5f, cameraFarPlane);
        Vector3 worldEdge = Camera.main.ViewportToWorldPoint(viewportEdge);
        float distanceToEdge = Vector3.Distance(ghost.transform.position, worldEdge);
        float additionalDistance = Mathf.Max(Screen.width, Screen.height) / Camera.main.pixelWidth * cameraFarPlane;
        float moveDistance = distanceToEdge + additionalDistance;

        Vector3 startPosition = ghost.transform.position;
        Vector3 endPosition = ghost.transform.position + (direction.normalized * moveDistance);

        float elapsedTime = 0f;
        float duration = moveDistance / moveSpeed;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            ghost.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 고스트가 화면 밖으로 나갔다면 원래 위치로 되돌림
        //ghost.transform.position = startPosition;
    }

    private Dictionary<GameObject, Vector3> ghostSpawnPositions = new Dictionary<GameObject, Vector3>();

    private IEnumerator EnhancedNorthGhosts()
    {
        List<GameObject> groupGhosts = new List<GameObject>();

        // 강화된 북쪽 고스트 생성 및 스폰
        foreach (Transform spawnPoint in enhancedNorthSpawnPoints)
        {
            for (int i = 0; i < numberOfGhostsPerType; i++)
            {
                GameObject ghost = Instantiate(ghost1Prefab, spawnPoint.position, Quaternion.identity);
                groupGhosts.Add(ghost);
                ghostSpawnPositions.Add(ghost, spawnPoint.position); // 스폰 위치 저장

                StartCoroutine(MoveGhost(ghost, Vector3.down, enhancedGhostMoveSpeed));
            }
        }

        // 모든 고스트가 화면 밖으로 이동할 때까지 대기
        yield return StartCoroutine(MoveGroupOfGhosts(groupGhosts, Vector3.down));

        // 1초 대기
        yield return new WaitForSeconds(1.0f);

        // 각 고스트를 즉시 원래 위치로 되돌림
        foreach (GameObject ghost in groupGhosts)
        {
            if (ghost != null && ghostSpawnPositions.ContainsKey(ghost))
            {
                Vector3 spawnPosition = ghostSpawnPositions[ghost];
                ghost.transform.position = spawnPosition; // 고스트 위치 즉시 업데이트
            }
        }
    }

    private IEnumerator EnhancedEastGhosts()
    {
        List<GameObject> groupGhosts = new List<GameObject>();

        // 강화된 동쪽 고스트 생성 및 스폰
        foreach (Transform spawnPoint in enhancedEastSpawnPoints)
        {
            for (int i = 0; i < numberOfGhostsPerType; i++)
            {
                GameObject ghost = Instantiate(ghost2Prefab, spawnPoint.position, Quaternion.identity);
                groupGhosts.Add(ghost);
                ghostSpawnPositions.Add(ghost, spawnPoint.position); // 스폰 위치 저장

                StartCoroutine(MoveGhost(ghost, Vector3.left, enhancedGhostMoveSpeed)); // 동쪽에서 서쪽으로 이동
            }
        }

        // 모든 고스트가 화면 밖으로 이동할 때까지 대기
        yield return StartCoroutine(MoveGroupOfGhosts(groupGhosts, Vector3.left));

        // 1초 대기
        yield return new WaitForSeconds(1.5f);

        // 각 고스트를 즉시 원래 위치로 되돌림
        foreach (GameObject ghost in groupGhosts)
        {
            if (ghost != null && ghostSpawnPositions.ContainsKey(ghost))
            {
                Vector3 spawnPosition = ghostSpawnPositions[ghost];
                ghost.transform.position = spawnPosition; // 고스트 위치 즉시 업데이트
            }
        }
    }



    private IEnumerator MoveGroupOfGhosts(List<GameObject> ghosts, Vector3 direction)
    {
        float startTime = Time.time; // startTime을 한 번만 설정

        bool allGhostsMoved = false;
        while (!allGhostsMoved)
        {
            allGhostsMoved = true;

            foreach (GameObject ghost in ghosts)
            {
                if (ghost != null)
                {
                    float cameraFarPlane = Camera.main.farClipPlane;
                    Vector3 viewportEdge = direction.x > 0 ? new Vector3(1, 0.5f, cameraFarPlane) : new Vector3(0, 0.5f, cameraFarPlane);
                    Vector3 worldEdge = Camera.main.ViewportToWorldPoint(viewportEdge);
                    float distanceToEdge = Vector3.Distance(ghost.transform.position, worldEdge);
                    float additionalDistance = Mathf.Max(Screen.width, Screen.height) / Camera.main.pixelWidth * cameraFarPlane;
                    float moveDistance = distanceToEdge + additionalDistance;

                    Vector3 startPosition = ghost.transform.position;
                    Vector3 endPosition = ghost.transform.position + (direction.normalized * moveDistance);

                    // 시간 계산을 조절하여 이동 속도 변경
                    float elapsedTime = Time.time - startTime;
                    float duration = moveDistance / ghostMoveSpeed; // 이동에 필요한 총 시간
                    float fractionOfJourney = elapsedTime / duration; // 전체 이동 시간에 대한 현재 경과 시간의 비율

                    // fractionOfJourney가 1을 초과하지 않도록 제한
                    fractionOfJourney = Mathf.Clamp01(fractionOfJourney);

                    ghost.transform.position = Vector3.Lerp(startPosition, endPosition, fractionOfJourney);

                    if (fractionOfJourney < 1)
                    {
                        allGhostsMoved = false;
                    }
                }
            }

            yield return new WaitForEndOfFrame();

        }
    }







    // 강화된 스킬 완료 시 호출될 내부 메서드
    private void OnEnhancedSkillCompletedInternal()
    {
        OnEnhancedSkillCompleted?.Invoke();
    }


    /*public IEnumerator ActivateEnhancedSkill()
    {
        

        // 강화된 북쪽 고스트 스킬 활성화
        //yield return StartCoroutine(EnhancedNorthGhosts());

        // 강화된 동쪽 고스트 스킬 활성화
        *//*yield return StartCoroutine(SpawnNorthGhosts());

        skillActivated = false;
        AllGhostsOutOfScreen(northGhosts);
        Debug.Log("All enhanced skill groups completed, spawning next boss...");
        // 강화된 스킬 완료 이벤트 호출
        OnEnhancedSkillCompletedInternal();*//*
    }*/


    // 일반 스킬 호출 메서드

    public IEnumerator ActivateNormalSkill()
    {


        /*if (skillActivated)
        {
            // 이미 스킬이 활성화된 경우, 일정 시간 기다린 후 다시 시도
            Debug.Log("이미 스킬 활성화 됨. 대기중");
            yield return new WaitForSeconds(skillActivationTime);
        }*/
        skillInProgress = true; // 스킬이 시작될 때 플래그를 true로 설정


        Debug.Log("GHOST Skill activated.");

        if (bossController.currentBossHealth > 50 || bossController.currentBossHealth < 50)
        {
            // 북쪽 몬스터 스폰
            if (!northSpawned)
            {
                StartCoroutine(SpawnNorthGhosts());
                northSpawned = true;
                yield return new WaitUntil(() => AllGhostsOutOfScreen(northGhosts));
                Debug.Log("북쪽 고스트 아웃");
            }

            // 북쪽 몬스터가 모두 화면 밖으로 나간 후 서쪽 몬스터 스폰
            if (!westSpawned)
            {
                yield return new WaitForSeconds(timeBetweenSkills); // 서쪽 몬스터 스폰 전 대기
                StartCoroutine(SpawnWestGhosts());
                yield return new WaitUntil(() => westGhostsSpawnCompleted && AllGhostsOutOfScreen(westGhosts));
                Debug.Log("서쪽 고스트 아웃");
                yield return new WaitForSeconds(timeBetweenSkills); // 서쪽 몬스터 스폰 완료 후 약간 대기
                yield return new WaitForSeconds(1f); // 서쪽 몬스터 스폰 완료 후 약간 대기

            }
        }

        // 서쪽 몬스터가 모두 화면 밖으로 나간 것을 확인한 후 이벤트 발생
        // 스킬 완료 확인
        yield return new WaitUntil(() => AllGhostsOutOfScreen(westGhosts));

        Debug.Log("Skill completed.");
        skillInProgress = false;

        OnNormalSkillCompletedInternal();

    }


    // 일반 스킬 완료 시 호출될 내부 메서드
    private void OnNormalSkillCompletedInternal()
    {
        skillActivated = false;
        northSpawned = false;
        westSpawned = false;
        OnNormalSkillCompleted?.Invoke();

        Debug.Log("Skill reset.");
        
    }
    IEnumerator FadeOut(GameObject objectToFade, float duration)
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
        }
    }

    /*public bool IsSkillActivated()
    {
        return skillActivated;
    }*/



}