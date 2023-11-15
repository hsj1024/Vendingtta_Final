using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour
{
    public LineRenderer hitRangeLineRenderer; // 라인 렌더러 오브젝트

    private void Start()
    {
        // 라인 렌더러 초기에는 비활성화
        hitRangeLineRenderer.enabled = false;
    }

    private void OnEnable()
    {
        // 몬스터가 활성화될 때 라인 렌더러 활성화
        hitRangeLineRenderer.enabled = true;
        StartCoroutine(HideHitRangeAfterDelay());
    }

    private IEnumerator HideHitRangeAfterDelay()
    {
        yield return new WaitForSeconds(0.5f); // 피격 범위를 활성화한 후 0.5초 후에 비활성화
        hitRangeLineRenderer.enabled = false;
    }

    private void OnDisable()
    {
        // 몬스터가 비활성화될 때 라인 렌더러 비활성화
        hitRangeLineRenderer.enabled = false;
    }

    // 기타 몬스터 스크립트 내용...
}
