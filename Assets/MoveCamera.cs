using System.Collections;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform target; // 기본 타겟 (예: 플레이어)
    public float speed;

    public Vector2 minBounds;
    public Vector2 maxBounds;

    public float closeUpSize = 5f;
    public float closeUpSpeed = 5f;

    private Camera cam;
    private float halfHeight;
    private float halfWidth;

    private bool isCloseUp = false;
    private float originalSize;
    private Vector3 originalPosition; // 카메라의 원래 위치
    private Transform closeUpTarget; // 팝헤드킬 대상 몬스터
    private Transform player; // 플레이어의 Transform

    void Start()
    {
        cam = GetComponent<Camera>();
        halfHeight = cam.orthographicSize;
        halfWidth = halfHeight * cam.aspect;

        originalSize = cam.orthographicSize; // 초기 카메라 크기
        originalPosition = transform.position; // 초기 카메라 위치

        // 플레이어의 Transform을 찾아 초기화
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void Update()
    {

    }

    void LateUpdate()
    {
        if (target == null) return;

        // 기본 카메라 추적 로직
        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, -5f);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * speed);

        // max/min 경계값 적용
        halfHeight = cam.orthographicSize;
        halfWidth = halfHeight * cam.aspect;
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, minBounds.x + halfWidth, maxBounds.x - halfWidth),
            Mathf.Clamp(transform.position.y, minBounds.y + halfHeight, maxBounds.y - halfHeight),
            -5f
        );
    }
}

   /* public void StartCloseUp(Transform monsterTransform)
    {
        // 팝헤드킬 상태로 설정
        isCloseUp = true;
        closeUpTarget = monsterTransform;
        target = monsterTransform;

        // 카메라 확대
        StartCoroutine(ZoomInOnTarget());
    }

    private IEnumerator ZoomInOnTarget()
    {
        while (isCloseUp && cam.orthographicSize > closeUpSize)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, closeUpSize, Time.deltaTime * closeUpSpeed);
            yield return null;
        }
    }
}

    public void EndCloseUp()
    {
        if (target != closeUpTarget)
        {
            return;
        }

        // 팝헤드킬 상태 종료
        isCloseUp = false;
        target = player; // 타겟을 플레이어로 변경
    }

    private void ResetCameraToOriginalState()
    {
        // 카메라 크기와 위치를 원래대로 복원
        cam.orthographicSize = originalSize;
        transform.position = originalPosition;

        // 팝헤드킬 상태와 관련된 변수들 초기화
        closeUpTarget = null;
        isCloseUp = false;

        // 카메라 타겟을 플레이어로 변경
        target = player;
    }
}*/