using UnityEngine;

public class GhostAI : MonoBehaviour
{
    private Transform target; // ���� ��� (�÷��̾�)
    private Vector3 moveDirection; // �̵� ����
    private bool isPaused = false; // ������ �Ͻ� ���� ����

    public float moveSpeed = 5f; // �̵� �ӵ�
    public Vector3 originalPosition { get; private set; }
    private const float PositionTolerance = 0.1f;


    public float moveTime // ������ �ð� ���
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
        // ��� �������� �̵� (�Ͻ� ���� ���°� �ƴ� ����)
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

    // ������ �Ͻ� ���� �Լ�
    public void PauseMovement()
    {
        isPaused = true;
    }

    // ������ �簳 �Լ�
    public void ResumeMovement()
    {
        isPaused = false;
    }

    // GhostAI Ŭ���� ����

    public bool AtOriginalPosition()
    {
        return Vector3.Distance(transform.position, originalPosition) <= PositionTolerance;

    }

}
