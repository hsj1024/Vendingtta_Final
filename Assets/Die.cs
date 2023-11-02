using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Die : MonoBehaviour
{
    public Image blackImage; // ������ �̹���
    public TextMeshProUGUI dieText; // DIE �ؽ�Ʈ
    public Button Title;
    public TextMeshProUGUI exit;
    public Button Retry;
    public TextMeshProUGUI retry;
    public float fadeDuration = 1.0f; // ���̵� �� �ð�
    private bool isDead = false; // �÷��̾ �׾����� üũ

    void Update()
    {
        if (isDead)
        {
            // ���İ� ����
            Color imageColor = blackImage.color;
            imageColor.a += Time.deltaTime / fadeDuration;
            blackImage.color = imageColor;

            Color textColor = dieText.color;
            textColor.a += Time.deltaTime / fadeDuration;
            dieText.color = textColor;

            // Title ��ư�� �ؽ�Ʈ ���� ����
            Color titleTextColor = Title.GetComponentInChildren<TextMeshProUGUI>().color;
            titleTextColor.a += Time.deltaTime / fadeDuration;
            Title.GetComponentInChildren<TextMeshProUGUI>().color = titleTextColor;

            // Retry ��ư�� �ؽ�Ʈ ���� ����
            Color retryTextColor = Retry.GetComponentInChildren<TextMeshProUGUI>().color;
            retryTextColor.a += Time.deltaTime / fadeDuration;
            Retry.GetComponentInChildren<TextMeshProUGUI>().color = retryTextColor;
        }
    }


    public void next1()
    {
        
            SceneManager.LoadScene("Chapter_Select/Select");

    }

    public void next2()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void next3()
    {
        SceneManager.LoadScene("Boss1_Chapter/Boss1_chapter");
    }

    public void PlayerDied()
    {
        isDead = true;
    }
}
