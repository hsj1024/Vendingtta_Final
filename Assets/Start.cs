using UnityEngine;
using UnityEngine.SceneManagement;

public class Start : MonoBehaviour
{
    public void LoadChapterSelection()
    {
        SceneManager.LoadScene("Select");
    }
}
