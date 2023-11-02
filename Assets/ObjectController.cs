using System.Collections;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    public float bounceForce = 1000f; // 튕겨나가는 힘
    public float destroyDelay = 1f; // 파괴되기까지의 지연 시간
    private bool hasCollidedWithPlayer = false; // 플레이어와 충돌했는지 확인하는 플래그

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Coffee"))
        {
            return; // 커피캔과 충돌했을 때 아무것도 하지 않고 반환
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
