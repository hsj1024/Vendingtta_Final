using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    void Update()
    {
        // 플레이어의 입력을 받아 이동 벡터 계산
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0f).normalized;

        // 이동 벡터를 사용하여 플레이어 이동
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌한 객체의 태그를 확인하여 처리
        if (collision.gameObject.CompareTag("Boundary1"))
        {
            // 왼쪽 바운더리와 충돌한 경우, 오른쪽으로 이동 막음
            transform.position = new Vector3(-3.9f, transform.position.y, transform.position.z);
        }
        else if (collision.gameObject.CompareTag("Boundary2"))
        {
            // 오른쪽 바운더리와 충돌한 경우, 왼쪽으로 이동 막음
            transform.position = new Vector3(3.9f, transform.position.y, transform.position.z);
        }
        else if (collision.gameObject.CompareTag("Boundary3"))
        {
            // 위쪽 바운더리와 충돌한 경우, 아래로 이동 막음
            transform.position = new Vector3(transform.position.x, 3.9f, transform.position.z);
        }
        else if (collision.gameObject.CompareTag("Boundary4"))
        {
            // 아래쪽 바운더리와 충돌한 경우, 위로 이동 막음
            transform.position = new Vector3(transform.position.x, -3.9f, transform.position.z);
        }
    }
}
