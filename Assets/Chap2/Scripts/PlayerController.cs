using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public DieBoss2 dieBoss2Script; // DieBoss2 스크립트 참조

    private void Start()
    {
        // 참조가 없는 경우, 찾아서 설정
        if (dieBoss2Script == null)
        {
            dieBoss2Script = FindObjectOfType<DieBoss2>();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // 'SpawnPoint' 태그를 가진 오브젝트와 충돌했을 때
        if (other.CompareTag("Boss2MouthPoint"))
        {
            Debug.Log("죽음");
            Die();
        }
    }
    private void Die()
    {
        // Die 메소드가 호출되면 DieBoss2 스크립트의 PlayerDied 메소드 호출
        if (dieBoss2Script != null)
        {
            dieBoss2Script.PlayerDied();
        }
    }

    // 나머지 코드...
}
