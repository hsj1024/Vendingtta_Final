using UnityEngine;

public class FlowerCollision : MonoBehaviour
{
    public float knockbackStrength = 10f; // �˹��� ����
    public float damageAmount = 20f; // ������ ���ݷ� �Ǵ� ü�� ���ҷ�
    public GameObject impactEffectPrefab; // ����Ʈ ������
    private Vector2 initialPlayerPosition; // �÷��̾��� �ʱ� ��ġ ���� ����

    private void Start()
    {
        // �ʱ�ȭ �� �÷��̾��� �ʱ� ��ġ�� ����
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            initialPlayerPosition = player.transform.position;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �÷��̾�� �浹�ߴ��� Ȯ��
        if (collision.gameObject.CompareTag("Player"))
        {
            // ����Ʈ �������� �����Ǿ� ������ ����Ʈ ����
            if (impactEffectPrefab != null)
            {
                GameObject impactEffect = Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
                Destroy(impactEffect, 3f); // 3�� �Ŀ� ����Ʈ �ı�
            }

            // �� �ı�
            Destroy(gameObject);
        }
        // �÷��̾� ���� �ٸ� ��ü���� �浹 ó�� (�ɼ�)
        else
        {
            // �ٸ� �±׳� ��ü�� �浹���� ���� ó�� ����
        }
    }
}
