using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform player;
    public Vector3 offset;
    public float yOffset;

    public float shakeDuration = 1f;
    public float shakeMagnitude = 0.7f;
    private float shakeTimer;
    private float nextShakeTime = 10f;

    public Vector2 shakeRange = new Vector2(0.5f, 0.5f);

    private Vector3 originalPosition; // ī�޶��� ���� ��ġ�� ����

    void Start()
    {
        /*originalPosition = transform.position;
        transform.position = new Vector3(player.position.x*2, yOffset, transform.position.z);*/
    }

    void Update()
    {
        if (shakeTimer <= 0)
        {
            // ��鸲�� ���� ���� ���������� ī�޶� �̵���Ŵ
            transform.position += new Vector3(moveSpeed * Time.deltaTime, 0, 0);
        }

        // ī�޶� ��鸲 ȿ��
        if (Time.time >= nextShakeTime)
        {
            shakeTimer = shakeDuration;
            nextShakeTime = Time.time +5f + shakeDuration;
        }

        if (shakeTimer > 0)
        {
            Vector3 shakeOffset = Random.insideUnitSphere * shakeMagnitude;
            shakeOffset.x = Mathf.Clamp(shakeOffset.x, -shakeRange.x, shakeRange.x);
            shakeOffset.y = Mathf.Clamp(shakeOffset.y, -shakeRange.y, shakeRange.y);
            shakeOffset.z = 0;

            // ��鸲�� �����Ͽ� ī�޶� ��ġ ����
            transform.position = originalPosition + shakeOffset;

            shakeTimer -= Time.deltaTime;
        }
        else
        {
            // ��鸲�� ���� ��, ���� ��ġ�� ����
            transform.position = new Vector3(transform.position.x, yOffset, transform.position.z);
            originalPosition = transform.position; // ���� ��ġ ����
        }
    }
}
