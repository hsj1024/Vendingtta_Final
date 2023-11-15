using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FakeAttackSkill : MonoBehaviour
{
    public Text lieText; // "거짓말이야" Text UI 요소
    public GameObject ghostSprite; // 귀신 스프라이트
    public Transform player;
    public Image fakeHitAreaLeft; // 왼쪽 빨간색 가짜 피격 공간 이미지
    public Image fakeHitAreaRight; // 오른쪽 빨간색 가짜 피격 공간 이미지
    private float lieTextDuration = 2.0f; // 거짓말이야 텍스트가 표시되는 데 걸리는 시간
    private float fakeHitAreaDuration = 1.0f; // 피격 범위가 표시되는 데 걸리는 시간
    private float delayBeforeGhost = 2.0f; // 피격 범위가 사라진 후 귀신이 나오기까지의 대기 시간

    private Camera mainCamera;
    private float screenHalfWidth;
    private bool isFakeHitAreaLeft; // 전역 변수로 변경

    private bool isPlayerAlive = true; // 플레이어가 살아있는지 여부를 나타내는 변수
    public delegate void SkillCompleted();
    public event SkillCompleted OnSkillCompleted;
    private BossController bossController;

    public enum BossSkillType
    {
        GhostAttack,
        BossFlowerThrow,
        FakeAttackSkill
    }


    private void Start()
    {
        bossController = BossController.Instance;

        mainCamera = Camera.main;
        screenHalfWidth = mainCamera.aspect * mainCamera.orthographicSize;
        StartCoroutine(ActivateFakeAttackSkill());
    }

    private IEnumerator ActivateFakeAttackSkill()
    {
        // "거짓말이야" 텍스트를 활성화
        lieText.gameObject.SetActive(true);

        // 랜덤하게 가짜 피격 공간 선택 및 표시
        isFakeHitAreaLeft = Random.value > 0.5f; // 값을 전역 변수에 설정

        // 거짓말 텍스트가 사라지도록 코루틴 시작
        yield return StartCoroutine(FadeOutLieText());
    }

    private IEnumerator FadeOutLieText()
    {
        yield return new WaitForSeconds(lieTextDuration);

        // 거짓말 텍스트 비활성화
        lieText.gameObject.SetActive(false);

        // 피격 범위를 표시하는 코루틴 시작
        yield return StartCoroutine(ShowFakeHitArea());
    }

    private IEnumerator ShowFakeHitArea()
    {
        // 거짓말 텍스트의 페이드 아웃이 끝날 때까지 대기
        yield return new WaitForSeconds(lieTextDuration);

        // 랜덤하게 피격 범위를 활성화 (1초 동안 보여줌)
        bool isLeft = Random.value > 0.5f;
        fakeHitAreaLeft.gameObject.SetActive(isLeft);
        fakeHitAreaRight.gameObject.SetActive(!isLeft);
        yield return new WaitForSeconds(fakeHitAreaDuration);

        // 피격 범위를 비활성화
        fakeHitAreaLeft.gameObject.SetActive(false);
        fakeHitAreaRight.gameObject.SetActive(false);

        // delayBeforeGhost 후에 귀신이 나오는 코루틴 시작
        yield return new WaitForSeconds(delayBeforeGhost);

        // 귀신이 나오는 위치 계산 (피격 범위와 반대편)
        Vector3 ghostPosition = isLeft ? fakeHitAreaRight.transform.position : fakeHitAreaLeft.transform.position;

        // 귀신 스프라이트 위치 설정 및 표시
        ghostSprite.transform.position = ghostPosition;
        ghostSprite.SetActive(true);
    }

    private void Update()
    {
        // 플레이어가 피격 범위 안에 있으면서 플레이어가 살아있지 않은 경우
        // 즉시 플레이어를 살림
        if (IsPlayerInsideHitArea() && !isPlayerAlive)
        {
            isPlayerAlive = true;
            ghostSprite.SetActive(false);
            player.gameObject.SetActive(true);
        }
    }

    private bool IsPlayerInsideHitArea()
    {
        // 플레이어의 X 좌표를 기준으로 피격 범위 안에 있는지 여부를 반환
        float playerX = player.position.x;
        if (isFakeHitAreaLeft)
        {
            return playerX < 0;
        }
        else
        {
            return playerX > 0;
        }
    }

    // 스킬 완료 시 보스 상태에 영향을 미치는 로직
    private void OnSkillCompletedInternal()
    {
        OnSkillCompleted?.Invoke();

        // 예: 보스 체력이 0이면 보스 패배 처리
        if (bossController != null && bossController.currentBossHealth <= 0)
        {
            bossController.BossDefeated();
        }
    }

    public void ActivateRandomSkill()
    {
        StartCoroutine(ActivateFakeAttackSkill());
    }



}
