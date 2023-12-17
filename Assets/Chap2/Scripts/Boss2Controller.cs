using UnityEngine;

public class Boss2Controller : MonoBehaviour
{
    
    public GameObject cable; // ���̺� ������
    public float attackDelay = 2f; // ���� ����
    public Transform player;
    public float offsetToPlayer; // �÷��̾ ���� ������ ������
    public GameObject[] spawnPoints; // �߻� ��ġ �迭
    public float moveSpeed = 5f; // ������ �̵� �ӵ�

    private float lastAttackTime;
    private int lastSpawnIndex = -1;


    void Update()
    {
        // ���� ����
        if (Time.time > lastAttackTime + attackDelay)
        {
            AttackPlayer();
            lastAttackTime = Time.time;
        }
    }


    void FollowPlayer()
    {
        // ������ �÷��̾�� �׻� �����ʿ� ��ġ�ϵ��� ����
        if (transform.position.x < player.position.x - offsetToPlayer)
        {
            // ������ �÷��̾ ���� ���� �ӵ��� ����
            transform.position += new Vector3(moveSpeed * Time.deltaTime, 0, 0);
        }
    }



    void AttackPlayer()
    {
        Vector3 attackPosition = CalculateTargetPosition();

        // �߻� ��ġ ����
        GameObject spawnPoint = ChooseSpawnPoint();

        // ���̺� ���� �� ��ǥ ��ġ ����
        GameObject newCable = Instantiate(cable, spawnPoint.transform.position, Quaternion.identity);
        cable cableScript = newCable.GetComponent<cable>();
        cableScript.targetPosition = attackPosition;
    }
    

    GameObject ChooseSpawnPoint()
    {
        lastSpawnIndex = (lastSpawnIndex + 1) % spawnPoints.Length;
        return spawnPoints[lastSpawnIndex];
    }

    Vector3 CalculateTargetPosition()
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
    }

}
