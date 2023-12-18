using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System; // 이 줄을 추가하세요

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
    public GameObject boss1;
    public GameObject boss2;
    public GameObject boss3;
    public GameObject boss4;
    public GameObject player;
    public MainGameController mainGameController; // MainGameController에 대한 참조


    public static event Action PlayerDiedEvent; // 죽음 이벤트

    void Update()
    {
        /*if (isDead)
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
        }*/
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
        SceneManager.LoadScene("Chap1/Boss1_Chapter/Boss1_chapter");
    }
    public void DeactivateAllGameObjectsWithTag(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject obj in objects)
        {
            obj.SetActive(false);
        }
    }
    public void PlayerDied()
    {
        isDead = true;
        SceneManager.LoadScene("Chap1/Boss1_Chapter/boss1_defeat");





    }
}
