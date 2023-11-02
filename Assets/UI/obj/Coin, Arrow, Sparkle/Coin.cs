using UnityEngine;

public class Coin : MonoBehaviour
{
    public GameObject coinAnimationPrefab;  // 이것은 원래 코인 애니메이션 (Coin_spin)을 위한 것입니다.
    public GameObject sparkleAnimationPrefab;  // 이것은 코인이 사라진 후에 나타낼 Sparkle_1 애니메이션을 위한 것입니다.

    public Vector3 animationOffset = new Vector3(0, 0, 0);
    public float SparkleEffectDuration = 0.6f;  // Sparkle_1 애니메이션의 지속 시간

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 원래 코인 (Coin_spin) 애니메이션이 있었던 위치에 Sparkle_1 애니메이션 생성
            GameObject sparkleAnimation = Instantiate(sparkleAnimationPrefab, transform.position + animationOffset, Quaternion.identity);

            // Sparkle_1 애니메이터의 속도 설정
            Animator sparkleAnimator = sparkleAnimation.GetComponent<Animator>();
            sparkleAnimator.speed = 1f / SparkleEffectDuration;

            // Sparkle_1 애니메이션을 실행하고 지정한 시간 뒤에 제거
            sparkleAnimator.Play("Sparkle_1");  // 애니메이션의 이름이 "Sparkle_1"이라고 가정
            Destroy(sparkleAnimation, SparkleEffectDuration);

            // 코인 카운트 증가
           

            // 원래의 코인 (Coin_spin) 오브젝트 제거
            Destroy(gameObject);
        }
    }
}
