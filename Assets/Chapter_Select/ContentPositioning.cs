using UnityEngine;
using UnityEngine.UI;

public class ContentPositioning : MonoBehaviour
{
    public RectTransform content; // Content의 RectTransform 컴포넌트

    void Start()
    {
        // Content의 Anchor Min과 Anchor Max 설정
        content.anchorMin = new Vector2(0.5f, 0.5f);
        content.anchorMax = new Vector2(0.5f, 0.5f);

        // Content의 Pivot 설정
        content.pivot = new Vector2(0.5f, 0.5f);

        // Content의 Position 설정 (스크롤 뷰의 정중앙에 위치)
        content.localPosition = Vector3.zero;
    }
}
