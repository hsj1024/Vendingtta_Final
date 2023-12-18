using UnityEngine;

public class GlobalCustomCursor : MonoBehaviour
{
    public static GlobalCustomCursor instance; // 싱글톤 인스턴스
    public Texture2D cursorTexture; // 사용할 커서 이미지
    public Vector2 hotSpot = Vector2.zero; // 커서의 클릭 지점

    void Awake()
    {
        // 싱글톤 패턴을 사용하여 중복 생성을 방지
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 파괴되지 않도록 설정

            // 커서 설정
            Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
        }
        else
        {
           // Destroy(gameObject); // 중복 인스턴스 파괴
        }
    }
}
