using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour
{
    public LineRenderer hitRangeLineRenderer; // ���� ������ ������Ʈ

    private void Start()
    {
        // ���� ������ �ʱ⿡�� ��Ȱ��ȭ
        hitRangeLineRenderer.enabled = false;
    }

    private void OnEnable()
    {
        // ���Ͱ� Ȱ��ȭ�� �� ���� ������ Ȱ��ȭ
        hitRangeLineRenderer.enabled = true;
        StartCoroutine(HideHitRangeAfterDelay());
    }

    private IEnumerator HideHitRangeAfterDelay()
    {
        yield return new WaitForSeconds(0.5f); // �ǰ� ������ Ȱ��ȭ�� �� 0.5�� �Ŀ� ��Ȱ��ȭ
        hitRangeLineRenderer.enabled = false;
    }

    private void OnDisable()
    {
        // ���Ͱ� ��Ȱ��ȭ�� �� ���� ������ ��Ȱ��ȭ
        hitRangeLineRenderer.enabled = false;
    }

    // ��Ÿ ���� ��ũ��Ʈ ����...
}
