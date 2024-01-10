using UnityEngine;

public class cable : MonoBehaviour
{
    public float speed = 10f;
    [SerializeField] public GameObject targetPoint; // �ν����Ϳ��� ���� �����ϵ��� ����
    [SerializeField] private GameObject returnPoint;
    private Vector3 launchDirection;

    private Boss2Controller bossController;
    private bool returning = false;
    private bool isLaunched = false;
    private Vector3 straightDirection;
    private bool isStraightMoving = false;
    private bool isReturning = false;
    public float maxDistance = 10f; // �ִ� �̵� �Ÿ�
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
            // ���� �Ÿ� �̵�
            transform.position += launchDirection * speed * Time.deltaTime;

            // �ִ� �̵� �Ÿ��� �����ϸ� �ǵ��ư��� ����
            if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
            {
                isReturning = true;
                launchDirection = -launchDirection; // �ݴ� �������� �̵�
            }
        }
        else
        {
            // �ǵ��ư��� ����
            transform.position += launchDirection * speed * Time.deltaTime;

            // ���� ��ġ�� �����ϸ� �Ҹ�
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
            // �÷��̾�� �¾��� ��
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
