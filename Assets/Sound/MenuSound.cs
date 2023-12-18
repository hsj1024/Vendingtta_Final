using UnityEngine;

public class MenuSound : MonoBehaviour
{
    public AudioSource audioSource; // Inspector���� ����

    // ��ư Ŭ���� ȣ��� �޼���

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        audioSource.Play();
    }
}
