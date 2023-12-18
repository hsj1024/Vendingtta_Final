using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float shakeDuration = 1f; // ī�޶� ��鸲 ���� �ð�
    public float shakeMagnitude = 0.7f; // ī�޶� ��鸲 ����
    public Vector2 shakeRange = new Vector2(0.5f, 0.5f); // ��鸲 ���� ���� (x, y��)

    private float shakeTimer;
    private float nextShakeTime = 10f; // ���� ��鸲������ �ð�

    void Update()
    {
        // ī�޶� ��鸲 ȿ��
        if (Time.time >= nextShakeTime)
        {
            shakeTimer = shakeDuration;
            nextShakeTime = Time.time + 10f + shakeDuration;
        }

        if (shakeTimer > 0)
        {
            Vector3 shakeOffset = Random.insideUnitSphere * shakeMagnitude;

            // ��鸲 ���� ���� ����
            shakeOffset.x = Mathf.Clamp(shakeOffset.x, -shakeRange.x, shakeRange.x);
            shakeOffset.y = Mathf.Clamp(shakeOffset.y, -shakeRange.y, shakeRange.y);
            shakeOffset.z = -10;

            transform.localPosition = shakeOffset;
            shakeTimer -= Time.deltaTime;
        }
        else
        {
            transform.localPosition = Vector3.zero; // ��鸲�� ������ ���� ��ġ�� ����
        }
    }
}
