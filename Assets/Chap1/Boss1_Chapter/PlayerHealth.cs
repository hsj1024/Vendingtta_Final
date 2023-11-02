using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= (int)damage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        // 여기에 체력이 0이 되었을 때의 처리를 추가할 수 있습니다.
    }
}
