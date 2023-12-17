using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    void Update()
    {
        // �÷��̾��� �Է��� �޾� �̵� ���� ���
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0f).normalized;

        // �̵� ���͸� ����Ͽ� �÷��̾� �̵�
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // �浹�� ��ü�� �±׸� Ȯ���Ͽ� ó��
        if (collision.gameObject.CompareTag("Boundary1"))
        {
            // ���� �ٿ������ �浹�� ���, ���������� �̵� ����
            transform.position = new Vector3(-3.9f, transform.position.y, transform.position.z);
        }
        else if (collision.gameObject.CompareTag("Boundary2"))
        {
            // ������ �ٿ������ �浹�� ���, �������� �̵� ����
            transform.position = new Vector3(3.9f, transform.position.y, transform.position.z);
        }
        else if (collision.gameObject.CompareTag("Boundary3"))
        {
            // ���� �ٿ������ �浹�� ���, �Ʒ��� �̵� ����
            transform.position = new Vector3(transform.position.x, 3.9f, transform.position.z);
        }
        else if (collision.gameObject.CompareTag("Boundary4"))
        {
            // �Ʒ��� �ٿ������ �浹�� ���, ���� �̵� ����
            transform.position = new Vector3(transform.position.x, -3.9f, transform.position.z);
        }
    }
}
