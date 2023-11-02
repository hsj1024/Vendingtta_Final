using UnityEngine;
using UnityEngine.UI;

public class move_bar : MonoBehaviour
{
    public RectTransform miniPlayerRect; // Mini �÷��̾��� RectTransform
    public Slider moveBar;
    public Transform player;
    public Transform startPoint; // ���� ���� ��ġ
    public Transform endPoint;   // ���� �� ��ġ

    void Start()
    {
        moveBar.value = 0; // �����̴��� ���� ���������� ����
        miniPlayerRect.anchoredPosition = new Vector2(0, miniPlayerRect.anchoredPosition.y); // miniPlayerRect�� �����̴��� �������� ��ġ��ŵ�ϴ�.
        //Debug.Log("Start: moveBar.value set to " + moveBar.value);
    }

    void Update()
    {
       // Debug.Log("Update: Before UpdateMoveBar - moveBar.value is " + moveBar.value);
        UpdateMoveBar();
        //Debug.Log("Update: After UpdateMoveBar - moveBar.value is " + moveBar.value);
    }

    void UpdateMoveBar()
    {
        if (player == null || moveBar == null || miniPlayerRect == null)
        {
            Debug.LogError("Player, MoveBar, or MiniPlayerRect is not assigned!");
            return;
        }

        float totalDistance = Vector2.Distance(startPoint.position, endPoint.position);
        float playerDistance = Vector2.Distance(startPoint.position, player.position);


        // �÷��̾ ���� ���� �Ѿ ��츦 ����Ͽ� playerDistance ���� �����մϴ�.
        playerDistance = Mathf.Clamp(playerDistance, 0, totalDistance);
       // Debug.Log("UpdateMoveBar: Clamped playerDistance = " + playerDistance);

        // progress ���� ����ϰ�, 0�� 1 ���̷� �����մϴ�.
        float progress = Mathf.Clamp(playerDistance / totalDistance, 0, 1);
        //Debug.Log("UpdateMoveBar: progress = " + progress);

        moveBar.value = progress;
        //Debug.Log("UpdateMoveBar: moveBar.value set to " + moveBar.value);

        // Mini �÷��̾��� ��ġ�� ������Ʈ
        miniPlayerRect.anchoredPosition = new Vector2(progress * moveBar.GetComponent<RectTransform>().sizeDelta.x, miniPlayerRect.anchoredPosition.y);
    }
}
