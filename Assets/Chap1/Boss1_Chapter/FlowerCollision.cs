using UnityEngine;

public class FlowerCollision : MonoBehaviour
{
    public float knockbackStrength = 10f; // 넉백의 강도
    public float damageAmount = 20f; // 몬스터의 공격력 또는 체력 감소량
    public GameObject impactEffectPrefab; // 이펙트 프리팹
    private Vector2 initialPlayerPosition; // 플레이어의 초기 위치 저장 변수

    private void Start()
    {
        // 초기화 시 플레이어의 초기 위치를 저장
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            initialPlayerPosition = player.transform.position;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 플레이어와 충돌했는지 확인
        if (collision.gameObject.CompareTag("Player"))
        {
            // 이펙트 프리팹이 설정되어 있으면 이펙트 생성
            if (impactEffectPrefab != null)
            {
                GameObject impactEffect = Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
                Destroy(impactEffect, 3f); // 3초 후에 이펙트 파괴
            }

            // 꽃 파괴
            Destroy(gameObject);
        }
        // 플레이어 외의 다른 객체와의 충돌 처리 (옵션)
        else
        {
            // 다른 태그나 객체와 충돌했을 때의 처리 로직
        }
    }
}
