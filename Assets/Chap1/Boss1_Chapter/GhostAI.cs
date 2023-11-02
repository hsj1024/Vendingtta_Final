using UnityEngine;

public class GhostAI : MonoBehaviour
{
    private Transform target; // 공격 대상 (플레이어)
    private Vector3 moveDirection; // 이동 방향
    private bool isPaused = false; // 움직임 일시 중지 여부

    public float moveSpeed = 5f; // 이동 속도
    public Vector3 originalPosition { get; private set; }
    private const float PositionTolerance = 0.1f;


    public float moveTime // 움직임 시간 계산
    {
        get
        {
            if (moveDirection != Vector3.zero)
            {
                float distance = Vector3.Distance(transform.position, target.position);
                return distance / moveSpeed;
            }
            return 0f;
        }
    }

    private void Update()
    {
        // 대상 방향으로 이동 (일시 중지 상태가 아닐 때만)
        if (target != null && !isPaused)
        {
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetMoveDirection(Vector3 direction)
    {
        moveDirection = direction;
    }

    // 움직임 일시 중지 함수
    public void PauseMovement()
    {
        isPaused = true;
    }

    // 움직임 재개 함수
    public void ResumeMovement()
    {
        isPaused = false;
    }

    // GhostAI 클래스 내부

    public bool AtOriginalPosition()
    {
        return Vector3.Distance(transform.position, originalPosition) <= PositionTolerance;

    }

}
