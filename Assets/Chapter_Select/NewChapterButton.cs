using UnityEngine;
using UnityEngine.UI;

public class NewChapterButton : MonoBehaviour // 클래스 이름 변경
{
    public int chapterNumber;
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();

        if (button == null)
        {
            Debug.LogError("Button component is missing in this gameobject");
            return;
        }

        if (chapterNumber < 1 || chapterNumber > 8)
        {
            Debug.LogError("Chapter number is out of range (1 ~ 8)");
            return;
        }

        UpdateButtonStatus();
    }

    private void UpdateButtonStatus()
    {
        if (chapterNumber == 1)
        {
            button.interactable = true;
            return;
        }

        bool previousChapterCompleted = PlayerPrefs.GetInt("Chapter" + (chapterNumber - 1), 0) == 1;
        button.interactable = previousChapterCompleted;
    }

    public void OnButtonClick()
    {
        // 챕터 시작 로직
    }
}
