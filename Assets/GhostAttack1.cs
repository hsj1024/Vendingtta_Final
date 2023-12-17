using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAttack1 : MonoBehaviour
{
    public GameObject northGhostPrefab; // ���� ���� ������
    public GameObject westGhostPrefab; // ���� ���� ������

    public Transform northSpawnPoint; // ���� ���� ���� ��ġ
    public Transform westSpawnPoint; // ���� ���� ���� ��ġ
    public Transform eastSpawnPoint; // ���� ���� ���� ��ġ

    public float ghostMoveSpeed = 3f; // ���� �̵� �ӵ�
    public float ghostLifetime = 5f; // ���� ���� �ð�
    public float skillCooldown = 10f; // ��ų ��ٿ� �ð�

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

        // ��ų �ʱ�ȭ �� ��ų Ȱ��ȭ ���
        isSkillActivated = false;
        nextSkillTime = Time.time + skillCooldown;
    }

    private void Update()
    {
        if (isSkillActivated)
        {
            // ��ų�� Ȱ��ȭ�� ��� ���� �̵� ó��
            MoveGhosts(activeNorthGhosts);
            MoveGhosts(activeWestGhosts);
        }
        else
        {
            // ��ų�� ��Ȱ��ȭ�� ��� ��ų ��ٿ� üũ
            if (Time.time >= nextSkillTime)
            {
                // ��ų ��ٿ��� ������ ��ų Ȱ��ȭ
                ActivateSkill();
            }
        }
    }

    private void ActivateSkill()
    {
        isSkillActivated = true;
        nextSkillTime = Time.time + skillCooldown;

        // ���ʿ��� ���� ����
        SpawnGhosts(northGhostPrefab, northSpawnPoint, activeNorthGhosts);
        // ���ʿ��� ���� ����
        SpawnGhosts(westGhostPrefab, westSpawnPoint, activeWestGhosts);
    }

    private void SpawnGhosts(GameObject prefab, Transform spawnPoint, List<GameObject> activeGhosts)
    {
        // ���� ���� �� �ʱ�ȭ
        GameObject ghost = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        activeGhosts.Add(ghost);
        StartCoroutine(DestroyGhost(ghost, activeGhosts)); // ���� ���� Ÿ�̸� ����
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
        // ������ ���� �̵�
        foreach (var ghost in activeGhosts)
        {
            if (ghost != null)
            {
                // ���͸� ȭ�� �Ʒ��� �̵�
                ghost.transform.Translate(Vector3.down * ghostMoveSpeed * Time.deltaTime);
            }
        }
    }

    public void ActivateEnhancedSkill()
    {
        // ��ȭ�� ��ų�� �ͽ� ���� �� �̵� ����
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
        // �ͽ��� ���������� �̵�
        float journey = 0f;
        while (journey <= 1f)
        {
            journey += Time.deltaTime / ghostMoveSpeed;
            ghost.transform.position = Vector3.Lerp(startPosition, endPosition, journey);
            yield return null;
        }

        // 1�ʰ� ���
        yield return new WaitForSeconds(1f);

        // �ͽ��� ���� ��ġ�� �̵�
        journey = 0f;
        while (journey <= 1f)
        {
            journey += Time.deltaTime / ghostMoveSpeed;
            ghost.transform.position = Vector3.Lerp(endPosition, startPosition, journey);
            yield return null;
        }

        // �ͽ� ����
        if (ghost != null)
        {
            Destroy(ghost);
        }
    }

    public void ActivateRandomSkill()
    {
        if (bossController.currentBossHealth <= 50)
        {
            // ü���� 50 ������ �� �Ϲ� ��ų ����
            Debug.Log("Activating Random Normal Skill");
            // �Ϲ� ��ų ���� ���� ����
        }
        else
        {
            // ü���� 50���� Ŭ �� ��ȭ�� ��ų ����
            ActivateEnhancedSkill();
        }
    }

    
}
