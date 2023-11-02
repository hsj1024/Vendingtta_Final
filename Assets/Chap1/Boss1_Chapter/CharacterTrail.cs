using UnityEngine;

public class CharacterTrail : MonoBehaviour
{
    public GameObject trailPrefab1; // ù ��° �ܻ� ������
    public GameObject trailPrefab2; // �� ��° �ܻ� ������

    private GameObject trail1; // ù ��° �ܻ� GameObject
    private GameObject trail2; // �� ��° �ܻ� GameObject

    private void Start()
    {
        // ù ��° �ܻ� �������� �����Ͽ� ����
        trail1 = Instantiate(trailPrefab1, transform.position, Quaternion.identity);
        trail1.transform.parent = transform; // ĳ������ �ڽ����� ����

        // �� ��° �ܻ� �������� �����Ͽ� ����
        trail2 = Instantiate(trailPrefab2, transform.position, Quaternion.identity);
        trail2.transform.parent = transform; // ĳ������ �ڽ����� ����
    }

    private void Update()
    {
        // �����ӿ� ���� ù ��° �ܻ��� Ȱ��ȭ/��Ȱ��ȭ
        if (Input.GetKey(KeyCode.W)) // ������ ���ǿ� ���� ����
        {
            trail1.SetActive(true); // ������ �� ù ��° �ܻ��� Ȱ��ȭ
        }
        else
        {
            trail1.SetActive(false); // ������ �� ù ��° �ܻ��� ��Ȱ��ȭ
        }

        // �����ӿ� ���� �� ��° �ܻ��� Ȱ��ȭ/��Ȱ��ȭ
        if (Input.GetKey(KeyCode.S)) // ������ ���ǿ� ���� ����
        {
            trail2.SetActive(true); // ������ �� �� ��° �ܻ��� Ȱ��ȭ
        }
        else
        {
            trail2.SetActive(false); // ������ �� �� ��° �ܻ��� ��Ȱ��ȭ
        }
    }
}
