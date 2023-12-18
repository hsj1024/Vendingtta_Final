using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class ImageSet
{
    public List<Image> images = new List<Image>();
}

public class ImageFadeIn : MonoBehaviour
{
    public List<ImageSet> imageSets = new List<ImageSet>(); // �̹��� ��Ʈ ����Ʈ
    public List<float> fadeDurations; // �� ��Ʈ�� ���̵� ���ϴµ� �ɸ��� �ð�
    public List<float> waitTimeAfterSet; // �� ��Ʈ �Ŀ� ��ٸ� �ð�

    private int currentSetIndex = 0; // ���� ���̵� �� ���� ��Ʈ�� �ε���
    private int currentImageIndex = 0; // ���� ���̵� �� ���� �̹����� �ε���
    private float currentTime = 0f; // ���� ���̵� �� ���� �̹����� ���� �ð�
    private bool isWaiting = false; // ��ٸ��� ������ ����

    public Image fadeImage; // ���̵� ��/�ƿ��� ����� �̹���
    public float fadeDuration = 1.0f; // ���̵� ��/�ƿ��� �ɸ��� �ð�

    public void GoToSchoolMap()
    {
        StartCoroutine(GoToSchoolMapCoroutine());
    }

    private IEnumerator GoToSchoolMapCoroutine()
    {
        yield return StartCoroutine(Fade(1, fadeDuration)); // ���̵� �ƿ��� ����
        SceneManager.LoadScene("Chap1/Chap1");
    }

    private IEnumerator Fade(float targetAlpha, float duration)
    {
        float startAlpha = fadeImage.color.a;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            Color color = fadeImage.color;
            color.a = alpha;
            fadeImage.color = color;
            yield return null;
        }

        Color finalColor = fadeImage.color;
        finalColor.a = targetAlpha;
        fadeImage.color = finalColor;
    }

    void Update()
    {
        if (isWaiting || currentSetIndex >= imageSets.Count)
        {
            return;
        }

        ImageSet currentSet = imageSets[currentSetIndex];
        if (currentImageIndex < currentSet.images.Count)
        {
            currentTime += Time.deltaTime;
            float alpha = currentTime / fadeDurations[currentSetIndex];
            Image currentImage = currentSet.images[currentImageIndex];
            Color color = currentImage.color;
            color.a = alpha;
            currentImage.color = color;

            if (alpha >= 1.0f)
            {
                currentImageIndex++;
                currentTime = 0f;

                // �̹��� ��Ʈ�� ������ �̹������ ��ٸ��� ����
                if (currentImageIndex == currentSet.images.Count)
                {
                    StartCoroutine(WaitForSecondsCoroutine(waitTimeAfterSet[currentSetIndex], () => {
                        currentSetIndex++;
                        currentImageIndex = 0;
                    }));
                }
            }
        }
    }

    public void OnButtonClick()
    {
        //Debug.Log("��ư�� Ŭ���Ǿ����ϴ�!");
    }


    IEnumerator WaitForSecondsCoroutine(float seconds, Action callback)
    {
        isWaiting = true;
        yield return new WaitForSeconds(seconds);
        isWaiting = false;
        callback?.Invoke();
    }
}
