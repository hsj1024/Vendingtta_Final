using UnityEngine;

public class cable : MonoBehaviour
{
    public float speed = 5f;
    public Vector3 targetPosition;
    public GameObject boss; // ���� ��ü ����
    public GameObject player; // �÷��̾� ��ü ����
    public float step = 1f; // ������ �÷��̾� ���� �Ÿ��� ���̴� �ܰ� ũ��
    void Update()
    {
        // ���̺��� ��ǥ ��ġ�� �̵�
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // �ָ� ���ư��� ���� ��Ȯ�� ��ġ �񱳴� ����
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            // �÷��̾�� ������ �Ÿ��� ���̴� ����
            ReduceDistanceBetweenPlayerAndBoss();

            Destroy(gameObject);
        }
    }

    void ReduceDistanceBetweenPlayerAndBoss()
    {
        // ������ �÷��̾� ������ �Ÿ��� ���̴� ���� ����
        boss.transform.position = Vector3.MoveTowards(boss.transform.position, player.transform.position, step);
    }
}
