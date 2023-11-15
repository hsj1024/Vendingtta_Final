using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FakeAttackSkill : MonoBehaviour
{
    public Text lieText; // "�������̾�" Text UI ���
    public GameObject ghostSprite; // �ͽ� ��������Ʈ
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
        // "�������̾�" �ؽ�Ʈ�� Ȱ��ȭ
        lieText.gameObject.SetActive(true);

        // �����ϰ� ��¥ �ǰ� ���� ���� �� ǥ��
        isFakeHitAreaLeft = Random.value > 0.5f; // ���� ���� ������ ����

        // ������ �ؽ�Ʈ�� ��������� �ڷ�ƾ ����
        yield return StartCoroutine(FadeOutLieText());
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
        bool isLeft = Random.value > 0.5f;
        fakeHitAreaLeft.gameObject.SetActive(isLeft);
        fakeHitAreaRight.gameObject.SetActive(!isLeft);
        yield return new WaitForSeconds(fakeHitAreaDuration);

        // �ǰ� ������ ��Ȱ��ȭ
        fakeHitAreaLeft.gameObject.SetActive(false);
        fakeHitAreaRight.gameObject.SetActive(false);

        // delayBeforeGhost �Ŀ� �ͽ��� ������ �ڷ�ƾ ����
        yield return new WaitForSeconds(delayBeforeGhost);

        // �ͽ��� ������ ��ġ ��� (�ǰ� ������ �ݴ���)
        Vector3 ghostPosition = isLeft ? fakeHitAreaRight.transform.position : fakeHitAreaLeft.transform.position;

        // �ͽ� ��������Ʈ ��ġ ���� �� ǥ��
        ghostSprite.transform.position = ghostPosition;
        ghostSprite.SetActive(true);
    }

    private void Update()
    {
        // �÷��̾ �ǰ� ���� �ȿ� �����鼭 �÷��̾ ������� ���� ���
        // ��� �÷��̾ �츲
        if (IsPlayerInsideHitArea() && !isPlayerAlive)
        {
            isPlayerAlive = true;
            ghostSprite.SetActive(false);
            player.gameObject.SetActive(true);
        }
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
    private void OnSkillCompletedInternal()
    {
        OnSkillCompleted?.Invoke();

        // ��: ���� ü���� 0�̸� ���� �й� ó��
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
