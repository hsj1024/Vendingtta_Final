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

    private Vector3 originalPosition; // 카메라의 원래 위치를 저장

    void Start()
    {
        /*originalPosition = transform.position;
        transform.position = new Vector3(player.position.x*2, yOffset, transform.position.z);*/
    }

    void Update()
    {
        if (shakeTimer <= 0)
        {
            // 흔들림이 없을 때는 정상적으로 카메라를 이동시킴
            transform.position += new Vector3(moveSpeed * Time.deltaTime, 0, 0);
        }

        // 카메라 흔들림 효과
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

            // 흔들림을 적용하여 카메라 위치 변경
            transform.position = originalPosition + shakeOffset;

            shakeTimer -= Time.deltaTime;
        }
        else
        {
            // 흔들림이 끝난 후, 원래 위치로 복원
            transform.position = new Vector3(transform.position.x, yOffset, transform.position.z);
            originalPosition = transform.position; // 원래 위치 갱신
        }
    }
}
