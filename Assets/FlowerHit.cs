// FlowerHit.cs
using UnityEngine;

public class FlowerHit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player hit by a flower");
            // �÷��̾�� �ǰ� ó���� ���⼭ ����
        }
    }
}
