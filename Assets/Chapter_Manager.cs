using UnityEngine;

public class Chapter_Manager : MonoBehaviour
{
    public static Chapter_Manager Instance { get; private set; }

    private int currentChapterNumber = 1; // �ʱ� é�� ��ȣ�� 1�� ����

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

    // é�͸� ������ �� ȣ��˴ϴ�.
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
