using UnityEngine;
using System.Collections;

public class KeyboardFragment : MonoBehaviour
{
    public float moveSpeed = 10f; // �̵� �ӵ�
    public GameObject nonMotionSpritePrefab; // �� ��� ��������Ʈ ������
    public GameObject hitAreaPrefab; // �ǰ� ���� ������
    public Transform player; // �÷��̾��� Transform

    private Vector3 targetPosition;
    private bool isMoving = true;

    void Start()
    {
        // ȭ�� ���߾� ��� ���� ��ġ ����
        targetPosition = new Vector3(Camera.main.transform.position.x, 
                                     Camera.main.transform.position.y + Camera.main.orthographicSize + 1.0f, // ȭ�� ������
                                     Camera.main.transform.position.z);
    }

    void Update()
    {
        if (isMoving)
        {
            // ��ǥ ��ġ�� �̵�
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // ��ǥ ��ġ�� �����ϸ� ó��
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                // ���� ��������Ʈ ��Ȱ��ȭ
                GetComponent<SpriteRenderer>().enabled = false;

                // �ǰ� ���� �� �� ������ ���� ���� ����
                StartCoroutine(ShowHitAreaAndSpawnRainPrefab(player.position));

                isMoving = false; // �̵� ����
            }
        }
    }

    IEnumerator ShowHitAreaAndSpawnRainPrefab(Vector3 targetPosition)
    {
        // �÷��̾� ��ġ�� �ǰ� ���� ǥ��
        GameObject hitArea = Instantiate(hitAreaPrefab, targetPosition, Quaternion.identity);
        yield return new WaitForSeconds(1.5f);
        Destroy(hitArea);

        // �÷��̾� �ǰ� ���� ��ġ���� 3 ���� ������ �� ������ ����
        Vector3 rainStartPosition = new Vector3(targetPosition.x, targetPosition.y + 9, targetPosition.z);
        GameObject rainPrefab = Instantiate(nonMotionSpritePrefab, rainStartPosition, Quaternion.identity);

        // 1.5�� �Ŀ� ���̵� �ƿ� ����
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(FadeOutSprite(rainPrefab, 1.5f));
    }

    IEnumerator FadeOutSprite(GameObject spriteObject, float duration)
    {
        SpriteRenderer spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Destroy(spriteObject);
            yield break;
        }

        float counter = 0;
        Color spriteColor = spriteRenderer.color;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, counter / duration);
            spriteRenderer.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, alpha);
            yield return null;
        }

        Destroy(spriteObject);
    }


}
