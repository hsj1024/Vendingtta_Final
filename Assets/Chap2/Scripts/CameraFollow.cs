using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float moveSpeed = 5f; // 카메라의 이동 속도

    public Transform player; // 플레이어의 Transform
    public float smoothSpeed = 0.125f; // 카메라가 따라가는 속도
    public Vector3 offset; // 플레이어와 카메라 사이의 오프셋
    public float yOffset; // 플레이어와 카메라 사이의 y축 오프셋
    private bool isFirstFrame = true;

    void Start()
    {
        transform.position = new Vector3(player.position.x, yOffset, transform.position.z);
    }
    void Update()
    {
        // 카메라를 오른쪽으로 일정 속도로 이동시킴
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
