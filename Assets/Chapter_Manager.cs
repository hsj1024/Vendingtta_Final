using UnityEngine;

public class Chapter_Manager : MonoBehaviour
{
    public static Chapter_Manager Instance { get; private set; }

    private int currentChapterNumber = 1; // 초기 챕터 번호를 1로 설정

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 챕터를 시작할 때 호출됩니다.
    public void StartChapter(int chapterNumber)
    {
        currentChapterNumber = chapterNumber;
        //Mob_Manager.Instance.ActivateSpawner(chapterNumber);
    }

    public int GetCurrentChapterNumber()
    {
        return currentChapterNumber;
    }
}
