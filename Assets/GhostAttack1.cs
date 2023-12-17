using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAttack1 : MonoBehaviour
{
    public GameObject northGhostPrefab; // 북쪽 몬스터 프리팹
    public GameObject westGhostPrefab; // 서쪽 몬스터 프리팹

    public Transform northSpawnPoint; // 북쪽 몬스터 생성 위치
    public Transform westSpawnPoint; // 서쪽 몬스터 생성 위치
    public Transform eastSpawnPoint; // 동쪽 몬스터 생성 위치

    public float ghostMoveSpeed = 3f; // 몬스터 이동 속도
    public float ghostLifetime = 5f; // 몬스터 생존 시간
    public float skillCooldown = 10f; // 스킬 쿨다운 시간

    private bool isSkillActivated = false;
    private float nextSkillTime = 0f;

    private List<GameObject> activeNorthGhosts = new List<GameObject>();
    private List<GameObject> activeWestGhosts = new List<GameObject>();

    public delegate void SkillCompleted();
    public event SkillCompleted OnSkillCompleted;
    private BossController bossController;

    private void Start()
    {
        bossController = BossController.Instance;

        // 스킬 초기화 및 스킬 활성화 대기
        isSkillActivated = false;
        nextSkillTime = Time.time + skillCooldown;
    }

    private void Update()
    {
        if (isSkillActivated)
        {
            // 스킬이 활성화된 경우 몬스터 이동 처리
            MoveGhosts(activeNorthGhosts);
            MoveGhosts(activeWestGhosts);
        }
        else
        {
            // 스킬이 비활성화된 경우 스킬 쿨다운 체크
            if (Time.time >= nextSkillTime)
            {
                // 스킬 쿨다운이 끝나면 스킬 활성화
                ActivateSkill();
            }
        }
    }

    private void ActivateSkill()
    {
        isSkillActivated = true;
        nextSkillTime = Time.time + skillCooldown;

        // 북쪽에서 몬스터 생성
        SpawnGhosts(northGhostPrefab, northSpawnPoint, activeNorthGhosts);
        // 서쪽에서 몬스터 생성
        SpawnGhosts(westGhostPrefab, westSpawnPoint, activeWestGhosts);
    }

    private void SpawnGhosts(GameObject prefab, Transform spawnPoint, List<GameObject> activeGhosts)
    {
        // 몬스터 생성 및 초기화
        GameObject ghost = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        activeGhosts.Add(ghost);
        StartCoroutine(DestroyGhost(ghost, activeGhosts)); // 몬스터 제거 타이머 시작
    }

    private IEnumerator DestroyGhost(GameObject ghost, List<GameObject> activeGhosts)
    {
        yield return new WaitForSeconds(ghostLifetime);
        if (ghost != null)
        {
            Destroy(ghost);
        }
        activeGhosts.Remove(ghost);
    }

    private void MoveGhosts(List<GameObject> activeGhosts)
    {
        // 생성된 몬스터 이동
        foreach (var ghost in activeGhosts)
        {
            if (ghost != null)
            {
                // 몬스터를 화면 아래로 이동
                ghost.transform.Translate(Vector3.down * ghostMoveSpeed * Time.deltaTime);
            }
        }
    }

    public void ActivateEnhancedSkill()
    {
        // 강화된 스킬의 귀신 생성 및 이동 로직
        for (int i = 0; i < 3; i++)
        {
            var ghostNorth = Instantiate(northGhostPrefab, northSpawnPoint.position, Quaternion.identity);
            StartCoroutine(MoveGhostBackAndForth(ghostNorth, northSpawnPoint.position, eastSpawnPoint.position));

            var ghostEast = Instantiate(westGhostPrefab, eastSpawnPoint.position, Quaternion.identity);
            StartCoroutine(MoveGhostBackAndForth(ghostEast, eastSpawnPoint.position, northSpawnPoint.position));
        }
    }

    private IEnumerator MoveGhostBackAndForth(GameObject ghost, Vector3 startPosition, Vector3 endPosition)
    {
        // 귀신을 목적지까지 이동
        float journey = 0f;
        while (journey <= 1f)
        {
            journey += Time.deltaTime / ghostMoveSpeed;
            ghost.transform.position = Vector3.Lerp(startPosition, endPosition, journey);
            yield return null;
        }

        // 1초간 대기
        yield return new WaitForSeconds(1f);

        // 귀신을 시작 위치로 이동
        journey = 0f;
        while (journey <= 1f)
        {
            journey += Time.deltaTime / ghostMoveSpeed;
            ghost.transform.position = Vector3.Lerp(endPosition, startPosition, journey);
            yield return null;
        }

        // 귀신 제거
        if (ghost != null)
        {
            Destroy(ghost);
        }
    }

    public void ActivateRandomSkill()
    {
        if (bossController.currentBossHealth <= 50)
        {
            // 체력이 50 이하일 때 일반 스킬 실행
            Debug.Log("Activating Random Normal Skill");
            // 일반 스킬 실행 로직 구현
        }
        else
        {
            // 체력이 50보다 클 때 강화된 스킬 실행
            ActivateEnhancedSkill();
        }
    }

    
}
