using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string targetSceneName; // 이동할 씬의 이름을 Inspector에서 설정

    // 버튼을 클릭했을 때 호출할 메소드
    public void LoadTargetScene()
    {
        SceneManager.LoadScene(targetSceneName); // 지정된 씬으로 이동
        
    }
}
