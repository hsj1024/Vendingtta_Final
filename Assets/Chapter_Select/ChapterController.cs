using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterController : MonoBehaviour
{
    public Button[] chapterButtons; // ��ư���� �迭�Դϴ�.
    public Button nextButton; // "����" ��ư�Դϴ�.
    public Button previousButton; // "����" ��ư�Դϴ�.
    public Transform[] cameraPositions; // ī�޶� �̵��� ��ġ���� �迭�Դϴ�.
    private int currentButtonIndex = 0; // ���� Ȱ��ȭ�� ��ư�� �ε����Դϴ�.

    private void Start()
    {
        // "����" ��ư�� "����" ��ư�� �̺�Ʈ�� �����մϴ�.
        nextButton.onClick.AddListener(ShowNextButton);
        previousButton.onClick.AddListener(ShowPreviousButton);
    }

    void ShowNextButton()
    {
        // ���� ��ư�� �ִٸ� Ȱ��ȭ�մϴ�.
        if (currentButtonIndex < chapterButtons.Length - 1)
        {
            currentButtonIndex++;
            chapterButtons[currentButtonIndex].gameObject.SetActive(true);
            MoveCamera();
        }
    }

    void ShowPreviousButton()
    {
        // ���� ��ư�� �ִٸ� Ȱ��ȭ�մϴ�.
        if (currentButtonIndex > 0)
        {
            currentButtonIndex--;
            chapterButtons[currentButtonIndex].gameObject.SetActive(true);
            MoveCamera();
        }
    }

    void MoveCamera()
    {
        // ���� ī�޶��� X�� Y ��ġ�� �����ϰ�, Z ��ġ�� �����մϴ�.
        Vector3 newPosition = cameraPositions[currentButtonIndex].position;
        newPosition.z = Camera.main.transform.position.z;
        Camera.main.transform.position = newPosition;
    }

}
