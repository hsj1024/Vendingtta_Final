using System.Collections.Generic;
using UnityEngine;

public class Mob_Manager : MonoBehaviour
{
    public static Mob_Manager Instance { get; private set; }

    [System.Serializable]
    public class ChapterSpawner
    {
        public int chapterNumber;
        public Mob_spawner mobSpawner;
    }

    public List<ChapterSpawner> chapterSpawners;

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

    public void ActivateSpawner(int chapterNumber)
    {
        foreach (var spawner in chapterSpawners)
        {
            spawner.mobSpawner.gameObject.SetActive(spawner.chapterNumber == chapterNumber);
        }
    }
}
