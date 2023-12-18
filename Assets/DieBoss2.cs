using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DieBoss2 : MonoBehaviour
{
    public Image blackImage; // ������ �̹���
    public TextMeshProUGUI dieText; // DIE �ؽ�Ʈ
    public Button Title;
    public TextMeshProUGUI exit;
    public Button Retry;
    public TextMeshProUGUI retry;
    public float fadeDuration = 1.0f; // ���̵� �� �ð�
    private bool isDead = false; // �÷��̾ �׾����� üũ
    public GameObject bossGameObject; // ���� ���� ������Ʈ�� ���� ����
    public GameObject player;
    void Update()
    {
        if (isDead)
        {
            // ���İ� ����
            Color imageColor = blackImage.color;
            imageColor.a += Time.deltaTime / fadeDuration;
            blackImage.color = imageColor;

            Color textColor = dieText.color;
            textColor.a += Time.deltaTime / fadeDuration;
            dieText.color = textColor;

            // Title ��ư�� �ؽ�Ʈ ���� ����
            Color titleTextColor = Title.GetComponentInChildren<TextMeshProUGUI>().color;
            titleTextColor.a += Time.deltaTime / fadeDuration;
            Title.GetComponentInChildren<TextMeshProUGUI>().color = titleTextColor;

            // Retry ��ư�� �ؽ�Ʈ ���� ����
            Color retryTextColor = Retry.GetComponentInChildren<TextMeshProUGUI>().color;
            retryTextColor.a += Time.deltaTime / fadeDuration;
            Retry.GetComponentInChildren<TextMeshProUGUI>().color = retryTextColor;
        }
    }

    public void next1()
    {
        // '���� ��' ��ư Ŭ�� �� �ε��� ��
        SceneManager.LoadScene("Chapter_Select/Select"); // ���⿡ ���� 2 ���� �� �� �̸� �Է�
    }

    public void next2()
    {
        // '���� �޴�' ��ư Ŭ�� �� �ε��� ��
        SceneManager.LoadScene("MainMenu"); // ���� �޴� �� �̸� ����
    }

    public void next3()
    {
        // '��õ�' ��ư Ŭ�� �� �ε��� ��
        SceneManager.LoadScene("Chap2/Boss2"); // ���⿡ ���� 2 �������� �� �̸� �Է�
    }

    public void PlayerDied()
    {
        isDead = true;

        // ���� ���� ������Ʈ�� ��Ȱ��ȭ
        
            bossGameObject.SetActive(false);
            player.SetActive(false);
        
    }
}
