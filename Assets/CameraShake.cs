using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float shakeDuration = 1f; // 카메라 흔들림 지속 시간
    public float shakeMagnitude = 0.7f; // 카메라 흔들림 강도
    public Vector2 shakeRange = new Vector2(0.5f, 0.5f); // 흔들림 범위 제한 (x, y축)

    private float shakeTimer;
    private float nextShakeTime = 10f; // 다음 흔들림까지의 시간

    void Update()
    {
        // 카메라 흔들림 효과
        if (Time.time >= nextShakeTime)
        {
            shakeTimer = shakeDuration;
            nextShakeTime = Time.time + 10f + shakeDuration;
        }

        if (shakeTimer > 0)
        {
            Vector3 shakeOffset = Random.insideUnitSphere * shakeMagnitude;

            // 흔들림 범위 제한 적용
            shakeOffset.x = Mathf.Clamp(shakeOffset.x, -shakeRange.x, shakeRange.x);
            shakeOffset.y = Mathf.Clamp(shakeOffset.y, -shakeRange.y, shakeRange.y);
            shakeOffset.z = -10;

            transform.localPosition = shakeOffset;
            shakeTimer -= Time.deltaTime;
        }
        else
        {
            transform.localPosition = Vector3.zero; // 흔들림이 끝나면 원래 위치로 복귀
        }
    }
}
