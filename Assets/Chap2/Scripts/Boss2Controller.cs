using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boss2Controller : MonoBehaviour
{
    public GameObject cablePrefab; // �ν����Ϳ��� ������ �� �ִ� ���̺� ������
    public GameObject ballRainHitAreaPrefab;

    public GameObject transparentWall; // ���� ���� ���� ����
    private bool isEnhancedSkillActive = false; // ��ȭ ��ų Ȱ��ȭ ����
    private bool hasEnhancedSkillActivatedOnce = false; // ��ȭ ��ų�� �� ���̶� Ȱ��ȭ�Ǿ�����
    private bool isSkillActive = false; // ���� ��ų�� ���� ������ ���θ� ��Ÿ���� �÷���
    public GameObject[] EnhancedSpawnPoints; // ��ȭ ��ų�� ���� ���� ����Ʈ �迭


    public float attackDelay = 2f; // ���� ����
    public Transform player;
    //public float offsetToPlayer; // �÷��̾ ���� ������ ������
    public GameObject[] spawnPoints; // �߻� ��ġ �迭
    public float moveSpeed = 5f; // ������ �̵� �ӵ�
    public float cableSpawnDelay = 1f; // ���̺��� ������Ǳ������ ���� �ð�

    private float lastAttackTime;
    private int lastSpawnIndex = -1;

    public GameObject targetPoint; // Ÿ�� ����Ʈ ��ü
    public float offset = 10f; // ī�޶� ������ �������� ������

    // Ű���� ����
    public GameObject obstaclePrefab; // ��ֹ� ������
    public GameObject PophitAreaPrefab; // �ǰ� ���� ������
    public GameObject CablehitAreaPrefab; // �ǰ� ���� ������

    // ���� ��ġ ��
    public Transform fragmentSpawnPoint; // Ű���� ���� ��ġ ���� ��ġ
    public float fragmentSpawnDelay = 1.5f; // ��ų �ߵ� ���� �ð�
    public float fragmentRadius = 5f; // �÷��̾� �ֺ� Ű���� ������ �ݰ�

    public GameObject keyboardFragmentPrefab; // Ű���� ���� ��ġ ������

    public GameObject nonMotionSpritePrefab; // �� ��� ��������Ʈ ������

    public void RespawnCable()
    {
        StartCoroutine(SpawnCableAfterDelay());
    }

    // ���� �ð� �� ���̺� ������Ʈ�� �����ϴ� �ڷ�ƾ
    private IEnumerator SpawnCableAfterDelay()
    {
        yield return new WaitForSeconds(cableSpawnDelay);
        SpawnCable();
    }

    void Update()
    {
        if (Time.time > lastAttackTime + attackDelay && !isSkillActive)
        {
            // ��ȭ ��ų�� Ȱ��ȭ�� ���
            if (isEnhancedSkillActive)
            {
                ActivateEnhancedCableSkill();
                hasEnhancedSkillActivatedOnce = true;
                isEnhancedSkillActive = false;
            }
            else if (hasEnhancedSkillActivatedOnce)
            {
                // �������� �Ϲ� ��ų �Ǵ� ��ȭ ��ų ����
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
                // �Ϲ� ��ų ����
                ExecuteNormalSkillLogic();
            }
            lastAttackTime = Time.time;
        }
    }

    void ExecuteNormalSkillLogic()
    {
        // ��� ��ų ����
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
        // ī�޶� ����Ʈ�� ������ �� y�� ���߾ӿ� Ÿ�� ����Ʈ ��ġ ����
        Vector3 viewportPoint = new Vector3(1, 0.5f, offset); // offset�� ī�޶󿡼� Ÿ�� ����Ʈ������ �Ÿ�
        Vector3 targetPosition = Camera.main.ViewportToWorldPoint(viewportPoint);

        targetPoint.transform.position = targetPosition;
    }

    IEnumerator DelayedLaunchKeyboardFragment()
    {
        yield return new WaitForSeconds(fragmentSpawnDelay);

        // Ű���� ���� ��ġ ��ų �ߵ�
        LaunchKeyboardFragment();
    }

    void LaunchKeyboardFragment()
    {
        // ī�޶� �̵��� ���� ���� ��ġ�� ī�޶��� ���߾� ������� ����
        Vector3 cameraTopCenter = new Vector3(Camera.main.transform.position.x,
                                              Camera.main.transform.position.y + Camera.main.orthographicSize,
                                              Camera.main.transform.position.z);
        GameObject fragment = Instantiate(keyboardFragmentPrefab, cameraTopCenter, Quaternion.identity);

        // Ű���� ���� ��ġ�� ���� ���� �÷��̾� ��ġ�� �ǰ� ������ ǥ���ϰ� 1.5�� �Ŀ� ������� ��
        StartCoroutine(ShowHitAreaAndSpawnRainPrefab(player.transform.position));
    }



    IEnumerator ShowHitAreaAndSpawnRainPrefab(Vector3 targetPosition)
    {
        // ���ο� �ǰ� ���� ��������Ʈ ���
        GameObject hitArea = Instantiate(ballRainHitAreaPrefab, targetPosition, Quaternion.identity);
        yield return new WaitForSeconds(1.5f);
        Destroy(hitArea);

        // �ǰ� ���� ����� �� �� ������ ����
        Instantiate(nonMotionSpritePrefab, targetPosition, Quaternion.identity);
    }

    private void SpawnCable()
    {
        GameObject spawnPoint = ChooseSpawnPoint();
        GameObject newCable = Instantiate(cablePrefab, spawnPoint.transform.position, Quaternion.identity);
        cable cableScript = newCable.GetComponent<cable>();
        cableScript.Launch(Vector3.right, this.gameObject, this); // ���� �̵� ������ Vector3.right�� ����
    }

    // ���̺� ����
    void AttackPlayer()
    {
        //Debug.Log("���̺� ���� ����");

        GameObject spawnPoint = ChooseSpawnPoint();
        Vector3 hitPosition = spawnPoint.transform.position;

        // �ǰ� ���� ��������Ʈ ����
        GameObject hitArea = Instantiate(CablehitAreaPrefab, hitPosition, Quaternion.identity);
        if (hitArea != null)
        {
            //Debug.Log("�ǰ� ���� ��������Ʈ ������: " + hitArea.name);
        }
        else
        {
            //Debug.LogError("�ǰ� ���� ��������Ʈ ���� ����!");
        }

        // �ǰ� ���� ��������Ʈ�� ��Ÿ���� �ð� �Ŀ� ���̺� ����
        StartCoroutine(DelayedSpawnCable(spawnPoint, 0.5f));

        // �ǰ� ���� ��������Ʈ ����
        Destroy(hitArea, 0.5f);
    }

    IEnumerator DelayedSpawnCable(GameObject spawnPoint, float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject newCable = Instantiate(cablePrefab, spawnPoint.transform.position, Quaternion.identity);
        newCable.SetActive(false); // ���̺��� ��Ȱ��ȭ ���·� ����

        // �ʿ��� ���� �� ���̺� Ȱ��ȭ
        cable cableScript = newCable.GetComponent<cable>();
        cableScript.Launch(Vector3.right, this.gameObject, this); // ���� �̵� ������ Vector3.right�� ����
        newCable.SetActive(true); // ���̺� Ȱ��ȭ
    }




    // Ű���� ��ֹ� ����
    void LaunchObstacleAttack()
    {
        // �÷��̾��� ���� ��ġ�� �������� �ǰ� ���� ����
        Vector3 playerPosition = player.transform.position;

        for (int i = 0; i < 3; i++) // 3���� �ǰ� ���� ����
        {
            // �÷��̾� �ֺ��� ������ ��ġ ����
            Vector3 hitPosition = playerPosition + new Vector3(Random.Range(0f, 5f), 0f, Random.Range(-3f, 3f));

            GameObject hitArea = Instantiate(PophitAreaPrefab, hitPosition, Quaternion.identity);
            Destroy(hitArea, 0.5f); // 0.5�� �Ŀ� �ǰ� ���� ����

            StartCoroutine(SpawnObstacle(hitPosition, 1f)); // ��ֹ� ����

        }
    }


    System.Collections.IEnumerator SpawnObstacle(Vector3 position, float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject obstacle = Instantiate(obstaclePrefab, position, Quaternion.identity); // ��ֹ� ����
        SpriteRenderer renderer = obstacle.GetComponent<SpriteRenderer>(); // �Ǵ� MeshRenderer

        // 1�� ���� ���̵� �ƿ�
        float fadeDuration = 3f;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            Color color = renderer.material.color;
            color.a = Mathf.Lerp(1, 0, t / fadeDuration);
            renderer.material.color = color;
            yield return null;
        }

        Destroy(obstacle); // ��ֹ� �ı�
    }

    GameObject ChooseSpawnPoint()
    {
        lastSpawnIndex = (lastSpawnIndex + 1) % spawnPoints.Length;
        return spawnPoints[lastSpawnIndex];
    }

    
    // ���̺�� �΋H���� �� ������ �÷��̾�� �Ÿ� ������ ����
    public void OnCableHit()
    {
        // �÷��̾�� ������ �Ÿ��� �����ϴ� ����
        // ����: ������ �÷��̾�� ���� �� ������ �̵���ŵ�ϴ�.
        Vector3 newPosition = transform.position;
        newPosition.x = Mathf.Lerp(newPosition.x, player.position.x, 0.1f); // X������ ���� �̵�
        transform.position = newPosition;
    }

    // �ð��� ��ȭ ��ų

    // ���� �� �ݶ��̴� ���� ���� ������ ��ȭ��ų ����
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
        isSkillActive = true; // ��ų ���� �� ǥ��

        // ��ȭ ��ų ���� ����Ʈ �� �������� 2�� ����
        List<int> selectedIndexes = new List<int>();
        while (selectedIndexes.Count < 2)
        {
            int randomIndex = Random.Range(0, EnhancedSpawnPoints.Length);
            if (!selectedIndexes.Contains(randomIndex))
            {
                selectedIndexes.Add(randomIndex);
            }
        }

        // ���õ� ���� ����Ʈ���� ���̺� �߻�
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
    cableScript.Launch(Vector3.right, this.gameObject, this); // ���� �̵� ������ Vector3.right�� ����

    }


    void CreateHitAreaAt(Vector3 position)
    {
        // �ǰ� ���� ��������Ʈ ����
        GameObject hitArea = Instantiate(CablehitAreaPrefab, position, Quaternion.identity);
        Destroy(hitArea, 0.5f); // 0.5�� �Ŀ� �ǰ� ���� ����
    }
    


    
    IEnumerator ResetSkillActiveFlag()
    {
        // ��ų ���� �� ����
        yield return new WaitForSeconds(1f);
        isSkillActive = false;

        // �ӽ÷� ������ targetDummy ��ü�� �ִٸ� �ı�
        GameObject targetDummy = GameObject.Find("TargetDummy");
        if (targetDummy != null)
        {
            Destroy(targetDummy);
        }
    }



    Vector3 GetRandomPosition()
    {
        // ȭ�� ������ ������ ��ġ ���
        float randomX = Random.Range(0f, Screen.width);
        float randomY = Random.Range(0f, Screen.height);
        Vector3 randomScreenPosition = new Vector3(randomX, randomY, 0);

        // ��ũ�� ��ǥ�� ���� ��ǥ�� ��ȯ
        Vector3 randomWorldPosition = Camera.main.ScreenToWorldPoint(randomScreenPosition);
        return randomWorldPosition;
    }


}