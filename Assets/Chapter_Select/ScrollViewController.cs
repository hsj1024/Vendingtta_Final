using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScrollViewController : MonoBehaviour
{
    public ScrollRect scrollRect; // �̵��� ScrollRect
    private int currentChapter = 1; // ���� é�� ��ȣ
    private int totalChapters = 8; // �� é�� ����

    private void Start()
    {
        // ���� ���� �� ��ũ�� ���� ��ġ�� Chapter 1�� �°� ����
        MoveToChapter(currentChapter);
    }

    public void OnNextButtonClick()
    {
        // ���� é�Ͱ� ������ é�Ͱ� �ƴ� ��츸 ���� é�ͷ� �̵�
        if (currentChapter < totalChapters)
        {
            currentChapter++;
            MoveToChapter(currentChapter);
        }
    }

    public void OnPreviousButtonClick()
    {
        // ���� é�Ͱ� ù ��° é�Ͱ� �ƴ� ��츸 ���� é�ͷ� �̵�
        if (currentChapter > 1)
        {
            currentChapter--;
            MoveToChapter(currentChapter);
        }
    }

    private void MoveToChapter(int chapterNumber)
    {
        // �������� ��ü ���̸� ������� �� é���� ��ġ�� ���
        float chapterWidth = 1f / (float)totalChapters; // totalChapters�� float�� ��ȯ�� ������ ������ ����
        float targetPosition = (chapterNumber - 1) * chapterWidth + chapterWidth * 0.5f;

        // ��ġ�� ScrollRect�� normalized position���� ��ȯ
        scrollRect.horizontalNormalizedPosition = Mathf.Clamp(targetPosition, 0, 1);
    }

    public void LoadChapterScene(int chapterNumber)
    {
        // é�� ��ư�� ���� �� ����� �Լ�
        // ���⼭ chapterNumber�� é�� ��ư�� ������ �ε����� ��Ÿ���ϴ�.

        string sceneName = "Chapter " + chapterNumber.ToString(); // ���� é�� Scene �̸� ����
        SceneManager.LoadScene(sceneName); // �ش� é�� Scene���� ��ȯ
    }
}
