using System.Collections;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform target; // �⺻ Ÿ�� (��: �÷��̾�)
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
    private Vector3 originalPosition; // ī�޶��� ���� ��ġ
    private Transform closeUpTarget; // �����ų ��� ����
    private Transform player; // �÷��̾��� Transform

    void Start()
    {
        cam = GetComponent<Camera>();
        halfHeight = cam.orthographicSize;
        halfWidth = halfHeight * cam.aspect;

        originalSize = cam.orthographicSize; // �ʱ� ī�޶� ũ��
        originalPosition = transform.position; // �ʱ� ī�޶� ��ġ

        // �÷��̾��� Transform�� ã�� �ʱ�ȭ
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

        // �⺻ ī�޶� ���� ����
        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, -5f);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * speed);

        // max/min ��谪 ����
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
        // �����ų ���·� ����
        isCloseUp = true;
        closeUpTarget = monsterTransform;
        target = monsterTransform;

        // ī�޶� Ȯ��
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

        // �����ų ���� ����
        isCloseUp = false;
        target = player; // Ÿ���� �÷��̾�� ����
    }

    private void ResetCameraToOriginalState()
    {
        // ī�޶� ũ��� ��ġ�� ������� ����
        cam.orthographicSize = originalSize;
        transform.position = originalPosition;

        // �����ų ���¿� ���õ� ������ �ʱ�ȭ
        closeUpTarget = null;
        isCloseUp = false;

        // ī�޶� Ÿ���� �÷��̾�� ����
        target = player;
    }
}*/