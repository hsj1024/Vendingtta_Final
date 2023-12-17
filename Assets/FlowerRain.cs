using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class FlowerRain : MonoBehaviour
{
    public GameObject hitRangeSprite;
    public GameObject flowerPrefab;
    public GameObject effectSprite; // ����Ʈ ��������Ʈ�� ���� ����
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

        List<Vector3> hitRangePositions = new List<Vector3>(); // �ǰ� ������ ��ġ�� ������ ����Ʈ
        int flowersCount = 0; // ���� ���� �ʱ�ȭ

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 viewportPosition = new Vector3(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 0.3f), 10f);
            Vector3 spawnPosition = Camera.main.ViewportToWorldPoint(viewportPosition);
            spawnPosition.z = 0;

            GameObject spawnedSprite = Instantiate(hitRangeSprite, spawnPosition, Quaternion.identity);
            hitRangePositions.Add(spawnPosition); // ��ġ ����
            StartCoroutine(ShowAndDestroyHitRange(spawnedSprite));
        }

        yield return new WaitForSeconds(0.5f);
        yield return new WaitForSeconds(2.0f); // �߰� ������

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
        // �̹� ������ �ǰ� ������ ��ġ�� �ʴ��� Ȯ��
        foreach (var hitRange in existingHitRanges)
        {
            float hitRangeHalfWidth = hitRange.transform.localScale.x * 0.5f;
            float hitRangeHalfHeight = hitRange.transform.localScale.y * 0.5f;
            Vector3 hitRangePosition = hitRange.transform.position;
            if (Mathf.Abs(hitRangePosition.x - position.x) < hitRangeHalfWidth * 2 &&
                Mathf.Abs(hitRangePosition.y - position.y) < hitRangeHalfHeight * 2)
            {
                // ��ġ�� ��� ��ȿ���� ���� ��ġ
                return false;
            }
        }
        return true; // ��ȿ�� ��ġ
    }

    IEnumerator ShowAndDestroyHitRange(GameObject hitRange)
    {
        // ���̵� �� ����
        float startAlpha = 0f;
        float endAlpha = 1f; // ���� ������
        float fadeInDuration = 0.5f; // ���̵� �� ���� �ð�

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

            // ���� ���� ���� �����Ͽ� ���� �������ϰ� ����
            Color finalColor = renderer.material.color;
            finalColor.a = endAlpha;
            renderer.material.color = finalColor;
        }

        // ���̵� �� �Ϸ� �� ���� �ð� ���
        yield return new WaitForSeconds(0.5f);

        // ���̵� �ƿ� ȿ�� ����
        yield return FadeOut(hitRange, fadeDuration);
    }


    IEnumerator FallDown(GameObject flower, Vector3 targetPosition, Action onFallComplete)
    {
        float fallDuration = 1.0f;
        float timer = 0;

        // ���� �������� ������ �ִϸ��̼�
        while (timer < fallDuration)
        {
            flower.transform.position = Vector3.Lerp(flower.transform.position, targetPosition, timer / fallDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        flower.transform.position = targetPosition;

        // ���� ������ ��ġ�� ����Ʈ ��������Ʈ ����
        GameObject effect = Instantiate(effectSprite, targetPosition, Quaternion.identity);

        // ���� 2�ʰ� ǥ�õ� �� ����
        //yield return new WaitForSeconds(2f);
        //Destroy(flower);

        // ����Ʈ ��������Ʈ�� 1�ʰ� ǥ�õ� �� ����
        yield return new WaitForSeconds(2f);
        yield return FadeOut(flower, fadeDuration);

        yield return FadeOut(effect, fadeDuration);
        /*Destroy(flower);
        Destroy(effect);*/
        skillInProgress = false;
        onFallComplete?.Invoke(); // ���� ������ �����Ǿ��� �� �ݹ� ȣ��

    }

    private void OnNormalSkillCompletedInternal()
    {
        skillInProgress = false;
        OnNormalSkillCompleted?.Invoke();


    }


    IEnumerator FadeOut(GameObject obj, float duration)
    {
        // ����Ʈ ���̵� �ƿ� �ִϸ��̼�
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

        // ������ ���������� ��ü ����
        Destroy(obj);

    }
}
