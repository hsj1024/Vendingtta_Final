using UnityEngine;
using System;

using System.Collections;

public class MainGameController : MonoBehaviour
{
    public GameObject boss1;
    public GameObject boss2;
    public GameObject boss3;
    public GameObject boss4; // FlowerRain ��ų�� ���� ����

    
    private GhostAttack ghostAttack;
    private BossFlowerThrow bossFlowerThrow;
    private FakeAttackSkill fakeAttackSkill;
    private FlowerRain flowerRain; // FlowerRain ��ų ��ũ��Ʈ ����

    private float nextSkillTime = 7f; // ���� ��ų Ȱ��ȭ �ð�
    private float skillInterval = 1f; // ��ų Ȱ��ȭ ����

    private BossController bossController;
    public Transform leftPosition; // ���� ��ġ
    public Transform rightPosition; // ������ ��ġ

    private bool fakeAttackActivatedAt20 = false; // ü�� 20 ������ �� FakeAttackSkill �ߵ� ����
    private bool fakeAttackActivatedAt50 = false; // ü�� 50 �̻��� �� FakeAttackSkill �ߵ� ����
    private GameObject activeSkill;
    private bool isFakeAttackActive = false;
    public Camera mainCamera; // ���� ���� ī�޶�
    public Vector3 bossViewportPosition = new Vector3(0.5f, 0.5f, 10f); // Z ���� ī�޶�κ��� ���������� �Ÿ��Դϴ�.

