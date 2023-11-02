using UnityEngine;
using UnityEngine.UI;

public class move_bar : MonoBehaviour
{
    public RectTransform miniPlayerRect; // Mini 플레이어의 RectTransform
    public Slider moveBar;
    public Transform player;
    public Transform startPoint; // 맵의 시작 위치
    public Transform endPoint;   // 맵의 끝 위치

    void Start()
    {
        moveBar.value = 0; // 슬라이더의 값을 시작점으로 설정
        miniPlayerRect.anchoredPosition = new Vector2(0, miniPlayerRect.anchoredPosition.y); // miniPlayerRect를 슬라이더의 시작점에 위치시킵니다.
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


        // 플레이어가 맵의 끝을 넘어간 경우를 대비하여 playerDistance 값을 제한합니다.
        playerDistance = Mathf.Clamp(playerDistance, 0, totalDistance);
       // Debug.Log("UpdateMoveBar: Clamped playerDistance = " + playerDistance);

        // progress 값을 계산하고, 0과 1 사이로 제한합니다.
        float progress = Mathf.Clamp(playerDistance / totalDistance, 0, 1);
        //Debug.Log("UpdateMoveBar: progress = " + progress);

        moveBar.value = progress;
        //Debug.Log("UpdateMoveBar: moveBar.value set to " + moveBar.value);

        // Mini 플레이어의 위치를 업데이트
        miniPlayerRect.anchoredPosition = new Vector2(progress * moveBar.GetComponent<RectTransform>().sizeDelta.x, miniPlayerRect.anchoredPosition.y);
    }
}
