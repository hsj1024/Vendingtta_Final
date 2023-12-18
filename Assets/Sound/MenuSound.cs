using UnityEngine;

public class MenuSound : MonoBehaviour
{
    public AudioSource audioSource; // Inspector에서 설정

    // 버튼 클릭에 호출될 메서드

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        audioSource.Play();
    }
}
