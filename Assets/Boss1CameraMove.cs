using UnityEngine;

public class Boss1CameraMove : MonoBehaviour
{
    public Transform target; // �÷��̾�(Transform)�� ���⿡ �Ҵ��մϴ�.
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    public float followThreshold = 0.5f; // �÷��̾ �� �Ÿ� �̻� �־����� �� ī�޶� ���󰩴ϴ�.

    // ī�޶� �̵��� �� �ִ� �ּ� �� �ִ� ������ �߰��մϴ�.
    public Vector2 minCameraBounds;
    public Vector2 maxCameraBounds;

    private void Start()
    {
        // ī�޶��� �ʱ� ��ġ�� (0, 0, 0)���� ����
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
                    transform.position.z // z ���� �����մϴ�.
                );
            }
        }
    }
}