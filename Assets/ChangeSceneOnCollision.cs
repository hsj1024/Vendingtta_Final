using UnityEngine;

public class ChangeSceneOnCollision : MonoBehaviour
{
    [SerializeField] private string bossSceneName = "Chap1/Boss1_Chapter/Boss1_chapter"; // 보스 씬의 이름
    [SerializeField] private FadeTransition fadeTransition; // Fade 애니메이션 스크립트 연결

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            fadeTransition.StartFadeOut(bossSceneName);
        }
    }
}
