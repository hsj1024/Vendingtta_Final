using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string targetSceneName; // �̵��� ���� �̸��� Inspector���� ����

    // ��ư�� Ŭ������ �� ȣ���� �޼ҵ�
    public void LoadTargetScene()
    {
        SceneManager.LoadScene(targetSceneName); // ������ ������ �̵�
        
    }
}
