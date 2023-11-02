using UnityEngine;

public class FlowerCollision : MonoBehaviour
{
    public float knockbackStrength = 10f; // 넉백의 강도
    public float damageAmount = 20f; // 몬스터의 공격력 또는 체력 감소량

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 플레이어와 충돌했을 때 플레이어의 체력을 감소시킴
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);

                // 넉백을 구현할 수 있는 코드 추가
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