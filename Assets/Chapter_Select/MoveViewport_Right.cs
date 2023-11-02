using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveViewport_Right : MonoBehaviour
{
    public ScrollRect scrollRect; // 이동할 ScrollRect
    public GameObject[] chapters; // 모든 챕터 객체들
    public float moveAmount = 0.1f; // 한번에 이동할 거리 (0 ~ 1)
    private int currentChapterIndex = 0; // 현재 선택된 챕터의 인덱스
    public float transitionTime = 0.5f; // 이동에 걸리는 시간

    // 버튼 이벤트를 위한 함수
    public void OnButtonClick()
    {
        // 이전 챕터를 축소
        chapters[currentChapterIndex].transform.localScale = Vector3.one / 1.5f;

        // 인덱스를 증가시키고 범위를 확인
        currentChapterIndex++;
        if (currentChapterIndex >= chapters.Length)
        {
            currentChapterIndex = chapters.Length - 1;
        }

        // 새로운 챕터를 확대
        chapters[currentChapterIndex].transform.localScale = Vector3.one * 3f; // 예: 1.5배 확대

        // 챕터 이동 시작
        StartCoroutine(MoveToChapter());
    }

    IEnumerator MoveToChapter()
    {
        float startTime = Time.time;
        float startPos = scrollRect.horizontalNormalizedPosition;
        float endPos = currentChapterIndex * moveAmount;

        // 스크롤 가능한 Content 크기 변경
        RectTransform contentRect = scrollRect.content;
        float contentWidth = (currentChapterIndex + 2) * chapters[0].GetComponent<RectTransform>().rect.width;
        contentRect.sizeDelta = new Vector2(contentWidth, contentRect.sizeDelta.y);

        while (Time.time - startTime < transitionTime)
        {
            float t = (Time.time - startTime) / transitionTime;
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(startPos, endPos, t);
            yield return null;
        }

        scrollRect.horizontalNormalizedPosition = endPos;
    }
}
