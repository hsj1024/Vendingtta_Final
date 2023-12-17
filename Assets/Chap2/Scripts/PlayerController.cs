using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum PlayerPosition { Top, Middle, Bottom }
    public PlayerPosition playerPosition;

    public GameObject camera; // 카메라 객체에 대한 참조

    void Update()
    {
        // 플레이어의 y축 위치에 따라 상태 업데이트
        float yPosition = transform.position.y;
        if (yPosition > 1)
        {
            playerPosition = PlayerPosition.Top;
        }
        else if (yPosition > -1)
        {
            playerPosition = PlayerPosition.Middle;
        }
        else
        {
            playerPosition = PlayerPosition.Bottom;
        }

        // 플레이어의 x좌표가 카메라의 x좌표보다 작아지지 않도록 제한
        if (transform.position.x < camera.transform.position.x)
        {
            transform.position = new Vector3(camera.transform.position.x, transform.position.y, transform.position.z);
        }
    }
}
