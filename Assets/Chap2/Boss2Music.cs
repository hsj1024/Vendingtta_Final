using UnityEngine;

public class Boss2Music : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play(); // 씬 시작 시 음악 재생
    }

    // 필요한 경우 추가 메소드 작성
}
