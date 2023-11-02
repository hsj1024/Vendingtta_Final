using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;  // �̱��� ������ ���� static instance ����
    public TextMeshProUGUI Coin_Count;
    public int coinCount = 0;

    void Awake()  // Awake �Լ����� instance�� �ʱ�ȭ�մϴ�.
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        GameObject coinCountObj = GameObject.FindWithTag("Coin_Count");
        if (coinCountObj != null)
        {
            Coin_Count = coinCountObj.GetComponent<TextMeshProUGUI>();
            if (Coin_Count == null)
            {
                //Debug.LogError("Text component not found on the GameObject with tag Coin_Count");
            }
            else
            {
                UpdateCoinText();
            }
        }
        else
        {
            //Debug.LogError("No GameObject with tag Coin_Count found");
        }
    }

    public void AddCoin(int amount = 1)  // ������ �߰��ϰų� ���ҽ�Ű�� ����� �ϳ��� �Լ����� ó��
    {
        //Debug.Log("AddCoin called");
        coinCount += amount;
        if (coinCount < 0) coinCount = 0;  // ������ ������ ���� �ʵ��� ó��
        UpdateCoinText();
    }

    public int GetCoinCount()  // ���� ���� ���� ��ȯ�ϴ� getter �Լ�
    {
        return coinCount;
    }

    public void UpdateCoinText()
    {
        Debug.Log("Updating coin text");
        if (Coin_Count != null)
        {
            Coin_Count.text = "X " + coinCount;
        }
        else
        {
            //Debug.LogError("coinText is not assigned");
        }
    }
}
