using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FakeAttackSkill : MonoBehaviour
{
    public Text lieText; // "거짓말이야" Text UI 요소
    public GameObject deathSprite; // 귀신 스프라이트
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
    public event SkillCompleted OnNormalSkillCompleted;

    private BossController bossController;
    private bool isAttacking; // 이 변수는 클래스 내부에서 공격 중인지 여부를 추적합니다.

    // 스킬이 활성화되었는지 확인하는 메서드
    public bool IsSkillActivated()
    {
        return isAttacking;
    }
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
        
    }
    public void ActivateNormalSkill()
    {
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


        // 여기에서 스킬 완료 후 처리를 위한 대기 시간을 추가할 수 있습니다.
        
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
        isFakeHitAreaLeft = Random.value > 0.5f;
        if (isFakeHitAreaLeft)
        {
            // 왼쪽 피격 범위에 페이드 인 적용
            yield return StartCoroutine(FadeInImage(fakeHitAreaLeft, fakeHitAreaDuration));
            fakeHitAreaRight.gameObject.SetActive(false); // 오른쪽 피격 범위 비활성화
        }
        else
        {
            // 오른쪽 피격 범위에 페이드 인 적용
            yield return StartCoroutine(FadeInImage(fakeHitAreaRight, fakeHitAreaDuration));
            fakeHitAreaLeft.gameObject.SetActive(false); // 왼쪽 피격 범위 비활성화
        }

        // 피격 범위 표시 시간 이후에 피격 범위를 비활성화
        yield return new WaitForSeconds(fakeHitAreaDuration);
        fakeHitAreaLeft.gameObject.SetActive(false);
        fakeHitAreaRight.gameObject.SetActive(false);

        // delayBeforeGhost 후에 귀신이 나오는 코루틴 시작
        yield return new WaitForSeconds(delayBeforeGhost);

        // 귀신이 나오는 위치 계산 (피격 범위와 반대편)
        Vector3 ghostPosition = isFakeHitAreaLeft ? fakeHitAreaRight.transform.position : fakeHitAreaLeft.transform.position;

        // 귀신 스프라이트 위치 설정 및 표시
        deathSprite.transform.position = ghostPosition;
        yield return StartCoroutine(FadeInSprite(deathSprite, 1.0f)); // 1초 동안 페이드 인
        yield return new WaitForSeconds(1.0f); // 귀신이 표시되는 시간
        deathSprite.SetActive(false);

        // 스킬 완료 이벤트 발생
        OnNormalSkillCompletedInternal();
    }


    private void Update()
    {
        // 플레이어가 피격 범위 안에 있으면서 플레이어가 살아있지 않은 경우
        // 즉시 플레이어를 살림
        if (IsPlayerInsideHitArea() && !isPlayerAlive)
        {
            isPlayerAlive = true;
            deathSprite.SetActive(false);
            player.gameObject.SetActive(true);
        }
    }
    private IEnumerator FadeInSprite(GameObject spriteObj, float fadeInDuration)
    {
        SpriteRenderer spriteRenderer = spriteObj.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // 초기 투명도를 0으로 설정합니다.
            Color color = spriteRenderer.color;
            color.a = 0;
            spriteRenderer.color = color;
            spriteObj.SetActive(true);

            // 페이드 인 동안의 시간을 추적합니다.
            float counter = 0;

            while (counter < fadeInDuration)
            {
                counter += Time.deltaTime;
                // alpha 값을 점차 증가시켜 페이드 인 효과를 만듭니다.
                color.a = Mathf.Lerp(0, 1, counter / fadeInDuration);
                spriteRenderer.color = color;

                // 다음 프레임까지 대기합니다.
                yield return null;
            }
        }
    }

    private IEnumerator FadeInImage(Image image, float fadeInDuration)
    {
        image.gameObject.SetActive(true);
        Color startColor = image.color;
        startColor.a = 0f;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0.3f);

        float elapsedTime = 0f;

        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            image.color = Color.Lerp(startColor, endColor, elapsedTime / fadeInDuration);
            yield return null;
        }

        image.color = endColor;
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
    

    public void ActivateRandomSkill()
    {
        StartCoroutine(ActivateFakeAttackSkill());
    }

    private void OnNormalSkillCompletedInternal()
    {
        OnNormalSkillCompleted?.Invoke();
        
    }

}
