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
        target = monsterTransform; // 몬스터를 타겟으로 설정
        isCloseUp = true;
    }

    public void EndCloseUp(Transform playerTransform)
    {
        target = playerTransform; // 플레이어를 다시 타겟으로 설정
        isCloseUp = false;
        cam.orthographicSize = halfHeight * 2; // 카메라 사이즈를 원래대로 복구
    }

    void LateUpdate()
    {
        if (isCloseUp)
        {
            Vector3 midPoint = (target.position + transform.position) / 2f;
            Vector3 desiredPosition = new Vector3(midPoint.x, midPoint.y, -10f);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * closeUpSpeed);
            transform.position = smoothedPosition;

            float newSize = Mathf.Lerp(cam.orthographicSize, closeUpSize, Time.deltaTime * closeUpSpeed);
            cam.orthographicSize = newSize;
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
