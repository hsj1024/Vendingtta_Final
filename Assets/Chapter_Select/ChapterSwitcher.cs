using UnityEngine;
using UnityEngine.SceneManagement;

public class ChapterSwitcher : MonoBehaviour
{
    private int currentChapter = 1;
    private const int maxChapter = 8;

    public void GoToNextChapter()
    {
        if (currentChapter < maxChapter)
        {
            currentChapter++;
            SceneManager.LoadScene("Chapter" + currentChapter);
        }
    }
}
