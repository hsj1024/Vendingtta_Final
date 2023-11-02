using UnityEngine;

public class FlowerCollision : MonoBehaviour
{
    public float knockbackStrength = 10f; // �˹��� ����
    public float damageAmount = 20f; // ������ ���ݷ� �Ǵ� ü�� ���ҷ�

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // �÷��̾�� �浹���� �� �÷��̾��� ü���� ���ҽ�Ŵ
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);

                // �˹��� ������ �� �ִ� �ڵ� �߰�
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                    playerRb.AddForce(knockbackDirection * knockbackStrength, ForceMode2D.Impulse);
                }
            }
        }
    }
}