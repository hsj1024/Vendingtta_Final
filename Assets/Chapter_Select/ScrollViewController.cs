using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScrollViewController : MonoBehaviour
{
    public ScrollRect scrollRect; // 이동할 ScrollRect
    private int currentChapter = 1; // 현재 챕터 번호
    private int totalChapters = 8; // 총 챕터 개수

    private void Start()
    {
        // 게임 시작 시 스크롤 뷰의 위치를 Chapter 1에 맞게 설정
        MoveToChapter(currentChapter);
    }

    public void OnNextButtonClick()
    {
        // 현재 챕터가 마지막 챕터가 아닌 경우만 다음 챕터로 이동
        if (currentChapter < totalChapters)
        {
            currentChapter++;
            MoveToChapter(currentChapter);
        }
    }

    public void OnPreviousButtonClick()
    {
        // 현재 챕터가 첫 번째 챕터가 아닌 경우만 이전 챕터로 이동
        if (currentChapter > 1)
        {
            currentChapter--;
            MoveToChapter(currentChapter);
        }
    }

    private void MoveToChapter(int chapterNumber)
    {
        // 콘텐츠의 전체 길이를 기반으로 각 챕터의 위치를 계산
        float chapterWidth = 1f / (float)totalChapters; // totalChapters를 float로 변환해 나눗셈 문제를 방지
        float targetPosition = (chapterNumber - 1) * chapterWidth + chapterWidth * 0.5f;

        // 위치를 ScrollRect의 normalized position으로 변환
        scrollRect.horizontalNormalizedPosition = Mathf.Clamp(targetPosition, 0, 1);
    }

    public void LoadChapterScene(int chapterNumber)
    {
        // 챕터 버튼을 누를 때 실행될 함수
        // 여기서 chapterNumber는 챕터 버튼의 순서나 인덱스를 나타냅니다.

        string sceneName = "Chapter " + chapterNumber.ToString(); // 실제 챕터 Scene 이름 설정
        SceneManager.LoadScene(sceneName); // 해당 챕터 Scene으로 전환
    }
}