    IEnumerator FadeIn(GameObject objectToFade, float duration)
    {
        SpriteRenderer spriteRenderer = objectToFade.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // �ʱ� ������ 0���� �����մϴ�.
            Color color = spriteRenderer.color;
            color.a = 0;
            spriteRenderer.color = color;

            // ���̵� �� ������ �ð��� �����մϴ�.
            float counter = 0;

            while (counter < duration)
            {
                counter += Time.deltaTime;
                // alpha ���� ���� �������� ���̵� �� ȿ���� ����ϴ�.
                color.a = Mathf.Lerp(0, 1, counter / duration);
                spriteRenderer.color = color;

                // ���� �����ӱ��� ����մϴ�.
                yield return null;
            }
        }
    }
    IEnumerator FadeOut(GameObject objectToFade, float duration, Action onComplete = null)
    {
        SpriteRenderer spriteRenderer = objectToFade.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            float counter = 0;
            Color startColor = spriteRenderer.color;

            while (counter < duration)
            {
                counter += Time.deltaTime;
                float alpha = Mathf.Lerp(1, 0, counter / duration);
                Color color = spriteRenderer.color;
                color.a = alpha;
                spriteRenderer.color = color;
                yield return null;
            }

            objectToFade.SetActive(false);
            onComplete?.Invoke(); // �ݹ� ȣ��
        }
    }




    // ������ ī�޶� ����Ʈ�� ���� ���� ��ġ�� Ȱ��ȭ�ϴ� �޼���
    // ������ ī�޶� ����Ʈ ������ ������ ��ġ�� Ȱ��ȭ�ϴ� �޼���
    // ������ ī�޶� ����Ʈ ������ ���򼱻� ������ ��ġ�� Ȱ��ȭ�ϴ� �޼���
    private void ActivateBossAtViewportPosition(GameObject boss)
    {
        // ����Ʈ�� X ��ǥ�� �����ϰ� �����մϴ�.
        // ���� ���, X�� 0.1���� 0.9 ���̷� �����Ͽ� ȭ�� �¿쿡 ��ġ�ϵ��� �մϴ�.
        float randomX = UnityEngine.Random.Range(0.1f, 0.9f);
        Vector3 randomViewportPosition = new Vector3(randomX, bossViewportPosition.y, bossViewportPosition.z);

        // ���� ����Ʈ ��ǥ�� ���� ��ǥ�� ��ȯ�մϴ�.
        Vector3 worldPosition = mainCamera.ViewportToWorldPoint(randomViewportPosition);

        // ������ ���� ���� ��ǥ�� Ȱ��ȭ�մϴ�.
        boss.transform.position = worldPosition;
        boss.SetActive(true);

        // ���̵� �� �ڷ�ƾ�� �����մϴ�.
        StartCoroutine(FadeIn(boss, 1f)); // 1�� ���� ���̵� ��

        // BossFlowerThrow Ŭ������ headTransform ��ġ�� �����մϴ�.
        if (bossFlowerThrow != null)
        {
            bossFlowerThrow.UpdateHeadTransformPosition();
        }
    }


    private void Start()
    {
        bossController = BossController.Instance;


        // ��� ������ �ʱ⿡ ��Ȱ��ȭ
        boss1.SetActive(false);
        boss2.SetActive(false);
        boss3.SetActive(false);
        boss4.SetActive(false);

        // ù ��° ���� (GhostAttack) �̺�Ʈ ����
        ghostAttack = boss1.GetComponent<GhostAttack>();
        ghostAttack.OnNormalSkillCompleted += OnGhostAttackNormalSkillCompleted;
        //ghostAttack.OnEnhancedSkillCompleted += OnGhostAttackEnhancedSkillCompleted;

        // �� ��° ���� (BossFlowerThrow) �̺�Ʈ ����
        bossFlowerThrow = boss2.GetComponent<BossFlowerThrow>();
        bossFlowerThrow.OnNormalSkillCompleted += OnBossFlowerThrowNormalSkillCompleted;
        bossFlowerThrow.OnEnhancedSkillCompleted += OnBossFlowerThrowEnhancedSkillCompleted;

        // �� ��° ���� (FakeAttackSkill) �̺�Ʈ ����
        fakeAttackSkill = boss3.GetComponent<FakeAttackSkill>();
        fakeAttackSkill.OnNormalSkillCompleted += OnBossFakeAttackNormalSkillCompleted;

        // ���� ���� �� ù ��° ��ų Ȱ��ȭ
        // �� ��° ���� (FlowerRain) �̺�Ʈ ����
        flowerRain = boss4.GetComponent<FlowerRain>();
        flowerRain.OnNormalSkillCompleted += OnFlowerRainNormalSkillCompleted; // �ʿ��� ���

        ActivateNextSkill();
    }


    private void OnGhostAttackNormalSkillCompleted()
    {
        boss1.SetActive(false); // Boss1 ��Ȱ��ȭ
        activeSkill = null;
        nextSkillTime = Time.time + skillInterval; // ���� ��ų Ȱ��ȭ �ð� ����
        ActivateNextSkill(); // FadeOut�� �Ϸ�� �� ���� ��ų Ȱ��ȭ
        
    }


    /*private void OnGhostAttackEnhancedSkillCompleted()
    {
        boss1.SetActive(false); // Boss1 ��Ȱ��ȭ
        activeSkill = null;
        // ���� ��ų Ȱ��ȭ ���� �߰�
        Debug.Log("������ų Ȱ��ȭ");
        ActivateNextSkill();
    }*/

    private void OnBossFlowerThrowNormalSkillCompleted()
    {

        StartCoroutine(FadeOut(boss2, 1f)); // 1�� ���� ���̵� �ƿ� ���� �� ��Ȱ��ȭ

        //boss2.SetActive(false);
        activeSkill = null;
        // ���� ��ų Ȱ��ȭ ���� �߰�
        nextSkillTime = Time.time + skillInterval; // ���� ��ų Ȱ��ȭ �ð� ������Ʈ
        
        ActivateNextSkill();
    }

    private void OnBossFlowerThrowEnhancedSkillCompleted()
    {


        //boss2.SetActive(false);
        StartCoroutine(FadeOut(boss2, 1f)); // 1�� ���� ���̵� �ƿ� ���� �� ��Ȱ��ȭ

        activeSkill = null;

        // ���� ��ų Ȱ��ȭ ���� �߰�
        nextSkillTime = Time.time + skillInterval; // ���� ��ų Ȱ��ȭ �ð� ������Ʈ
       
        ActivateNextSkill();
    }
    private void OnBossFakeAttackNormalSkillCompleted()
    {
        

        //boss3.SetActive(false);
        isFakeAttackActive = false; // FakeAttackSkill �Ϸ�

        // FakeAttackSkill�� �ٽ� Ȱ��ȭ�� �� �ֵ��� �÷��� �缳��
        if (bossController.currentBossHealth == 50)
        {
            fakeAttackActivatedAt50 = false;
        }
        nextSkillTime = Time.time + skillInterval; // ���� ��ų Ȱ��ȭ �ð� ������Ʈ
        StartCoroutine(FadeOut(boss3, 2f));
        ActivateNextSkill();
    }


    /*private void ActivateFakeAttackSkill()
    {
        boss3.SetActive(true);
        fakeAttackSkill.ActivateNormalSkill();
        fakeAttackActivatedAt50 = true;
        isFakeAttackActive = true; // FakeAttackSkill Ȱ��ȭ
    }*/


    private void OnFlowerRainNormalSkillCompleted()
    {


        //boss4.SetActive(false);
        StartCoroutine(FadeOut(boss4, 1f)); // 1�� ���� ���̵� �ƿ� ���� �� ��Ȱ��ȭ

        activeSkill = null;
        
        // ���� ��ų Ȱ��ȭ ���� �߰�
        nextSkillTime = Time.time + skillInterval; // ���� ��ų Ȱ��ȭ �ð� ������Ʈ
        
        ActivateNextSkill();
    }
    

    private void Update()
    {
        if (ShouldActivateSkill() && activeSkill == null && !IsAnySkillInProgress())
        {
            ActivateNextSkill();
        }
    }
    private bool IsAnySkillInProgress()
    {
        // �� ��ų ��ũ��Ʈ�� ���� ���¸� Ȯ��
        return ghostAttack.IsSkillInProgress || flowerRain.IsSkillInProgress || 
               bossFlowerThrow.IsSkillInProgress;
    }
    public void ActivateNextSkill()
    {
        // ���� Ȱ��ȭ�� ��ų�� ���� FakeAttackSkill�� Ȱ��ȭ���� �ʾ��� ���� ���ο� ��ų Ȱ��ȭ
        if (Time.time >= nextSkillTime && activeSkill == null && !isFakeAttackActive)
        {
            if (bossController.currentBossHealth > 50)
            {
                //Debug.Log("active next skill log");
                ActivateRandomNormalSkill();
            }
            else if (bossController.currentBossHealth == 50 && !fakeAttackActivatedAt50)
            {
                //boss3.SetActive(true); // ����3 Ȱ��ȭ
                ActivateBossAtViewportPosition(boss3);
                fakeAttackSkill.ActivateNormalSkill();
                fakeAttackActivatedAt50 = true;
                isFakeAttackActive = true; // FakeAttackSkill Ȱ��ȭ
            }
            else if (bossController.currentBossHealth == 20 && !fakeAttackActivatedAt20)
            {
                //boss3.SetActive(true); // ����3 Ȱ��ȭ
                ActivateBossAtViewportPosition(boss3);

                fakeAttackSkill.ActivateNormalSkill();
                fakeAttackActivatedAt20 = true;
                isFakeAttackActive = true; // FakeAttackSkill Ȱ��ȭ
            }
            else if (bossController.currentBossHealth < 50)
            {
                //Debug.Log("���� ü�� 50 �̸�");
                ActivateRandomMixedSkill();
            }
            // ���� ��ų Ȱ��ȭ�� �ٷ� ȣ������ �ʰ� ��ų ������ �� ���Դϴ�.
            nextSkillTime = Time.time + skillInterval;
        }
    }

    public void OnEnable()
    {
        Die.PlayerDiedEvent += StopGameActivities;
    }

    private void OnDisable()
    {
        Die.PlayerDiedEvent -= StopGameActivities;
    }

    private void StopGameActivities()
    {
        // ���⿡ ���� Ȱ�� ���� ���� ����
        this.enabled = false;
    }
    private void ActivateRandomNormalSkill()
    {
        
        if (activeSkill == null && !isFakeAttackActive) // FakeAttackSkill�� Ȱ��ȭ���� ���� ��쿡��
        {
            float randomValue = UnityEngine.Random.value;
            if (randomValue < 0.3f) // 30% Ȯ���� FlowerRain
            {
                ActivateBossAtViewportPosition(boss4);
                flowerRain.ActivateNormalSkill();
                activeSkill = boss4;
            }
            else if (randomValue < 0.5f) // �߰� 20% Ȯ���� BossFlowerThrow
            {
                //boss2.SetActive(true);
                ActivateBossAtViewportPosition(boss2);

                bossFlowerThrow.ActivateNormalSkill();
                activeSkill = boss2;
            }
            else if (randomValue < 0.8f) // �߰� 30% Ȯ���� GhostAttack
            {
                //boss1.SetActive(true);
                ActivateBossAtViewportPosition(boss1);

                ghostAttack.ActivateNormalSkill();
                Debug.Log("��Ʈ���� normal skill ȣ���");
                activeSkill = boss1;
            }
        }

        


    }
    /*else
        {
            if (bossController.currentBossHealth == 50 && !fakeAttackActivatedAt50)
            {
                boss3.SetActive(true); // ����3 Ȱ��ȭ
                fakeAttackSkill.ActivateNormalSkill();
                fakeAttackActivatedAt50 = true;

                // Ȱ��ȭ�� ��ų�� ���� ������ �Ҵ�
                activeSkill = boss3;
            }
        }*/


    private void ActivateRandomMixedSkill()
    {
        if (activeSkill == null && !isFakeAttackActive)
        {
            float randomValue = UnityEngine.Random.value;

            if (randomValue < 0.2f) // 20% Ȯ���� FlowerRain �Ϲ� ��ų
            {
                ActivateBossAtViewportPosition(boss4);
                flowerRain.ActivateNormalSkill();
                activeSkill = boss4;
                Debug.Log("FlowerRain (Normal) activated");
            }
            else if (randomValue < 0.4f) // 20% Ȯ���� BossFlowerThrow �Ϲ� ��ų
            {
                ActivateBossAtViewportPosition(boss2);
                bossFlowerThrow.ActivateNormalSkill();
                activeSkill = boss2;
                //Debug.Log("BossFlowerThrow (Normal) activated");
            }
            else if (randomValue < 0.6f) // 20% Ȯ���� BossFlowerThrow ��ȭ ��ų
            {
                //boss2.SetActive(true);
                ActivateBossAtViewportPosition(boss2);

                bossFlowerThrow.ActivateEnhancedSkill();
                activeSkill = boss2;
                //Debug.Log("BossFlowerThrow (Enhanced) activated");
            }
            else if (randomValue < 0.9f) // 20% Ȯ���� GhostAttack �Ϲ� ��ų
            {
                //yield return new WaitForSeconds(1f);
                ActivateBossAtViewportPosition(boss1);
                ghostAttack.ActivateNormalSkill();
                activeSkill = boss1;
                //Debug.Log("GhostAttack (Normal) activated");
            }
            // �߰��� �ٸ� ��ų�̳� �⺻ ������ �߰��� �� �ֽ��ϴ�.
        }
    }




    private bool ShouldActivateSkill()
    {
        return Time.time >= nextSkillTime;
    }




}
