using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    public AudioClip mainTheme;
    private AudioSource audioSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = mainTheme;
            audioSource.Play();
            Debug.Log("MusicManager instance created and music started.");
        }
        else if (instance != this)
        {
            Debug.Log("Another MusicManager instance found. Destroying this one.");
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);

        if (scene.name == "MainMenu" ||
            scene.name == "Select" ||
            scene.name == "CutScene1")
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
                Debug.Log("Continuing music playback.");
            }
        }
        else
        {
            audioSource.Stop();
            Destroy(gameObject);
            Debug.Log("Stopping music and destroying MusicManager.");
        }
    }
}
