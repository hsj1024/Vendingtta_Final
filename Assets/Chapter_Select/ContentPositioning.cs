using UnityEngine;
using UnityEngine.UI;

public class ContentPositioning : MonoBehaviour
{
    public RectTransform content; // Content�� RectTransform ������Ʈ

    void Start()
    {
        // Content�� Anchor Min�� Anchor Max ����
        content.anchorMin = new Vector2(0.5f, 0.5f);
        content.anchorMax = new Vector2(0.5f, 0.5f);

        // Content�� Pivot ����
        content.pivot = new Vector2(0.5f, 0.5f);

        // Content�� Position ���� (��ũ�� ���� ���߾ӿ� ��ġ)
        content.localPosition = Vector3.zero;
    }
}
