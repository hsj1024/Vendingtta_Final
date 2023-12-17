using System.Collections;
using UnityEngine;

public class MobManager : MonoBehaviour
{
    public static MobManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void ApplyKnockback(Rigidbody2D rb, Vector2 direction, float strength, float duration)
    {
        StartCoroutine(KnockbackCoroutine(rb, direction, strength, duration));
    }

    private IEnumerator KnockbackCoroutine(Rigidbody2D rb, Vector2 direction, float strength, float duration)
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.AddForce(direction * strength, ForceMode2D.Impulse);

        yield return new WaitForSeconds(duration);

        rb.velocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void Die(GameObject mob, GameObject coinPrefab, int coinsToDrop, float deathAnimationDuration)
    {
        StartCoroutine(DeathAnimation(mob, coinPrefab, coinsToDrop, deathAnimationDuration));
    }

    private IEnumerator DeathAnimation(GameObject mob, GameObject coinPrefab, int coinsToDrop, float duration)
    {
        // Play death animation
        Animator animator = mob.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // Wait for the death animation to complete
        yield return new WaitForSeconds(duration);

        // Drop coins
        DropCoins(mob.transform.position, coinPrefab, coinsToDrop);

        // Destroy the mob object
        Destroy(mob);
    }

    private void DropCoins(Vector3 position, GameObject coinPrefab, int numberOfCoins)
    {
        for (int i = 0; i < numberOfCoins; i++)
        {
            // Instantiate the coin prefab at the specified position
            Instantiate(coinPrefab, position, Quaternion.identity);
            // Additional logic for animating or moving the coins can be added here
        }
    }
}
