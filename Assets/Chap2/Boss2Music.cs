using UnityEngine;

public class Boss2Music : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play(); // �� ���� �� ���� ���
    }

    // �ʿ��� ��� �߰� �޼ҵ� �ۼ�
}
