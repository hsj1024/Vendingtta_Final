using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class FlowerRain : MonoBehaviour
{
    public GameObject hitRangeSprite;
    public GameObject flowerPrefab;
    public GameObject effectSprite; // 이펙트 스프라이트에 대한 참조
    public float spawnRadius = 5f;
    public int spawnCount = 6;
    public delegate void SkillCompleted();
    public event SkillCompleted OnNormalSkillCompleted;
    private bool skillInProgress = false;
    public float fadeDuration = 0.5f;

    void Start()
    {

    }

    public bool IsSkillInProgress
    {
        get { return skillInProgress; }
    }
    public void ActivateNormalSkill()
    {
        if (!skillInProgress)
        {
            StartCoroutine(SkillSequence());
        }
    }



    IEnumerator SkillSequence()
    {
        skillInProgress = true;

        List<Vector3> hitRangePositions = new List<Vector3>(); // 피격 범위의 위치를 저장할 리스트
        int flowersCount = 0; // 꽃의 개수 초기화

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 viewportPosition = new Vector3(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 0.3f), 10f);
            Vector3 spawnPosition = Camera.main.ViewportToWorldPoint(viewportPosition);
            spawnPosition.z = 0;

            GameObject spawnedSprite = Instantiate(hitRangeSprite, spawnPosition, Quaternion.identity);
            hitRangePositions.Add(spawnPosition); // 위치 저장
            StartCoroutine(ShowAndDestroyHitRange(spawnedSprite));
        }

        yield return new WaitForSeconds(0.5f);
        yield return new WaitForSeconds(2.0f); // 추가 딜레이

        foreach (var position in hitRangePositions)
        {
            GameObject flower = Instantiate(flowerPrefab, position + Vector3.up * 10, Quaternion.identity);
            flower.tag = "flower";
            flowersCount++;

            StartCoroutine(FallDown(flower, position, () => {
                flowersCount--;
                if (flowersCount == 0)
                {
                    OnNormalSkillCompletedInternal();
                }
            }));
        }
    }





    bool IsSpawnPositionValid(List<GameObject> existingHitRanges, Vector3 position)
    {
        // 이미 생성된 피격 범위와 겹치지 않는지 확인
        foreach (var hitRange in existingHitRanges)
        {
            float hitRangeHalfWidth = hitRange.transform.localScale.x * 0.5f;
            float hitRangeHalfHeight = hitRange.transform.localScale.y * 0.5f;
            Vector3 hitRangePosition = hitRange.transform.position;
            if (Mathf.Abs(hitRangePosition.x - position.x) < hitRangeHalfWidth * 2 &&
                Mathf.Abs(hitRangePosition.y - position.y) < hitRangeHalfHeight * 2)
            {
                // 겹치는 경우 유효하지 않은 위치
                return false;
            }
        }
        return true; // 유효한 위치
    }

    IEnumerator ShowAndDestroyHitRange(GameObject hitRange)
    {
        // 페이드 인 시작
        float startAlpha = 0f;
        float endAlpha = 1f; // 완전 불투명
        float fadeInDuration = 0.5f; // 페이드 인 지속 시간

        SpriteRenderer renderer = hitRange.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            float timer = 0f;
            while (timer < fadeInDuration)
            {
                float alpha = Mathf.Lerp(startAlpha, endAlpha, timer / fadeInDuration);
                Color color = renderer.material.color;
                color.a = alpha;
                renderer.material.color = color;

                timer += Time.deltaTime;
                yield return null;
            }

            // 최종 알파 값을 설정하여 완전 불투명하게 만듦
            Color finalColor = renderer.material.color;
            finalColor.a = endAlpha;
            renderer.material.color = finalColor;
        }

        // 페이드 인 완료 후 일정 시간 대기
        yield return new WaitForSeconds(0.5f);

        // 페이드 아웃 효과 적용
        yield return FadeOut(hitRange, fadeDuration);
    }


    IEnumerator FallDown(GameObject flower, Vector3 targetPosition, Action onFallComplete)
    {
        float fallDuration = 1.0f;
        float timer = 0;

        // 꽃이 떨어지는 동안의 애니메이션
        while (timer < fallDuration)
        {
            flower.transform.position = Vector3.Lerp(flower.transform.position, targetPosition, timer / fallDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        flower.transform.position = targetPosition;

        // 꽃이 떨어진 위치에 이펙트 스프라이트 생성
        GameObject effect = Instantiate(effectSprite, targetPosition, Quaternion.identity);

        // 꽃이 2초간 표시된 후 삭제
        //yield return new WaitForSeconds(2f);
        //Destroy(flower);

        // 이펙트 스프라이트가 1초간 표시된 후 삭제
        yield return new WaitForSeconds(2f);
        yield return FadeOut(flower, fadeDuration);

        yield return FadeOut(effect, fadeDuration);
        /*Destroy(flower);
        Destroy(effect);*/
        skillInProgress = false;
        onFallComplete?.Invoke(); // 꽃이 완전히 삭제되었을 때 콜백 호출

    }

    private void OnNormalSkillCompletedInternal()
    {
        skillInProgress = false;
        OnNormalSkillCompleted?.Invoke();


    }


    IEnumerator FadeOut(GameObject obj, float duration)
    {
        // 이펙트 페이드 아웃 애니메이션
        float startAlpha = obj.GetComponent<Renderer>().material.color.a;
        float timer = 0;

        while (timer < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, 0, timer / duration);
            Color color = obj.GetComponent<Renderer>().material.color;
            color.a = alpha;
            obj.GetComponent<Renderer>().material.color = color;
            timer += Time.deltaTime;
            yield return null;
        }

        // 완전히 투명해지면 객체 삭제
        Destroy(obj);

    }
}
