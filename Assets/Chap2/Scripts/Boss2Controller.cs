using UnityEngine;
using System.Collections;

public class Boss2Controller : MonoBehaviour
{
    public GameObject cablePrefab; // �ν����Ϳ��� ������ �� �ִ� ���̺� ������


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

        // ���� �ð��� ������ ���� �ð� + ���� �ð����� ū ��쿡�� ������ ����
        if (Time.time > lastAttackTime + attackDelay)
        {
            if (Random.value > 0.5f) // 50% Ȯ���� ���̺� ���� ����
            {
                //Debug.Log("���̺� ����");
                AttackPlayer(); // ���̺� ���� ����

            }
            else
            {
                LaunchObstacleAttack(); // ��ֹ� ���� ����

            }
            lastAttackTime = Time.time; // ������ ���� �ð� ������Ʈ
        }

        UpdateTargetPointPosition();

    }
    private void UpdateTargetPointPosition()
    {
        // ī�޶� ����Ʈ�� ������ �� y�� ���߾ӿ� Ÿ�� ����Ʈ ��ġ ����
        Vector3 viewportPoint = new Vector3(1, 0.5f, offset); // offset�� ī�޶󿡼� Ÿ�� ����Ʈ������ �Ÿ�
        Vector3 targetPosition = Camera.main.ViewportToWorldPoint(viewportPoint);

        targetPoint.transform.position = targetPosition;
    }


    private void SpawnCable()
    {
        GameObject spawnPoint = ChooseSpawnPoint();
        GameObject newCable = Instantiate(cablePrefab, spawnPoint.transform.position, Quaternion.identity);
        cable cableScript = newCable.GetComponent<cable>();
        cableScript.Launch(targetPoint, spawnPoint, this); // �ùٸ� returnPoint�� ����
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
        cableScript.Launch(targetPoint, spawnPoint, this);
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

    /*Vector3 CalculateTargetPosition()
    {
        Vector3 playerPosition = player.transform.position;
        float yOffset;

        switch (player.GetComponent<PlayerController>().playerPosition)
        {
            case PlayerController.PlayerPosition.Top:
                yOffset = 1.0f; // ��ܺ� ��ġ�� Y ������
                break;
            case PlayerController.PlayerPosition.Middle:
                yOffset = 0.0f; // �߰��� ��ġ�� Y ������
                break;
            case PlayerController.PlayerPosition.Bottom:
                yOffset = -1.0f; // �ϴܺ� ��ġ�� Y ������
                break;
            default:
                yOffset = 0.0f; // �⺻���� �߰��η� ����
                break;
        }

        return new Vector3(playerPosition.x, playerPosition.y + yOffset, playerPosition.z);
    }*/
    // ���̺�� �΋H���� �� ������ �÷��̾�� �Ÿ� ������ ����
    public void OnCableHit()
    {
        // �÷��̾�� ������ �Ÿ��� �����ϴ� ����
        // ����: ������ �÷��̾�� ���� �� ������ �̵���ŵ�ϴ�.
        Vector3 newPosition = transform.position;
        newPosition.x = Mathf.Lerp(newPosition.x, player.position.x, 0.1f); // X������ ���� �̵�
        transform.position = newPosition;
    }
}