using UnityEngine;

public class Boss1CameraMove : MonoBehaviour
{
    public Transform target; // 플레이어(Transform)를 여기에 할당합니다.
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    public float followThreshold = 0.5f; // 플레이어가 이 거리 이상 멀어졌을 때 카메라가 따라갑니다.

    // 카메라가 이동할 수 있는 최소 및 최대 범위를 추가합니다.
    public Vector2 minCameraBounds;
    public Vector2 maxCameraBounds;

    private void Start()
    {
        // 카메라의 초기 위치를 (0, 0, 0)으로 설정
        transform.position = new Vector3(0, 0, -10);
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minCameraBounds.x, maxCameraBounds.x);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minCameraBounds.y, maxCameraBounds.y);

            float distanceFromTarget = Vector3.Distance(desiredPosition, transform.position);

            if (distanceFromTarget > followThreshold)
            {
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
                transform.position = new Vector3(
                    smoothedPosition.x,
                    smoothedPosition.y,
                    transform.position.z // z 축은 유지합니다.
                );
            }
        }
    }
}