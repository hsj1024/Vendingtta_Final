using System.Collections;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    public float bounceForce = 1000f; // ƨ�ܳ����� ��
    public float destroyDelay = 1f; // �ı��Ǳ������ ���� �ð�
    private bool hasCollidedWithPlayer = false; // �÷��̾�� �浹�ߴ��� Ȯ���ϴ� �÷���

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Coffee"))
        {
            return; // Ŀ��ĵ�� �浹���� �� �ƹ��͵� ���� �ʰ� ��ȯ
        }
        Debug.Log("Collided with: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Player"))
        {
            hasCollidedWithPlayer = true;
            Bounce(collision.transform);
            gameObject.layer = LayerMask.NameToLayer("Monster");
        }

        else if (collision.gameObject.CompareTag("Monster"))
        {
            if (hasCollidedWithPlayer)
            {
                Bounce(collision.transform);
            }
        }
    }


    private void Update()
    {
        if (hasCollidedWithPlayer)
        {
            CheckIfOutOfCameraView();
        }
    }

    private void Bounce(Transform other)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 bounceDirection = (transform.position - other.position).normalized;
            rb.AddForce(bounceDirection * bounceForce, ForceMode2D.Impulse);
        }
    }

    private void CheckIfOutOfCameraView()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        if (!onScreen)
        {
            Destroy(gameObject);
        }
    }
}
