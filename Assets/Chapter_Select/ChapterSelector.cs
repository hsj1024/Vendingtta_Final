using UnityEngine;
using UnityEngine.UI;

public class ChapterSelector : MonoBehaviour
{
    public ScrollRect scrollRect; // 스크롤 뷰의 ScrollRect 컴포넌트
    public RectTransform[] chapterRects; // 각 챕터의 RectTransform 컴포넌트 배열
    public Button nextButton; // 다음 버튼
    public Button prevButton; // 이전 버튼
    public float targetWidth = 800; // 챕터의 너비 설정 (이 값을 조정하여 RIGHT PADDING을 설정할 수 있습니다)

    private int currentChapter = 0; // 현재 선택된 챕터
    private int totalChapters = 8; // 챕터의 총 수
    private float normalizedSpacing; // 정규화된 간격

    public float scrollAmount; // 스크롤하는 양을 조절할 변수 추가

    void Start()
    {
        // 챕터의 총 수 설정
        totalChapters = chapterRects.Length;

        // 스크롤 뷰의 가로 스크롤을 사용하도록 설정
        scrollRect.horizontal = true;
        scrollRect.vertical = false;

        // 스크롤 뷰의 가로 스크롤 바 설정
        scrollRect.horizontalScrollbar = null;
        scrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
        scrollRect.horizontalScrollbarSpacing = -3;

        // 스크롤 뷰의 크기 및 뷰포트 설정
        float contentWidth = totalChapters * targetWidth + (totalChapters - 1) * 10f; // 간격 추가
        scrollRect.content.sizeDelta = new Vector2(contentWidth, scrollRect.content.sizeDelta.y);
        scrollRect.viewport.sizeDelta = new Vector2(targetWidth, scrollRect.viewport.sizeDelta.y);

        // 다음 버튼과 이전 버튼에 대한 이벤트 리스너 설정
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(() => ChangeChapter(1));
        }

        if (prevButton != null)
        {
            prevButton.onClick.AddListener(() => ChangeChapter(-1));
        }

        // 초기 스크롤 위치 설정
        UpdateScrollPosition();
    }

    private void ChangeChapter(int offset)
    {
        currentChapter += offset; // 챕터 변경

        // 현재 챕터가 총 챕터 수를 초과하지 않도록 체크
        currentChapter = Mathf.Clamp(currentChapter, 0, totalChapters - 1);

        // 스크롤 양을 조절하여 스크롤 뷰를 이동
        float targetPosition = (float)currentChapter / (totalChapters - 1);
        scrollRect.horizontalNormalizedPosition = targetPosition;

        // 각 챕터의 가시성 설정
        UpdateChapterVisibility();

        Debug.Log("Current Chapter: " + currentChapter); // 디버그 출력 추가

        // 버튼 상태 업데이트
        UpdateButtonStates();
    }

    private void UpdateChapterVisibility()
    {
        // 모든 챕터를 일단 비활성화
        for (int i = 0; i < totalChapters; i++)
        {
            chapterRects[i].gameObject.SetActive(false);
        }

        // 현재 챕터만 활성화
        chapterRects[currentChapter].gameObject.SetActive(true);
    }

    private void UpdateScrollPosition()
    {
        // 챕터의 너비와 정규화된 간격 계산
        float chapterWidth = targetWidth; // targetWidth를 사용하여 챕터 너비 설정
        normalizedSpacing = 1.0f / (totalChapters - 1);

        // 스크롤 뷰의 위치 조정
        float targetPosition = currentChapter * normalizedSpacing;
        scrollRect.horizontalNormalizedPosition = targetPosition;

        // 각 챕터의 가시성 설정
        UpdateChapterVisibility();

        // 버튼 상태 업데이트
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        // 현재 챕터에 기반하여 다음 버튼과 이전 버튼 활성화/비활성화
        if (nextButton != null)
            nextButton.interactable = currentChapter < totalChapters - 1;

        if (prevButton != null)
            prevButton.interactable = currentChapter > 0;
    }
}
