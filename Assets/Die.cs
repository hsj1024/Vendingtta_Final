using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Die : MonoBehaviour
{
    public Image blackImage; // 검정색 이미지
    public TextMeshProUGUI dieText; // DIE 텍스트
    public Button Title;
    public TextMeshProUGUI exit;
    public Button Retry;
    public TextMeshProUGUI retry;
    public float fadeDuration = 1.0f; // 페이드 인 시간
    private bool isDead = false; // 플레이어가 죽었는지 체크

    void Update()
    {
        if (isDead)
        {
            // 알파값 조절
            Color imageColor = blackImage.color;
            imageColor.a += Time.deltaTime / fadeDuration;
            blackImage.color = imageColor;

            Color textColor = dieText.color;
            textColor.a += Time.deltaTime / fadeDuration;
            dieText.color = textColor;

            // Title 버튼의 텍스트 색상 조절
            Color titleTextColor = Title.GetComponentInChildren<TextMeshProUGUI>().color;
            titleTextColor.a += Time.deltaTime / fadeDuration;
            Title.GetComponentInChildren<TextMeshProUGUI>().color = titleTextColor;

            // Retry 버튼의 텍스트 색상 조절
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
