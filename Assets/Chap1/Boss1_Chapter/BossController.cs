using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
//using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

// ���� ü�°��� ��ũ��Ʈ
public class BossController : MonoBehaviour
{
    public static BossController Instance { get; private set; }

    [SerializeField] private float maxBossHealth = 100; // ������ �ִ� ü��
    public float currentBossHealth; // ������ ���� ü��
    public List<GameObject> bosses; // ���� ������Ʈ���� ����Ʈ
    public TextMeshProUGUI bossHealthText; // ���� ü�� UI

    public event Action OnHealthBelowFifty; // ü���� 50 ������ �� �߻��ϴ� �̺�Ʈ


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
        currentBossHealth = maxBossHealth; // �ʱ� ü�� ����
        UpdateBossHealthUI();
    }

    // ������ ü���� ���ҽ�Ű�� �Լ�
    public void TakeDamage(float damage)
    {
        if (currentBossHealth > 0)
        {
            currentBossHealth -= damage; // float ���� ����
            //currentBossHealth -= Mathf.Max(currentBossHealth, 0); // ü���� ������ ���� �ʵ��� ��

            // ü���� 0 ������ ��� ó��
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


    // ������ ü���� ǥ���ϴ� �Լ�
    private void UpdateBossHealthUI()
    {
        if (bossHealthText != null)
        {
            bossHealthText.text = "Boss Health: " + currentBossHealth.ToString();
        }
    }

    // ���� ��� ó�� �Լ�
    public void BossDefeated()
    {
        SceneManager.LoadScene("Chap1/Boss1_Chapter/boss1_defeat");
    
        Debug.Log("All Bosses Defeated");

    }
}
