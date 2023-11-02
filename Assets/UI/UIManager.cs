using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;  // 싱글턴 패턴을 위한 static instance 변수
    public TextMeshProUGUI Coin_Count;
    public int coinCount = 0;

    void Awake()  // Awake 함수에서 instance를 초기화합니다.
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

    public void AddCoin(int amount = 1)  // 코인을 추가하거나 감소시키는 기능을 하나의 함수에서 처리
    {
        //Debug.Log("AddCoin called");
        coinCount += amount;
        if (coinCount < 0) coinCount = 0;  // 코인이 음수가 되지 않도록 처리
        UpdateCoinText();
    }

    public int GetCoinCount()  // 현재 코인 수를 반환하는 getter 함수
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
