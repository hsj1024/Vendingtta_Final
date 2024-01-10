using UnityEngine;

public class cable : MonoBehaviour
{
    public float speed = 10f;
    [SerializeField] public GameObject targetPoint; // 인스펙터에서 설정 가능하도록 변경
    [SerializeField] private GameObject returnPoint;
    private Vector3 launchDirection;

    private Boss2Controller bossController;
    private bool returning = false;
    private bool isLaunched = false;
    private Vector3 straightDirection;
    private bool isStraightMoving = false;
    private bool isReturning = false;
    public float maxDistance = 10f; // 최대 이동 거리
    private Vector3 startPosition;

    public void SetStraightMovement(Vector3 direction)
    {
        straightDirection = direction.normalized;
        isStraightMoving = true;
    }

    



    public void Launch(Vector3 launchDir, GameObject returnObj, Boss2Controller boss)
    {
        launchDirection = launchDir.normalized;
        startPosition = transform.position;
        bossController = boss;
        isReturning = false;
    }

    void Update()
    {
        if (!isReturning)
        {
            // 일정 거리 이동
            transform.position += launchDirection * speed * Time.deltaTime;

            // 최대 이동 거리에 도달하면 되돌아가기 시작
            if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
            {
                isReturning = true;
                launchDirection = -launchDirection; // 반대 방향으로 이동
            }
        }
        else
        {
            // 되돌아가는 로직
            transform.position += launchDirection * speed * Time.deltaTime;

            // 원래 위치에 도달하면 소멸
            if (Vector3.Distance(startPosition, transform.position) <= 0.1f)
            {
                Destroy(gameObject);
            }
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어에게 맞았을 때
            bossController.OnCableHit();
            isReturning = true;
        }
    }


    void OnDestroy()
    {
        if (bossController != null)
        {
            //bossController.RespawnCable();
        }
    }
}
