using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform target;
    public float speed;

    public Vector2 minBounds;
    public Vector2 maxBounds;

    public float closeUpSize = 5f;
    public float closeUpSpeed = 5f;

    private Camera cam;
    private float halfHeight;
    private float halfWidth;

    private bool isCloseUp = false;

    void Start()
    {
        cam = GetComponent<Camera>();
        halfHeight = cam.orthographicSize;
        halfWidth = halfHeight * cam.aspect;
    }

    public void StartCloseUp(Transform monsterTransform)
    {
        target = monsterTransform; // ���͸� Ÿ������ ����
        isCloseUp = true;
    }

    public void EndCloseUp(Transform playerTransform)
    {
        target = playerTransform; // �÷��̾ �ٽ� Ÿ������ ����
        isCloseUp = false;
        cam.orthographicSize = halfHeight * 2; // ī�޶� ����� ������� ����
    }

    void LateUpdate()
    {
        if (isCloseUp)
        {
            // Ÿ���� ī�޶� �߽ɿ� �ε��� ����
            Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, -10f);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * closeUpSpeed);

            // ī�޶� Ȯ��/���
            float newSize = Mathf.Lerp(cam.orthographicSize, closeUpSize, Time.deltaTime * closeUpSpeed);
            cam.orthographicSize = newSize;

            // Ȯ��� ī�޶� ũ�⿡ ���� halfWidth�� halfHeight ����
            halfHeight = cam.orthographicSize;
            halfWidth = halfHeight * cam.aspect;

            // Ȯ��� ���¿����� ��谪 üũ
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minBounds.x + halfWidth, maxBounds.x - halfWidth);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minBounds.y + halfHeight, maxBounds.y - halfHeight);

            transform.position = smoothedPosition;
        }
        else
        {
            Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, -10f);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * speed);

            float clampedX = Mathf.Clamp(smoothedPosition.x, minBounds.x + halfWidth, maxBounds.x - halfWidth);
            float clampedY = Mathf.Clamp(smoothedPosition.y, minBounds.y + halfHeight, maxBounds.y - halfHeight);

            transform.position = new Vector3(clampedX, clampedY, -10f);
        }
    }
}
