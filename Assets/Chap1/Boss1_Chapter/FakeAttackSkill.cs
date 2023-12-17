using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FakeAttackSkill : MonoBehaviour
{
    public Text lieText; // "�������̾�" Text UI ���
    public GameObject deathSprite; // �ͽ� ��������Ʈ
    public Transform player;
    public Image fakeHitAreaLeft; // ���� ������ ��¥ �ǰ� ���� �̹���
    public Image fakeHitAreaRight; // ������ ������ ��¥ �ǰ� ���� �̹���
    private float lieTextDuration = 2.0f; // �������̾� �ؽ�Ʈ�� ǥ�õǴ� �� �ɸ��� �ð�
    private float fakeHitAreaDuration = 1.0f; // �ǰ� ������ ǥ�õǴ� �� �ɸ��� �ð�
    private float delayBeforeGhost = 2.0f; // �ǰ� ������ ����� �� �ͽ��� ����������� ��� �ð�

    private Camera mainCamera;
    private float screenHalfWidth;
    private bool isFakeHitAreaLeft; // ���� ������ ����

    private bool isPlayerAlive = true; // �÷��̾ ����ִ��� ���θ� ��Ÿ���� ����
    public delegate void SkillCompleted();
    public event SkillCompleted OnNormalSkillCompleted;

    private BossController bossController;
    private bool isAttacking; // �� ������ Ŭ���� ���ο��� ���� ������ ���θ� �����մϴ�.

    // ��ų�� Ȱ��ȭ�Ǿ����� Ȯ���ϴ� �޼���
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
        // "�������̾�" �ؽ�Ʈ�� Ȱ��ȭ
        lieText.gameObject.SetActive(true);

        // �����ϰ� ��¥ �ǰ� ���� ���� �� ǥ��
        isFakeHitAreaLeft = Random.value > 0.5f; // ���� ���� ������ ����

        // ������ �ؽ�Ʈ�� ��������� �ڷ�ƾ ����
        yield return StartCoroutine(FadeOutLieText());


        // ���⿡�� ��ų �Ϸ� �� ó���� ���� ��� �ð��� �߰��� �� �ֽ��ϴ�.
        
    }

    private IEnumerator FadeOutLieText()
    {
        yield return new WaitForSeconds(lieTextDuration);

        // ������ �ؽ�Ʈ ��Ȱ��ȭ
        lieText.gameObject.SetActive(false);

        // �ǰ� ������ ǥ���ϴ� �ڷ�ƾ ����
        yield return StartCoroutine(ShowFakeHitArea());
    }

    private IEnumerator ShowFakeHitArea()
    {
        // ������ �ؽ�Ʈ�� ���̵� �ƿ��� ���� ������ ���
        yield return new WaitForSeconds(lieTextDuration);

        // �����ϰ� �ǰ� ������ Ȱ��ȭ (1�� ���� ������)
        isFakeHitAreaLeft = Random.value > 0.5f;
        if (isFakeHitAreaLeft)
        {
            // ���� �ǰ� ������ ���̵� �� ����
            yield return StartCoroutine(FadeInImage(fakeHitAreaLeft, fakeHitAreaDuration));
            fakeHitAreaRight.gameObject.SetActive(false); // ������ �ǰ� ���� ��Ȱ��ȭ
        }
        else
        {
            // ������ �ǰ� ������ ���̵� �� ����
            yield return StartCoroutine(FadeInImage(fakeHitAreaRight, fakeHitAreaDuration));
            fakeHitAreaLeft.gameObject.SetActive(false); // ���� �ǰ� ���� ��Ȱ��ȭ
        }

        // �ǰ� ���� ǥ�� �ð� ���Ŀ� �ǰ� ������ ��Ȱ��ȭ
        yield return new WaitForSeconds(fakeHitAreaDuration);
        fakeHitAreaLeft.gameObject.SetActive(false);
        fakeHitAreaRight.gameObject.SetActive(false);

        // delayBeforeGhost �Ŀ� �ͽ��� ������ �ڷ�ƾ ����
        yield return new WaitForSeconds(delayBeforeGhost);

        // �ͽ��� ������ ��ġ ��� (�ǰ� ������ �ݴ���)
        Vector3 ghostPosition = isFakeHitAreaLeft ? fakeHitAreaRight.transform.position : fakeHitAreaLeft.transform.position;

        // �ͽ� ��������Ʈ ��ġ ���� �� ǥ��
        deathSprite.transform.position = ghostPosition;
        yield return StartCoroutine(FadeInSprite(deathSprite, 1.0f)); // 1�� ���� ���̵� ��
        yield return new WaitForSeconds(1.0f); // �ͽ��� ǥ�õǴ� �ð�
        deathSprite.SetActive(false);

        // ��ų �Ϸ� �̺�Ʈ �߻�
        OnNormalSkillCompletedInternal();
    }


    private void Update()
    {
        // �÷��̾ �ǰ� ���� �ȿ� �����鼭 �÷��̾ ������� ���� ���
        // ��� �÷��̾ �츲
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
            // �ʱ� ������ 0���� �����մϴ�.
            Color color = spriteRenderer.color;
            color.a = 0;
            spriteRenderer.color = color;
            spriteObj.SetActive(true);

            // ���̵� �� ������ �ð��� �����մϴ�.
            float counter = 0;

            while (counter < fadeInDuration)
            {
                counter += Time.deltaTime;
                // alpha ���� ���� �������� ���̵� �� ȿ���� ����ϴ�.
                color.a = Mathf.Lerp(0, 1, counter / fadeInDuration);
                spriteRenderer.color = color;

                // ���� �����ӱ��� ����մϴ�.
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
        // �÷��̾��� X ��ǥ�� �������� �ǰ� ���� �ȿ� �ִ��� ���θ� ��ȯ
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

    // ��ų �Ϸ� �� ���� ���¿� ������ ��ġ�� ����
    

    public void ActivateRandomSkill()
    {
        StartCoroutine(ActivateFakeAttackSkill());
    }

    private void OnNormalSkillCompletedInternal()
    {
        OnNormalSkillCompleted?.Invoke();
        
    }

}
