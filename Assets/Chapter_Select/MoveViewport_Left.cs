using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveViewport_Left : MonoBehaviour
{
    public ScrollRect scrollRect; // 이동할 ScrollRect
    public float moveAmount = 0.1f; // 한번에 이동할 거리 (0 ~ 1)

    // 버튼 이벤트를 위한 함수
    public void OnButtonClick()
    {
        // 현재 ScrollRect의 위치 가져오기
        float currentPos = scrollRect.horizontalNormalizedPosition;
        // ScrollRect의 위치를 왼쪽으로 이동
        scrollRect.horizontalNormalizedPosition = Mathf.Clamp(currentPos - moveAmount, 0f, 1f);
    }
}
