using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveViewport_Right : MonoBehaviour
{
    public ScrollRect scrollRect; // �̵��� ScrollRect
    public GameObject[] chapters; // ��� é�� ��ü��
    public float moveAmount = 0.1f; // �ѹ��� �̵��� �Ÿ� (0 ~ 1)
    private int currentChapterIndex = 0; // ���� ���õ� é���� �ε���
    public float transitionTime = 0.5f; // �̵��� �ɸ��� �ð�

    // ��ư �̺�Ʈ�� ���� �Լ�
    public void OnButtonClick()
    {
        // ���� é�͸� ���
        chapters[currentChapterIndex].transform.localScale = Vector3.one / 1.5f;

        // �ε����� ������Ű�� ������ Ȯ��
        currentChapterIndex++;
        if (currentChapterIndex >= chapters.Length)
        {
            currentChapterIndex = chapters.Length - 1;
        }

        // ���ο� é�͸� Ȯ��
        chapters[currentChapterIndex].transform.localScale = Vector3.one * 3f; // ��: 1.5�� Ȯ��

        // é�� �̵� ����
        StartCoroutine(MoveToChapter());
    }

    IEnumerator MoveToChapter()
    {
        float startTime = Time.time;
        float startPos = scrollRect.horizontalNormalizedPosition;
        float endPos = currentChapterIndex * moveAmount;

        // ��ũ�� ������ Content ũ�� ����
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
