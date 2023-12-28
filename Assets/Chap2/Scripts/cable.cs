using UnityEngine;

public class cable : MonoBehaviour
{
    public float speed = 10f;
    [SerializeField] public GameObject targetPoint; // �ν����Ϳ��� ���� �����ϵ��� ����
    [SerializeField] private GameObject returnPoint;
    
    private Boss2Controller bossController;
    private bool returning = false;
    private bool isLaunched = false;

    public void Launch(GameObject targetObj, GameObject returnObj, Boss2Controller boss)
    {
       
        isLaunched = true;
        
        targetPoint = targetObj;
        returnPoint = returnObj;
        bossController = boss;
    }

    void Update()
    {
        if (!isLaunched) return; // Launch�� ȣ��Ǳ� ������ Update�� ������ �κ��� �������� ����

        if (targetPoint == null)
        {
            Debug.LogError("targetPoint is null or destroyed.");
            return; // targetPoint�� null�̸� ���⼭ �Լ��� ����
        }

        if (!returning)
        {
            // Ÿ�� ��ġ�� �̵�
            
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.transform.position, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPoint.transform.position) < 0.1f)
            {
                // Ÿ�� ��ġ�� �����ϸ� �ǵ��ư�
                returning = true;
                speed *= 2; // �ǵ��ư� ���� �ӵ��� �� ��� ����
            }
        }
        else
        {
            // �ǵ��ư��� ����
            transform.position = Vector3.MoveTowards(transform.position, returnPoint.transform.position, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, returnPoint.transform.position) < 0.1f)
            {
                // �ǵ��ư� ��ġ�� �����ϸ� �Ҹ�
                //Destroy(gameObject);
            }
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // �÷��̾�� �¾��� ��
            bossController.OnCableHit();
            returning = true;
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
