using UnityEngine;

public class ChangeSceneOnCollision : MonoBehaviour
{
    [SerializeField] private string bossSceneName = "Chap1/Boss1_Chapter/Boss1_chapter"; // ���� ���� �̸�
    [SerializeField] private FadeTransition fadeTransition; // Fade �ִϸ��̼� ��ũ��Ʈ ����

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            fadeTransition.StartFadeOut(bossSceneName);
        }
    }
}
