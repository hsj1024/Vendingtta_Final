using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum PlayerPosition { Top, Middle, Bottom }
    public PlayerPosition playerPosition;

    public GameObject camera; // ī�޶� ��ü�� ���� ����

    void Update()
    {
        // �÷��̾��� y�� ��ġ�� ���� ���� ������Ʈ
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

        // �÷��̾��� x��ǥ�� ī�޶��� x��ǥ���� �۾����� �ʵ��� ����
        if (transform.position.x < camera.transform.position.x)
        {
            transform.position = new Vector3(camera.transform.position.x, transform.position.y, transform.position.z);
        }
    }
}
