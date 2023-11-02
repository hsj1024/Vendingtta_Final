using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterController : MonoBehaviour
{
    public Button[] chapterButtons; // 버튼들의 배열입니다.
    public Button nextButton; // "다음" 버튼입니다.
    public Button previousButton; // "이전" 버튼입니다.
    public Transform[] cameraPositions; // 카메라가 이동할 위치들의 배열입니다.
    private int currentButtonIndex = 0; // 현재 활성화된 버튼의 인덱스입니다.

    private void Start()
    {
        // "다음" 버튼과 "이전" 버튼에 이벤트를 연결합니다.
        nextButton.onClick.AddListener(ShowNextButton);
        previousButton.onClick.AddListener(ShowPreviousButton);
    }

    void ShowNextButton()
    {
        // 다음 버튼이 있다면 활성화합니다.
        if (currentButtonIndex < chapterButtons.Length - 1)
        {
            currentButtonIndex++;
            chapterButtons[currentButtonIndex].gameObject.SetActive(true);
            MoveCamera();
        }
    }

    void ShowPreviousButton()
    {
        // 이전 버튼이 있다면 활성화합니다.
        if (currentButtonIndex > 0)
        {
            currentButtonIndex--;
            chapterButtons[currentButtonIndex].gameObject.SetActive(true);
            MoveCamera();
        }
    }

    void MoveCamera()
    {
        // 메인 카메라의 X와 Y 위치만 변경하고, Z 위치는 유지합니다.
        Vector3 newPosition = cameraPositions[currentButtonIndex].position;
        newPosition.z = Camera.main.transform.position.z;
        Camera.main.transform.position = newPosition;
    }

}
