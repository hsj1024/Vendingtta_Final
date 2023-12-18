using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Start : MonoBehaviour
{
    // 버튼 클릭에 반응하는 메서드
    public void LoadChapterSelection()
    {
        StartCoroutine(LoadSceneAfterDelay("Chapter_Select/Select", 0.5f));
    }

    // 지연 후 씬을 로드하는 코루틴
    IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
