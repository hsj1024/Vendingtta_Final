using UnityEngine;
using System.Collections;
public class FlowerImpactEffect : MonoBehaviour
{
    public GameObject impactSpritePrefab; // 이펙트 스프라이트 프리팹

    private bool isGrounded = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground")) // 땅과 충돌한 경우
        {
            isGrounded = true;
            // 3초 후에 이펙트 재생 및 게임 오브젝트 파괴
            StartCoroutine(DelayedImpact());
        }
    }

    private IEnumerator DelayedImpact()
    {
        yield return new WaitForSeconds(3f); // 3초 대기

        if (isGrounded)
        {
            PlayImpactEffect();
            Destroy(gameObject); // 이펙트 재생 후 이펙트 게임 오브젝트 파괴
        }
    }

    private void PlayImpactEffect()
    {
        if (impactSpritePrefab != null)
        {
            // 이펙트 스프라이트를 꽃의 위치에 생성합니다.
            Instantiate(impactSpritePrefab, transform.position, Quaternion.identity);
        }
    }
}
