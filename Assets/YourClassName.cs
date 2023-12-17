using UnityEngine;

public class YourClassName : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("RedMonster"))
        {
            Destroy(other.gameObject);
        }
    }
}
