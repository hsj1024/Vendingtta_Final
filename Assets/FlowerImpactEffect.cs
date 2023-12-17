using UnityEngine;
using System.Collections;
public class FlowerImpactEffect : MonoBehaviour
{
    public GameObject impactSpritePrefab; // ����Ʈ ��������Ʈ ������

    private bool isGrounded = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground")) // ���� �浹�� ���
        {
            isGrounded = true;
            // 3�� �Ŀ� ����Ʈ ��� �� ���� ������Ʈ �ı�
            StartCoroutine(DelayedImpact());
        }
    }

    private IEnumerator DelayedImpact()
    {
        yield return new WaitForSeconds(3f); // 3�� ���

        if (isGrounded)
        {
            PlayImpactEffect();
            Destroy(gameObject); // ����Ʈ ��� �� ����Ʈ ���� ������Ʈ �ı�
        }
    }

    private void PlayImpactEffect()
    {
        if (impactSpritePrefab != null)
        {
            // ����Ʈ ��������Ʈ�� ���� ��ġ�� �����մϴ�.
            Instantiate(impactSpritePrefab, transform.position, Quaternion.identity);
        }
    }
}
