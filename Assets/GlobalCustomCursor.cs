using UnityEngine;

public class GlobalCustomCursor : MonoBehaviour
{
    public static GlobalCustomCursor instance; // �̱��� �ν��Ͻ�
    public Texture2D cursorTexture; // ����� Ŀ�� �̹���
    public Vector2 hotSpot = Vector2.zero; // Ŀ���� Ŭ�� ����

    void Awake()
    {
        // �̱��� ������ ����Ͽ� �ߺ� ������ ����
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �ÿ��� �ı����� �ʵ��� ����

            // Ŀ�� ����
            Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
        }
        else
        {
           // Destroy(gameObject); // �ߺ� �ν��Ͻ� �ı�
        }
    }
}
