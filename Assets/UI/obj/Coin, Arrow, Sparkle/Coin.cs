using UnityEngine;

public class Coin : MonoBehaviour
{
    public GameObject coinAnimationPrefab;  // �̰��� ���� ���� �ִϸ��̼� (Coin_spin)�� ���� ���Դϴ�.
    public GameObject sparkleAnimationPrefab;  // �̰��� ������ ����� �Ŀ� ��Ÿ�� Sparkle_1 �ִϸ��̼��� ���� ���Դϴ�.

    public Vector3 animationOffset = new Vector3(0, 0, 0);
    public float SparkleEffectDuration = 0.6f;  // Sparkle_1 �ִϸ��̼��� ���� �ð�

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ���� ���� (Coin_spin) �ִϸ��̼��� �־��� ��ġ�� Sparkle_1 �ִϸ��̼� ����
            GameObject sparkleAnimation = Instantiate(sparkleAnimationPrefab, transform.position + animationOffset, Quaternion.identity);

            // Sparkle_1 �ִϸ������� �ӵ� ����
            Animator sparkleAnimator = sparkleAnimation.GetComponent<Animator>();
            sparkleAnimator.speed = 1f / SparkleEffectDuration;

            // Sparkle_1 �ִϸ��̼��� �����ϰ� ������ �ð� �ڿ� ����
            sparkleAnimator.Play("Sparkle_1");  // �ִϸ��̼��� �̸��� "Sparkle_1"�̶�� ����
            Destroy(sparkleAnimation, SparkleEffectDuration);

            // ���� ī��Ʈ ����
           

            // ������ ���� (Coin_spin) ������Ʈ ����
            Destroy(gameObject);
        }
    }
}
