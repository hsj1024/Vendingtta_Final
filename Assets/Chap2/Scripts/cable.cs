using UnityEngine;

public class cable : MonoBehaviour
{
    public float speed = 10f;
    [SerializeField] public GameObject targetPoint; // 인스펙터에서 설정 가능하도록 변경
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
        if (!isLaunched) return; // Launch가 호출되기 전에는 Update의 나머지 부분을 실행하지 않음

        if (targetPoint == null)
        {
            Debug.LogError("targetPoint is null or destroyed.");
            return; // targetPoint가 null이면 여기서 함수를 종료
        }

        if (!returning)
        {
            // 타겟 위치로 이동
            
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.transform.position, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPoint.transform.position) < 0.1f)
            {
                // 타겟 위치에 도달하면 되돌아감
                returning = true;
                speed *= 2; // 되돌아갈 때의 속도를 두 배로 증가
            }
        }
        else
        {
            // 되돌아가는 로직
            transform.position = Vector3.MoveTowards(transform.position, returnPoint.transform.position, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, returnPoint.transform.position) < 0.1f)
            {
                // 되돌아간 위치에 도달하면 소멸
                //Destroy(gameObject);
            }
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어에게 맞았을 때
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
