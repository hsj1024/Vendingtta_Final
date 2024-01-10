using UnityEngine;
using System.Collections;

public class KeyboardFragment : MonoBehaviour
{
    public float moveSpeed = 10f; // 이동 속도
    public GameObject nonMotionSpritePrefab; // 비 모션 스프라이트 프리팹
    public GameObject hitAreaPrefab; // 피격 범위 프리팹
    public Transform player; // 플레이어의 Transform

    private Vector3 targetPosition;
    private bool isMoving = true;

    void Start()
    {
        // 화면 정중앙 상단 밖의 위치 설정
        targetPosition = new Vector3(Camera.main.transform.position.x, 
                                     Camera.main.transform.position.y + Camera.main.orthographicSize + 1.0f, // 화면 밖으로
                                     Camera.main.transform.position.z);
    }

    void Update()
    {
        if (isMoving)
        {
            // 목표 위치로 이동
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // 목표 위치에 도달하면 처리
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                // 파편 스프라이트 비활성화
                GetComponent<SpriteRenderer>().enabled = false;

                // 피격 범위 및 비 프리팹 생성 로직 시작
                StartCoroutine(ShowHitAreaAndSpawnRainPrefab(player.position));

                isMoving = false; // 이동 중지
            }
        }
    }

    IEnumerator ShowHitAreaAndSpawnRainPrefab(Vector3 targetPosition)
    {
        // 플레이어 위치에 피격 범위 표시
        GameObject hitArea = Instantiate(hitAreaPrefab, targetPosition, Quaternion.identity);
        yield return new WaitForSeconds(1.5f);
        Destroy(hitArea);

        // 플레이어 피격 범위 위치에서 3 유닛 위에서 비 프리팹 생성
        Vector3 rainStartPosition = new Vector3(targetPosition.x, targetPosition.y + 9, targetPosition.z);
        GameObject rainPrefab = Instantiate(nonMotionSpritePrefab, rainStartPosition, Quaternion.identity);

        // 1.5초 후에 페이드 아웃 시작
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(FadeOutSprite(rainPrefab, 1.5f));
    }

    IEnumerator FadeOutSprite(GameObject spriteObject, float duration)
    {
        SpriteRenderer spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Destroy(spriteObject);
            yield break;
        }

        float counter = 0;
        Color spriteColor = spriteRenderer.color;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, counter / duration);
            spriteRenderer.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, alpha);
            yield return null;
        }

        Destroy(spriteObject);
    }


}
