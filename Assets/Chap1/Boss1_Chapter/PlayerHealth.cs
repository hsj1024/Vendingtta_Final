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

        // ���⿡ ü���� 0�� �Ǿ��� ���� ó���� �߰��� �� �ֽ��ϴ�.
    }
}
