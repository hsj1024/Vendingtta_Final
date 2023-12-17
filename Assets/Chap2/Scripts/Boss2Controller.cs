using UnityEngine;

public class Boss2Controller : MonoBehaviour
{
    
    public GameObject cable; // 케이블 프리팹
    public float attackDelay = 2f; // 공격 간격
    public Transform player;
    public float offsetToPlayer; // 플레이어에 대한 보스의 오프셋
    public GameObject[] spawnPoints; // 발사 위치 배열
    public float moveSpeed = 5f; // 보스의 이동 속도

    private float lastAttackTime;
    private int lastSpawnIndex = -1;


    void Update()
    {
        // 공격 로직
        if (Time.time > lastAttackTime + attackDelay)
        {
            AttackPlayer();
            lastAttackTime = Time.time;
        }
    }


    void FollowPlayer()
    {
        // 보스가 플레이어보다 항상 오른쪽에 위치하도록 보장
        if (transform.position.x < player.position.x - offsetToPlayer)
        {
            // 보스가 플레이어를 향해 일정 속도로 전진
            transform.position += new Vector3(moveSpeed * Time.deltaTime, 0, 0);
        }
    }



    void AttackPlayer()
    {
        Vector3 attackPosition = CalculateTargetPosition();

        // 발사 위치 선택
        GameObject spawnPoint = ChooseSpawnPoint();

        // 케이블 생성 및 목표 위치 설정
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
    }

}
