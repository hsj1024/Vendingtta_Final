using UnityEngine;
using UnityEngine.UI;

public class ChapterSelector : MonoBehaviour
{
    public ScrollRect scrollRect; // ��ũ�� ���� ScrollRect ������Ʈ
    public RectTransform[] chapterRects; // �� é���� RectTransform ������Ʈ �迭
    public Button nextButton; // ���� ��ư
    public Button prevButton; // ���� ��ư
    public float targetWidth = 800; // é���� �ʺ� ���� (�� ���� �����Ͽ� RIGHT PADDING�� ������ �� �ֽ��ϴ�)

    private int currentChapter = 0; // ���� ���õ� é��
    private int totalChapters = 8; // é���� �� ��
    private float normalizedSpacing; // ����ȭ�� ����

    public float scrollAmount; // ��ũ���ϴ� ���� ������ ���� �߰�

    void Start()
    {
        // é���� �� �� ����
        totalChapters = chapterRects.Length;

        // ��ũ�� ���� ���� ��ũ���� ����ϵ��� ����
        scrollRect.horizontal = true;
        scrollRect.vertical = false;

        // ��ũ�� ���� ���� ��ũ�� �� ����
        scrollRect.horizontalScrollbar = null;
        scrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
        scrollRect.horizontalScrollbarSpacing = -3;

        // ��ũ�� ���� ũ�� �� ����Ʈ ����
        float contentWidth = totalChapters * targetWidth + (totalChapters - 1) * 10f; // ���� �߰�
        scrollRect.content.sizeDelta = new Vector2(contentWidth, scrollRect.content.sizeDelta.y);
        scrollRect.viewport.sizeDelta = new Vector2(targetWidth, scrollRect.viewport.sizeDelta.y);

        // ���� ��ư�� ���� ��ư�� ���� �̺�Ʈ ������ ����
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(() => ChangeChapter(1));
        }

        if (prevButton != null)
        {
            prevButton.onClick.AddListener(() => ChangeChapter(-1));
        }

        // �ʱ� ��ũ�� ��ġ ����
        UpdateScrollPosition();
    }

    private void ChangeChapter(int offset)
    {
        currentChapter += offset; // é�� ����

        // ���� é�Ͱ� �� é�� ���� �ʰ����� �ʵ��� üũ
        currentChapter = Mathf.Clamp(currentChapter, 0, totalChapters - 1);

        // ��ũ�� ���� �����Ͽ� ��ũ�� �並 �̵�
        float targetPosition = (float)currentChapter / (totalChapters - 1);
        scrollRect.horizontalNormalizedPosition = targetPosition;

        // �� é���� ���ü� ����
        UpdateChapterVisibility();

        Debug.Log("Current Chapter: " + currentChapter); // ����� ��� �߰�

        // ��ư ���� ������Ʈ
        UpdateButtonStates();
    }

    private void UpdateChapterVisibility()
    {
        // ��� é�͸� �ϴ� ��Ȱ��ȭ
        for (int i = 0; i < totalChapters; i++)
        {
            chapterRects[i].gameObject.SetActive(false);
        }

        // ���� é�͸� Ȱ��ȭ
        chapterRects[currentChapter].gameObject.SetActive(true);
    }

    private void UpdateScrollPosition()
    {
        // é���� �ʺ�� ����ȭ�� ���� ���
        float chapterWidth = targetWidth; // targetWidth�� ����Ͽ� é�� �ʺ� ����
        normalizedSpacing = 1.0f / (totalChapters - 1);

        // ��ũ�� ���� ��ġ ����
        float targetPosition = currentChapter * normalizedSpacing;
        scrollRect.horizontalNormalizedPosition = targetPosition;

        // �� é���� ���ü� ����
        UpdateChapterVisibility();

        // ��ư ���� ������Ʈ
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        // ���� é�Ϳ� ����Ͽ� ���� ��ư�� ���� ��ư Ȱ��ȭ/��Ȱ��ȭ
        if (nextButton != null)
            nextButton.interactable = currentChapter < totalChapters - 1;

        if (prevButton != null)
            prevButton.interactable = currentChapter > 0;
    }
}
