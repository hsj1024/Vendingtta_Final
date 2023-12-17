// FlowerHit.cs
using UnityEngine;

public class FlowerHit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player hit by a flower");
            // 플레이어에게 피격 처리를 여기서 수행
        }
    }
}
