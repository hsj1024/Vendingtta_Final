using UnityEngine;

public class cable : MonoBehaviour
{
    public float speed = 5f;
    public Vector3 targetPosition;
    public GameObject boss; // 보스 객체 참조
    public GameObject player; // 플레이어 객체 참조
    public float step = 1f; // 보스와 플레이어 사이 거리를 줄이는 단계 크기
    void Update()
    {
        // 케이블이 목표 위치로 이동
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // 멀리 날아가기 위해 정확한 위치 비교는 제거
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            // 플레이어와 보스의 거리를 줄이는 로직
            ReduceDistanceBetweenPlayerAndBoss();

            Destroy(gameObject);
        }
    }

    void ReduceDistanceBetweenPlayerAndBoss()
    {
        // 보스와 플레이어 사이의 거리를 줄이는 로직 구현
        boss.transform.position = Vector3.MoveTowards(boss.transform.position, player.transform.position, step);
    }
}
