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
    private bool westGhostsSpawnCompleted = false; // ���� ���� ���� �Ϸ� ����

    private bool northEnhancedSpawned = false;
    private bool skillTimerStarted = false;

    public Transform eastEndPosition;
    public Transform northEndPosition;
    public float westGhostSpacing = 1.0f;

    //��ų �Ϸ� ����
    public delegate void SkillCompleted();
    public event SkillCompleted OnNormalSkillCompleted;
    public event SkillCompleted OnEnhancedSkillCompleted;


    public delegate void AttackCompletedHandler();
    public event AttackCompletedHandler AttackCompleted;
    public float ghostMoveSpeed = 15f;

    public List<Transform> northSpawnPoints; // ���� ���� ���� ��ġ ����Ʈ
    public List<Transform> westSpawnPoints; // ���� ���� ���� ��ġ ����Ʈ
    public List<Transform> eastSpawnPoints; // ���� ���� ���� ��ġ ����Ʈ

    private BossController bossController;

    private int currentSkillGroup = 0; // ���� ���� ���� ��ų �׷� �ε���

    private int westGhostIndex = 0; // ���� ���� ���� ��ġ �ε���
    //private bool northSkillActivated = false; // ���� ��ų�� Ȱ��ȭ�Ǿ����� ����

    private bool isFirstSkill = true; // ó�� ��ų�� ��Ÿ���� ����

    private float enhancedGhostMoveSpeed = 10.0f; // EnhancedNorthGhosts ���� �̵� �ӵ�
                                                  // ��ȭ�� ���� ��Ʈ ���� ��ġ ����Ʈ
    public List<Transform> enhancedNorthSpawnPoints;
    public bool normalSkillCompleted = false;  // �Ϲ� ��ų �������� Ȯ��
    public float timeBetweenSkills = 4.0f; // ���ʰ� ���� ��ų ���� ��� �ð� (�� ����)
    // ��ȭ�� ���� ��Ʈ ���� ��ġ ����Ʈ
    public List<Transform> enhancedEastSpawnPoints;

    public List<Transform> northMonsterSpawnPoints; // ���� ���� ���� ��ġ ����Ʈ
    public List<Transform> westMonsterSpawnPoints; // ���� ���� ���� ��ġ ����Ʈ

    public List<Transform> northHitRangeSpawnPoints; // ���� �ǰ� ���� ���� ��ġ ����Ʈ
    public List<Transform> westHitRangeSpawnPoints; // ���� �ǰ� ���� ���� ��ġ ����Ʈ
    public GameObject northHitRangePrefab;
    public GameObject westHitRangePrefab;

    public bool skillActivated = false;
    private bool skillInProgress = false; // ��ų ���� ���¸� ��Ÿ���� �÷���
                                          // �ǰ� ������ ������ ī�޶� ��ġ�� ������ �ʵ� �߰�
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


        /*// ���� ���� ����Ʈ�� ī�޶��� �ڽ����� �����ϰ� ����� ��ġ�� ����
        northSpawnPoint.SetParent(mainCamera.transform, false);
        northSpawnPoint.localPosition = new Vector3(0, 10, 15); // ī�޶�κ��� ���� 10, ������ 15�� ��ġ

        // ���� ���� ����Ʈ�� ī�޶��� �ڽ����� �����ϰ� ����� ��ġ�� ����
        westSpawnPoint.SetParent(mainCamera.transform, false);
        westSpawnPoint.localPosition = new Vector3(-10, 0, 15); // ī�޶�κ��� �������� 10, ������ 15�� ��ġ*/

        // ī�޶��� �ڽ����� �ǰ� ���� ���� ����Ʈ ����
        foreach (Transform spawnPoint in northHitRangeSpawnPoints)
        {
            spawnPoint.SetParent(mainCamera.transform, false);
            // ���⿡ ����� ��ġ ����
            //spawnPoint.localPosition = new Vector3(0, 10, 15);
        }

        // ���� �ǰ� ���� ���� ����Ʈ�� ���ؼ��� �����ϰ� ����
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
                // ü���� 50 �̻��� ���� ��ų Ȱ��ȭ ����
                StartCoroutine(ActivateNormalSkill());
                skillActivated = true; // ��ų�� Ȱ��ȭ�Ǿ����� ��Ÿ��
            }
            else
            {
                StartCoroutine(ActivateNormalSkill());
                skillActivated = true;

            }
        }

    }




    // �ǰ� ���� �޼ҵ�
    private IEnumerator ShowHitRangeAndSpawnGhosts(List<Transform> hitRangeSpawnPoints, IEnumerator spawnGhostsCoroutine, GameObject hitRangePrefab)
    {
        // �ǰ� ���� ǥ��
        foreach (var spawnPoint in hitRangeSpawnPoints)
        {
            ShowHitRangeAt(spawnPoint, hitRangePrefab);
        }

        // �ǰ� ���� ǥ�� �� ���� �ð� ��ٸ�
        yield return new WaitForSeconds(0.5f);

        // ��Ʈ ����
        yield return StartCoroutine(spawnGhostsCoroutine);
    }

    // �ǰ� ���� ��ġ�� ������ ����Ʈ
    private List<Vector3> hitRangePositions = new List<Vector3>();

    // �ǰ� ���� ���� ���� ����
    private void ShowHitRangeAt(Transform spawnPoint, GameObject hitRangePrefab)
    {
        Vector3 hitRangePosition = mainCamera.transform.TransformPoint(spawnPoint.localPosition);
        hitRangePositions.Add(hitRangePosition); // ��ġ ����
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

        float maxAlpha = 0.4f; // �ִ� ���� ��
        float duration = 0.5f;

        // ���̵� ��
        for (float t = 0; t < maxAlpha; t += Time.deltaTime / duration)
        {
            Color newColor = renderer.color;
            newColor.a = t;
            renderer.color = newColor;
            yield return null;
        }

        // ����
        yield return new WaitForSeconds(0.5f);

        // ���̵� �ƿ�
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
        // ��� ��Ʈ�� ȭ�� ������ �������� Ȯ��
        foreach (GameObject ghost in ghosts)
        {
            if (ghost != null)
            {
                Vector3 screenPosition = mainCamera.WorldToViewportPoint(ghost.transform.position);

                // ȭ�� ������ ������ ������ �ʾ����� false ��ȯ
                if (screenPosition.x > 0 && screenPosition.x < 1 && screenPosition.y > 0 && screenPosition.y < 1)
                {
                    return false;
                }
            }
        }

        // ��� ��Ʈ�� ȭ�� ������ �������� true ��ȯ
        return true;
    }













    // ���͸� �����ϰ� �̵���Ŵ
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
    // �������� ���� ī�޶� �̵�������
    // ���� ���� ���� ����
    // ��Ʈ ���� ���� ����
    private IEnumerator SpawnGhosts(List<GameObject> list, GameObject prefab, int numberOfGhosts, Vector3 direction)
    {
        if (hitRangePositions.Count == 0)
        {
            yield break; // �ǰ� ���� ��ġ�� ������ �Լ� ����
        }

        foreach (var hitRangePosition in hitRangePositions)
        {
            GameObject ghost = Instantiate(prefab, hitRangePosition, Quaternion.identity);
            list.Add(ghost);

            // ��Ʈ �̵� ���� ...
            StartCoroutine(MoveGhost(ghost, direction, ghostMoveSpeed));

            yield return new WaitForSeconds(spawnDelay); // �̵� ���� �� ���� �ð� ���
        }
        hitRangePositions.Clear(); // ���� �� ����� ���
    }



    /*private IEnumerator SpawnNorthGhosts()
    {
        int numberOfGroups = 1;
        float xOffsetMultiplier = 2.0f; // X�� ������ ������ ����

        for (int j = 0; j < numberOfGroups; j++)
        {
            List<Transform> spawnPoints = new List<Transform>();

            // �ǰ� ������ ��� ���� ������ ���� �� ���� ǥ��
            foreach (Transform spawnPoint in northSpawnPoints)
            {

                ShowHitRangeAt(spawnPoint.position, northHitRangePrefab);
            }

            // �ǰ� ���� ǥ�� �� ���� �ð� ��ٸ�
            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < numberOfGhostsPerGroup; i++)
            {
                int xOffset = i * (int)(xOffsetMultiplier); // X�� ������ xOffset���� ����
                int yOffset = 0; // Y�� ������ 0���� �����Ͽ� ���� ������ ����

                spawnPoints.Add(northSpawnPoints[i]); // ���� ������ ����Ʈ�� �߰�
            }
            yield return new WaitForSeconds(0.5f);

            yield return SpawnGhosts(spawnPoints, Vector3.down, ghost1Prefab, northGhosts, numberOfGhostsPerGroup);
            //yield return new WaitForSeconds(spawnDelay);
        }
        Debug.Log("Spawned north ghosts");
    }*/
    // ���� ��Ʈ ���� ���� ����
    private IEnumerator SpawnNorthGhosts()
    {

        hitRangePositions.Clear(); // ����Ʈ �ʱ�ȭ

        // �ǰ� ������ ��� ���� ������ ���� �� ���� ǥ��
        foreach (Transform spawnPoint in northHitRangeSpawnPoints)
        {
            ShowHitRangeAt(spawnPoint, northHitRangePrefab);
        }

        // �ǰ� ���� ǥ�� �� ���� �ð� ��ٸ�
        yield return new WaitForSeconds(0.5f);

        // ��Ʈ ����
        yield return StartCoroutine(SpawnGhosts(northGhosts, ghost1Prefab, 1, Vector3.down));
    }






    /*private IEnumerator SpawnWestGhosts()
    {
        *//*if (1)
        {*//*
        westSpawned = true; // ���� ���� �׷� ���� ���۵��� ǥ��
        for (int i = 0; i < numberOfGhostsPerGroup; i++)
            {
                Transform spawnPoint = westSpawnPoints[westGhostIndex];

                ShowHitRangeAt(spawnPoint.position, westHitRangePrefab);

                yield return new WaitForSeconds(0.5f); // �ǰ� ���� ǥ�� �ð�

                // ���� ����
                List<Transform> spawnPoints = new List<Transform> { spawnPoint };
                yield return SpawnGhosts(spawnPoints, Vector3.right, ghost2Prefab, westGhosts, numberOfGhostsPerGroup);

                westGhostIndex = (westGhostIndex + 1) % westSpawnPoints.Count;

                // ������ ���Ͱ� �ƴ� ���, ���� ���� ���� �� ������
                if (i < numberOfGhostsPerGroup - 1)
                {
                    yield return new WaitForSeconds(spawnDelay);
                }
            }
        *//*}*//*

        // ��� ���� ���Ͱ� ���� �Ϸ�Ǿ����� ��Ÿ���� �÷��� ����
        //yield return new WaitForSeconds(1f);
        westGhostsSpawnCompleted = true;
    }*/

    // SpawnWestGhosts �Լ��� �����ε� ����
    private IEnumerator SpawnWestGhosts()
    {
        westSpawned = true;

        for (int i = 0; i < 3; i++)
        {
            Transform spawnPoint = westSpawnPoints[i % westSpawnPoints.Count];
            ShowHitRangeAt(spawnPoint, westHitRangePrefab);

            // �ǰ� ���� ǥ�� �� ��Ʈ ����
            yield return new WaitForSeconds(0.5f); // �ǰ� ���� ǥ�� �ð�

            GameObject ghost = Instantiate(ghost2Prefab, hitRangePositions[i], Quaternion.identity);
            westGhosts.Add(ghost);

            // ��Ʈ �̵�
            StartCoroutine(MoveGhost(ghost, Vector3.right, ghostMoveSpeed));

            // ���� ��Ʈ ���� ���� ���� �ð� ���
            yield return new WaitForSeconds(spawnDelay);
        }

        westGhostsSpawnCompleted = true;
    }








    // ���� �̵� ����
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

        // ��Ʈ�� ȭ�� ������ �����ٸ� ���� ��ġ�� �ǵ���
        //ghost.transform.position = startPosition;
    }

    private Dictionary<GameObject, Vector3> ghostSpawnPositions = new Dictionary<GameObject, Vector3>();

    private IEnumerator EnhancedNorthGhosts()
    {
        List<GameObject> groupGhosts = new List<GameObject>();

        // ��ȭ�� ���� ��Ʈ ���� �� ����
        foreach (Transform spawnPoint in enhancedNorthSpawnPoints)
        {
            for (int i = 0; i < numberOfGhostsPerType; i++)
            {
                GameObject ghost = Instantiate(ghost1Prefab, spawnPoint.position, Quaternion.identity);
                groupGhosts.Add(ghost);
                ghostSpawnPositions.Add(ghost, spawnPoint.position); // ���� ��ġ ����

                StartCoroutine(MoveGhost(ghost, Vector3.down, enhancedGhostMoveSpeed));
            }
        }

        // ��� ��Ʈ�� ȭ�� ������ �̵��� ������ ���
        yield return StartCoroutine(MoveGroupOfGhosts(groupGhosts, Vector3.down));

        // 1�� ���
        yield return new WaitForSeconds(1.0f);

        // �� ��Ʈ�� ��� ���� ��ġ�� �ǵ���
        foreach (GameObject ghost in groupGhosts)
        {
            if (ghost != null && ghostSpawnPositions.ContainsKey(ghost))
            {
                Vector3 spawnPosition = ghostSpawnPositions[ghost];
                ghost.transform.position = spawnPosition; // ��Ʈ ��ġ ��� ������Ʈ
            }
        }
    }

    private IEnumerator EnhancedEastGhosts()
    {
        List<GameObject> groupGhosts = new List<GameObject>();

        // ��ȭ�� ���� ��Ʈ ���� �� ����
        foreach (Transform spawnPoint in enhancedEastSpawnPoints)
        {
            for (int i = 0; i < numberOfGhostsPerType; i++)
            {
                GameObject ghost = Instantiate(ghost2Prefab, spawnPoint.position, Quaternion.identity);
                groupGhosts.Add(ghost);
                ghostSpawnPositions.Add(ghost, spawnPoint.position); // ���� ��ġ ����

                StartCoroutine(MoveGhost(ghost, Vector3.left, enhancedGhostMoveSpeed)); // ���ʿ��� �������� �̵�
            }
        }

        // ��� ��Ʈ�� ȭ�� ������ �̵��� ������ ���
        yield return StartCoroutine(MoveGroupOfGhosts(groupGhosts, Vector3.left));

        // 1�� ���
        yield return new WaitForSeconds(1.5f);

        // �� ��Ʈ�� ��� ���� ��ġ�� �ǵ���
        foreach (GameObject ghost in groupGhosts)
        {
            if (ghost != null && ghostSpawnPositions.ContainsKey(ghost))
            {
                Vector3 spawnPosition = ghostSpawnPositions[ghost];
                ghost.transform.position = spawnPosition; // ��Ʈ ��ġ ��� ������Ʈ
            }
        }
    }



    private IEnumerator MoveGroupOfGhosts(List<GameObject> ghosts, Vector3 direction)
    {
        float startTime = Time.time; // startTime�� �� ���� ����

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

                    // �ð� ����� �����Ͽ� �̵� �ӵ� ����
                    float elapsedTime = Time.time - startTime;
                    float duration = moveDistance / ghostMoveSpeed; // �̵��� �ʿ��� �� �ð�
                    float fractionOfJourney = elapsedTime / duration; // ��ü �̵� �ð��� ���� ���� ��� �ð��� ����

                    // fractionOfJourney�� 1�� �ʰ����� �ʵ��� ����
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







    // ��ȭ�� ��ų �Ϸ� �� ȣ��� ���� �޼���
    private void OnEnhancedSkillCompletedInternal()
    {
        OnEnhancedSkillCompleted?.Invoke();
    }


    /*public IEnumerator ActivateEnhancedSkill()
    {
        

        // ��ȭ�� ���� ��Ʈ ��ų Ȱ��ȭ
        //yield return StartCoroutine(EnhancedNorthGhosts());

        // ��ȭ�� ���� ��Ʈ ��ų Ȱ��ȭ
        *//*yield return StartCoroutine(SpawnNorthGhosts());

        skillActivated = false;
        AllGhostsOutOfScreen(northGhosts);
        Debug.Log("All enhanced skill groups completed, spawning next boss...");
        // ��ȭ�� ��ų �Ϸ� �̺�Ʈ ȣ��
        OnEnhancedSkillCompletedInternal();*//*
    }*/


    // �Ϲ� ��ų ȣ�� �޼���

    public IEnumerator ActivateNormalSkill()
    {


        /*if (skillActivated)
        {
            // �̹� ��ų�� Ȱ��ȭ�� ���, ���� �ð� ��ٸ� �� �ٽ� �õ�
            Debug.Log("�̹� ��ų Ȱ��ȭ ��. �����");
            yield return new WaitForSeconds(skillActivationTime);
        }*/
        skillInProgress = true; // ��ų�� ���۵� �� �÷��׸� true�� ����


        Debug.Log("GHOST Skill activated.");

        if (bossController.currentBossHealth > 50 || bossController.currentBossHealth < 50)
        {
            // ���� ���� ����
            if (!northSpawned)
            {
                StartCoroutine(SpawnNorthGhosts());
                northSpawned = true;
                yield return new WaitUntil(() => AllGhostsOutOfScreen(northGhosts));
                Debug.Log("���� ��Ʈ �ƿ�");
            }

            // ���� ���Ͱ� ��� ȭ�� ������ ���� �� ���� ���� ����
            if (!westSpawned)
            {
                yield return new WaitForSeconds(timeBetweenSkills); // ���� ���� ���� �� ���
                StartCoroutine(SpawnWestGhosts());
                yield return new WaitUntil(() => westGhostsSpawnCompleted && AllGhostsOutOfScreen(westGhosts));
                Debug.Log("���� ��Ʈ �ƿ�");
                yield return new WaitForSeconds(timeBetweenSkills); // ���� ���� ���� �Ϸ� �� �ణ ���
                yield return new WaitForSeconds(1f); // ���� ���� ���� �Ϸ� �� �ణ ���

            }
        }

        // ���� ���Ͱ� ��� ȭ�� ������ ���� ���� Ȯ���� �� �̺�Ʈ �߻�
        // ��ų �Ϸ� Ȯ��
        yield return new WaitUntil(() => AllGhostsOutOfScreen(westGhosts));

        Debug.Log("Skill completed.");
        skillInProgress = false;

        OnNormalSkillCompletedInternal();

    }


    // �Ϲ� ��ų �Ϸ� �� ȣ��� ���� �޼���
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