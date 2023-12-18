using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public DieBoss2 dieBoss2Script; // DieBoss2 ��ũ��Ʈ ����

    private void Start()
    {
        // ������ ���� ���, ã�Ƽ� ����
        if (dieBoss2Script == null)
        {
            dieBoss2Script = FindObjectOfType<DieBoss2>();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // 'SpawnPoint' �±׸� ���� ������Ʈ�� �浹���� ��
        if (other.CompareTag("Boss2MouthPoint"))
        {
            Debug.Log("����");
            Die();
        }
    }
    private void Die()
    {
        // Die �޼ҵ尡 ȣ��Ǹ� DieBoss2 ��ũ��Ʈ�� PlayerDied �޼ҵ� ȣ��
        if (dieBoss2Script != null)
        {
            dieBoss2Script.PlayerDied();
        }
    }

    // ������ �ڵ�...
}
