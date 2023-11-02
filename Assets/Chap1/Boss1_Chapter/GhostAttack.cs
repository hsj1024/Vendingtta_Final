using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GhostAttack : MonoBehaviour
{
    public GameObject ghost1Prefab;
    public GameObject ghost2Prefab;
    public Transform player;

    public Transform westSpawnPoint;
    public Transform northSpawnPoint;
    public Transform eastSpawnPoint;

    public int numberOfGhostsPerType = 3;
    public int numberOfGhostsPerGroup = 3;
    public float yOffset = 1f;
    public float spawnDelay = 1.0f;

    public bool skillActivated = false;
    public float skillActivationTime = 10.0f;
    public float ghostPauseTime = 1.0f;

    private Camera mainCamera;
    private List<GameObject> northGhosts = new List<GameObject>();
    private List<GameObject> westGhosts = new List<GameObject>();
    private List<GameObject> eastEnhancedGhosts = new List<GameObject>();
    private List<GameObject> northEnhancedGhosts = new List<GameObject>();
    private bool westSpawned = false;
    private bool northEnhancedSpawned = false;
    private bool skillTimerStarted = false;
    private bool skillActivatedOnce = false;

    public Transform eastEndPosition;
    public Transform northEndPosition;
    public float westGhostSpacing = 1.0f; // 서쪽 유령 간격 설정
    public delegate void SkillCompleted();
    public event SkillCompleted OnSkillCompleted;
    public delegate void AttackCompletedHandler();
    public event AttackCompletedHandler AttackCompleted;
    private void Start()
    {
        mainCamera = Camera.main;
        StartCoroutine(SpawnNorthGhosts());
    }
    
    private void Update()
    {
        if (!westSpawned && AllGhostsOutOfScreen(northGhosts))
        {
            StartCoroutine(SpawnWestGhosts());
            westSpawned = true;
        }

        if (westSpawned && !skillActivated && AllGhostsOutOfScreen(westGhosts) && AllGhostsOutOfScreen(northGhosts) && !skillTimerStarted)
        {
            ActivateSkill();
        }

        if (skillActivated && !northEnhancedSpawned)
        {
            SpawnEnhancedGhosts(northSpawnPoint, Vector3.down, northEnhancedGhosts, northEndPosition);
            northEnhancedSpawned = true;
        }
        if (skillActivated && AllGhostsOutOfScreen(eastEnhancedGhosts) && AllGhostsOutOfScreen(northEnhancedGhosts) && AllGhostsOutOfScreen(westGhosts) && AllGhostsOutOfScreen(northGhosts))
        {
            skillActivated = false;
            Debug.Log("Skill completed, spawning next boss...");
            OnSkillCompletedInternal();
            

        }





    }

    private bool AllGhostsOutOfScreen(List<GameObject> ghosts)
    {
        foreach (GameObject ghost in ghosts)
        {
            if (ghost != null)
            {
                Vector3 screenPosition = mainCamera.WorldToViewportPoint(ghost.transform.position);
                if (screenPosition.x >= 0 && screenPosition.x <= 1 && screenPosition.y >= 0 && screenPosition.y <= 1)
                {
                    Debug.Log("Ghost is on screen: " + ghost.name);

                    return false;
                    
                }
            }
        }
        Debug.Log("All ghosts are out of screen");

        return true;
      

    }

    private void ActivateSkill()
    {
        //skillActivated = true;
        StartCoroutine(EnhancedGhostSpawn());
        skillTimerStarted = true;
        

    }



    private IEnumerator SpawnWestGhosts()
    {
        for (int i = 0; i < numberOfGhostsPerGroup; i++)
        {
            yield return SpawnGhosts(westSpawnPoint, Vector3.right, ghost2Prefab, westGhosts, 1);
            yield return new WaitForSeconds(spawnDelay); // 각 유령이 나온 후에 대기
        }
    }


    private IEnumerator SpawnNorthGhosts()
    {
        yield return SpawnGhosts(northSpawnPoint, Vector3.down, ghost1Prefab, northGhosts, numberOfGhostsPerGroup);
    }

    private IEnumerator SpawnGhosts(Transform spawnPoint, Vector3 direction, GameObject prefab, List<GameObject> list, int numberOfGhosts)
    {
        Vector3 spawnPosition = spawnPoint.position - Vector3.up * (numberOfGhosts - 1) * yOffset / 2;

        for (int i = 0; i < numberOfGhosts; i++)
        {
            GameObject ghost = Instantiate(prefab, spawnPosition + Vector3.up * i * yOffset, Quaternion.identity);
            list.Add(ghost);

            GhostAI ghostAI = ghost.GetComponent<GhostAI>();
            ghostAI.SetTarget(player);
            ghostAI.SetMoveDirection(direction);

            yield return new WaitForSeconds(spawnDelay);
        }
    }





    private IEnumerator EnhancedGhostSpawn()
    {
        if (skillActivatedOnce)
            yield break;

        yield return new WaitForSeconds(skillActivationTime);

        SpawnEnhancedGhosts(eastSpawnPoint, Vector3.left, eastEnhancedGhosts, eastEndPosition);
        SpawnEnhancedGhosts(northSpawnPoint, Vector3.down, northEnhancedGhosts, northEndPosition);

        skillActivatedOnce = true;
    }



    private void SpawnEnhancedGhosts(Transform spawnPoint, Vector3 direction, List<GameObject> ghostList, Transform endPoint)
    {
        Debug.Log("enhanced Ghost");
        for (int i = 0; i < numberOfGhostsPerGroup; i++)
        {
            Vector3 spawnPosition = spawnPoint.position + Vector3.up * (i * yOffset);
            GameObject ghost = Instantiate(ghost1Prefab, spawnPosition, Quaternion.identity);
            ghostList.Add(ghost);

            GhostAI ghostAI = ghost.GetComponent<GhostAI>();
            ghostAI.SetMoveDirection(direction);
            StartCoroutine(EnhancedGhostBehavior(ghost, endPoint));

        }
    }

    private IEnumerator EnhancedGhostBehavior(GameObject ghost, Transform endPoint)
    {
        GhostAI ghostAI = ghost.GetComponent<GhostAI>();
        Vector3 originalPosition = ghost.transform.position; // 원래 위치 저장

        while (Vector3.Distance(ghost.transform.position, endPoint.position) > 0.1f)
        {
            ghost.transform.position = Vector3.MoveTowards(ghost.transform.position, endPoint.position, ghostAI.moveSpeed * Time.deltaTime);
            yield return null;
        }

        ghostAI.PauseMovement();
        yield return new WaitForSeconds(ghostPauseTime);
        ghostAI.ResumeMovement();

        while (Vector3.Distance(ghost.transform.position, originalPosition) > 0.1f)
        {
            ghost.transform.position = Vector3.MoveTowards(ghost.transform.position, originalPosition, ghostAI.moveSpeed * Time.deltaTime);
            yield return null;
        }
        
        ghostAI.SetTarget(player); // 유령이 플레이어를 향하게 설정
        skillActivated = true;
        

    }

    private bool AllGhostsAtOriginalPosition(List<GameObject> enhancedGhosts)
    {
        foreach (GameObject ghost in enhancedGhosts)
        {
            if (ghost != null)
            {
                GhostAI ghostAI = ghost.GetComponent<GhostAI>();
                if (!ghostAI.AtOriginalPosition())
                {
                    return false;
                }
            }
        }
        return true;
    }



    private void OnSkillCompletedInternal()
    {

        OnSkillCompleted?.Invoke();
        //skillActivated = false; // 스킬 비활성화
    }
}
