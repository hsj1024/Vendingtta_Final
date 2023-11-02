using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveViewport_Left : MonoBehaviour
{
    public ScrollRect scrollRect; // �̵��� ScrollRect
    public float moveAmount = 0.1f; // �ѹ��� �̵��� �Ÿ� (0 ~ 1)

    // ��ư �̺�Ʈ�� ���� �Լ�
    public void OnButtonClick()
    {
        // ���� ScrollRect�� ��ġ ��������
        float currentPos = scrollRect.horizontalNormalizedPosition;
        // ScrollRect�� ��ġ�� �������� �̵�
        scrollRect.horizontalNormalizedPosition = Mathf.Clamp(currentPos - moveAmount, 0f, 1f);
    }
}
