using UnityEngine;
using UnityEngine.SceneManagement;

public class Chapter1 : MonoBehaviour
{
    public void LoadChapter1()
    {
        SceneManager.LoadScene("Chap1/CutScene/CutScene1");
    }
    public void LoadChapter2()
    {
        SceneManager.LoadScene("Chap2/Boss2");
    }
}
