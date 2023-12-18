using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DieBoss2 : MonoBehaviour
{
    public Image blackImage; // 검정색 이미지
    public TextMeshProUGUI dieText; // DIE 텍스트
    public Button Title;
    public TextMeshProUGUI exit;
    public Button Retry;
    public TextMeshProUGUI retry;
    public float fadeDuration = 1.0f; // 페이드 인 시간
    private bool isDead = false; // 플레이어가 죽었는지 체크
    public GameObject bossGameObject; // 보스 게임 오브젝트에 대한 참조
    public GameObject player;
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
        // '다음 장' 버튼 클릭 시 로드할 씬
        SceneManager.LoadScene("Chapter_Select/Select"); // 여기에 보스 2 다음 장 씬 이름 입력
    }

    public void next2()
    {
        // '메인 메뉴' 버튼 클릭 시 로드할 씬
        SceneManager.LoadScene("MainMenu"); // 메인 메뉴 씬 이름 유지
    }

    public void next3()
    {
        // '재시도' 버튼 클릭 시 로드할 씬
        SceneManager.LoadScene("Chap2/Boss2"); // 여기에 보스 2 스테이지 씬 이름 입력
    }

    public void PlayerDied()
    {
        isDead = true;

        // 보스 게임 오브젝트를 비활성화
        
            bossGameObject.SetActive(false);
            player.SetActive(false);
        
    }
}
