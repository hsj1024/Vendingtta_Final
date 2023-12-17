using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float moveSpeed = 5f; // ī�޶��� �̵� �ӵ�

    public Transform player; // �÷��̾��� Transform
    public float smoothSpeed = 0.125f; // ī�޶� ���󰡴� �ӵ�
    public Vector3 offset; // �÷��̾�� ī�޶� ������ ������
    public float yOffset; // �÷��̾�� ī�޶� ������ y�� ������
    private bool isFirstFrame = true;

    void Start()
    {
        transform.position = new Vector3(player.position.x, yOffset, transform.position.z);
    }
    void Update()
    {
        // ī�޶� ���������� ���� �ӵ��� �̵���Ŵ
        transform.position += new Vector3(moveSpeed * Time.deltaTime, 0, 0);
    }
    /*void LateUpdate()
    {
        if (isFirstFrame)
        {
            isFirstFrame = false;
            return;
        }
        Vector3 desiredPosition = new Vector3(player.position.x + offset.x, yOffset, offset.z);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }*/

}
