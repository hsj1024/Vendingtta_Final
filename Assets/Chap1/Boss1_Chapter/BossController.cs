using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
//using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

// 보스 체력관리 스크립트
public class BossController : MonoBehaviour
{
    public static BossController Instance { get; private set; }

    [SerializeField] private float maxBossHealth = 100; // 보스의 최대 체력
    public float currentBossHealth; // 보스의 현재 체력
    public List<GameObject> bosses; // 보스 오브젝트들의 리스트
    public TextMeshProUGUI bossHealthText; // 보스 체력 UI

    public event Action OnHealthBelowFifty; // 체력이 50 이하일 때 발생하는 이벤트


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentBossHealth = maxBossHealth; // 초기 체력 설정
        UpdateBossHealthUI();
    }

    // 보스의 체력을 감소시키는 함수
    public void TakeDamage(float damage)
    {
        if (currentBossHealth > 0)
        {
            currentBossHealth -= damage; // float 값을 전달
            //currentBossHealth -= Mathf.Max(currentBossHealth, 0); // 체력이 음수가 되지 않도록 함

            // 체력이 0 이하인 경우 처리
            if (currentBossHealth == 0)
            {
                BossDefeated();
            }
            else if (currentBossHealth <= 50 && OnHealthBelowFifty != null)
            {
                OnHealthBelowFifty?.Invoke();
            }

            UpdateBossHealthUI();
        }
    }


    // 보스의 체력을 표시하는 함수
    private void UpdateBossHealthUI()
    {
        if (bossHealthText != null)
        {
            bossHealthText.text = "Boss Health: " + currentBossHealth.ToString();
        }
    }

    // 보스 사망 처리 함수
    public void BossDefeated()
    {
        SceneManager.LoadScene("Chap1/Boss1_Chapter/boss1_defeat");
    
        Debug.Log("All Bosses Defeated");

    }
}
