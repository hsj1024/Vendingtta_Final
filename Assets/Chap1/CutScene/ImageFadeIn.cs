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
    public List<ImageSet> imageSets = new List<ImageSet>(); // 이미지 세트 리스트
    public List<float> fadeDurations; // 각 세트별 페이드 인하는데 걸리는 시간
    public List<float> waitTimeAfterSet; // 각 세트 후에 기다릴 시간

    private int currentSetIndex = 0; // 현재 페이드 인 중인 세트의 인덱스
    private int currentImageIndex = 0; // 현재 페이드 인 중인 이미지의 인덱스
    private float currentTime = 0f; // 현재 페이드 인 중인 이미지에 대한 시간
    private bool isWaiting = false; // 기다리는 중인지 여부

    public Image fadeImage; // 페이드 인/아웃에 사용할 이미지
    public float fadeDuration = 1.0f; // 페이드 인/아웃에 걸리는 시간

    public void GoToSchoolMap()
    {
        StartCoroutine(GoToSchoolMapCoroutine());
    }

    private IEnumerator GoToSchoolMapCoroutine()
    {
        yield return StartCoroutine(Fade(1, fadeDuration)); // 페이드 아웃만 실행
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

                // 이미지 세트의 마지막 이미지라면 기다리기 시작
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
        //Debug.Log("버튼이 클릭되었습니다!");
    }


    IEnumerator WaitForSecondsCoroutine(float seconds, Action callback)
    {
        isWaiting = true;
        yield return new WaitForSeconds(seconds);
        isWaiting = false;
        callback?.Invoke();
    }
}
