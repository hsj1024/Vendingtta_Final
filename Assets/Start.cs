using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Start : MonoBehaviour
{
    // ��ư Ŭ���� �����ϴ� �޼���
    public void LoadChapterSelection()
    {
        StartCoroutine(LoadSceneAfterDelay("Chapter_Select/Select", 0.5f));
    }

    // ���� �� ���� �ε��ϴ� �ڷ�ƾ
    IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
