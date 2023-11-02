using UnityEngine;

public class CharacterTrail : MonoBehaviour
{
    public GameObject trailPrefab1; // 첫 번째 잔상 프리팹
    public GameObject trailPrefab2; // 두 번째 잔상 프리팹

    private GameObject trail1; // 첫 번째 잔상 GameObject
    private GameObject trail2; // 두 번째 잔상 GameObject

    private void Start()
    {
        // 첫 번째 잔상 프리팹을 복제하여 생성
        trail1 = Instantiate(trailPrefab1, transform.position, Quaternion.identity);
        trail1.transform.parent = transform; // 캐릭터의 자식으로 설정

        // 두 번째 잔상 프리팹을 복제하여 생성
        trail2 = Instantiate(trailPrefab2, transform.position, Quaternion.identity);
        trail2.transform.parent = transform; // 캐릭터의 자식으로 설정
    }

    private void Update()
    {
        // 움직임에 따라 첫 번째 잔상을 활성화/비활성화
        if (Input.GetKey(KeyCode.W)) // 움직임 조건에 따라 변경
        {
            trail1.SetActive(true); // 움직일 때 첫 번째 잔상을 활성화
        }
        else
        {
            trail1.SetActive(false); // 정지할 때 첫 번째 잔상을 비활성화
        }

        // 움직임에 따라 두 번째 잔상을 활성화/비활성화
        if (Input.GetKey(KeyCode.S)) // 움직임 조건에 따라 변경
        {
            trail2.SetActive(true); // 움직일 때 두 번째 잔상을 활성화
        }
        else
        {
            trail2.SetActive(false); // 정지할 때 두 번째 잔상을 비활성화
        }
    }
}
