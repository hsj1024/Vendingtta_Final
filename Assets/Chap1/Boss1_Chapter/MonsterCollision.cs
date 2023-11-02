using UnityEngine;
using System.Collections;

public class MonsterCollision : MonoBehaviour
{
    public float knockbackStrength = 10f;
    public float damageAmount = 20f;
    public float knockbackDuration = 0.5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);

                Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;
                    StartCoroutine(ApplyKnockback(playerRb, knockbackDirection));
                }
            }
        }
    }

    private IEnumerator ApplyKnockback(Rigidbody2D playerRb, Vector2 knockbackDirection)
    {
        float timer = 0;

        while (timer < knockbackDuration)
        {
            playerRb.AddForce(knockbackDirection * knockbackStrength);
            timer += Time.deltaTime;
            yield return null;
        }

        playerRb.velocity = Vector2.zero;
    }
}
