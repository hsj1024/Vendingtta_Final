using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossFlowerThrow : MonoBehaviour
{
    public GameObject flowerPrefab;
    public GameObject impactEffectPrefab; // ����Ʈ ������ �߰�

    public Transform headTransform;
    public Camera mainCamera; // ���� ī�޶�
    public Vector3 headLocalPosition; // ī�޶� ������� ��� ��ġ


    public float flowerSpeed = 20f;
    public int numberOfFlowers = 3;
    public int enhancedNumberOfFlowers = 5;
    public float flowerSpacing = 1.0f;
    private bool skillEnhanced = false;
    private bool isThrowingFlowers = false;

    public Transform bossTransform; // ������ Transform
    public Vector3 headOffset; // headTransform�� ���� ������ ����� ��ġ ������
    private Vector3 someOffset; // �� ���� ������ �����ؾ� �մϴ�.



    private BossController bossController;
    private Vector3 initialPlayerPosition;
    // �̺�Ʈ ����
    public delegate void SkillCompleted();
    public event SkillCompleted OnNormalSkillCompleted;
    public event SkillCompleted OnEnhancedSkillCompleted;
    public enum BossSkillType
    {
        GhostAttack,
        BossFlowerThrow,
        FakeAttackSkill
    }
    private bool skillInProgress = false; // ��ų ���� ���¸� ��Ÿ���� �÷���

    public bool IsSkillInProgress
    {
        get { return skillInProgress; }
    }
    /*public void ActivateEnhancedSkill()
    {
        // ��ȭ�� ��ų�� Ȱ��ȭ�ϴ� ����
        // ��: �� ���� ���� �����ϰ� ������ ����
        skillEnhanced = true;
        ThrowEnhancedFlowers();
    }
*/
    
    // ��ų �Ϸ� �̺�Ʈ�� MainGameController�� ActivateNextSkill �޼��忡 ����
    private void Start()
    {
        bossController = BossController.Instance;
        initialPlayerPosition = PlayerPosition();

        // �̺�Ʈ ����
        OnNormalSkillCompleted += FindObjectOfType<MainGameController>().ActivateNextSkill;
        OnEnhancedSkillCompleted += FindObjectOfType<MainGameController>().ActivateNextSkill;

        //ThrowFlowers();
        // ��� Ʈ�������� ī�޶��� �ڽ����� ����
        mainCamera = FindObjectOfType<MainGameController>().mainCamera;
        headOffset = new Vector3(0, 2, 1); // x, y, z �࿡ ���� ������

        headTransform.SetParent(mainCamera.transform);
        headTransform.localPosition = headLocalPosition;

        // bossTransform�� ���� GameObject�� Transform���� �����մϴ�.
        bossTransform = this.transform;

        // headTransform�� �ʱ�ȭ�մϴ�.
        UpdateHeadTransformPosition();
    }

    private void Update()
    {
        // `Update`���� ��ų�� ���� ȣ������ �ʰ�, �÷��׸� Ȯ���Ͽ� �ʿ��� �޼��带 ȣ���մϴ�.
        if (skillEnhanced && !isThrowingFlowers && !skillInProgress)
        {
            ActivateEnhancedSkill();
        }

        if (headTransform != null && bossTransform != null)
        {
            // headTransform�� ��ġ�� ������ ��ġ�� ����ϴ�.
            headTransform.position = bossTransform.position + someOffset;
        }
    }
    
    public void UpdateHeadTransformPosition()
    {
        if (headTransform != null && bossTransform != null)
        {
            // headTransform�� ��ġ�� ������ ���� ��ġ�� ����ϴ�.
            headOffset = new Vector3(0, 3, 0);
            headTransform.position = bossTransform.position + headOffset;
        }
    }
    

    public void ThrowFlowers()
    {
        if (!isThrowingFlowers && !skillEnhanced)
        {
            StartCoroutine(ThrowFlowersCoroutine());
        }
    }

    private IEnumerator ThrowFlowersCoroutine()
    {
        isThrowingFlowers = true;
        skillInProgress = true;

        for (int i = 0; i < numberOfFlowers; i++)
        {
            Vector3 flowerPosition = headTransform.position + Vector3.right * (i - (numberOfFlowers - 1) * 0.5f) * flowerSpacing;
            GameObject flower = Instantiate(flowerPrefab, flowerPosition, Quaternion.identity);

            // ���̵� �� �ִϸ��̼�
            StartCoroutine(FadeInFlower(flower));

            // ���� ������ �ڷ�ƾ ȣ��
            StartCoroutine(LaunchFlowerAfterDelay(flower, 1.5f)); // 1.5�� �Ŀ� ������
        }

        // ��� ���� ���� �Ŀ� isThrowingFlowers ���¸� ������Ʈ�մϴ�.
        yield return new WaitForSeconds(3f);
        isThrowingFlowers = false;

        yield return new WaitForSeconds(0.5f);
        skillInProgress = false;
        OnNormalSkillCompleted?.Invoke();
    }

    private IEnumerator ThrowEnhancedFlowersCoroutine()
    {
        isThrowingFlowers = true;
        skillInProgress = true;
        int numPetals = enhancedNumberOfFlowers;
        float petalAngle = 360f / numPetals;
        List<GameObject> createdFlowers = new List<GameObject>();

        for (int i = 0; i < numPetals; i++)
        {
            float angle = i * petalAngle;
            Vector3 flowerDirection = Quaternion.Euler(0, 0, angle) * Vector3.up;
            Vector3 flowerPosition = headTransform.position + flowerDirection * flowerSpacing;
            GameObject flower = Instantiate(flowerPrefab, flowerPosition, Quaternion.identity);

            // ���̵� �� �ִϸ��̼�
            StartCoroutine(FadeInFlower(flower));
            createdFlowers.Add(flower);

        }

        // ��� ���� ������ �� 1�� ���
        yield return new WaitForSeconds(1f);

        // ���� �� ���� ���������� �����ϴ�.
        foreach (GameObject flower in createdFlowers)
        {
            StartCoroutine(LaunchFlowerAfterDelay(flower, 0.5f));  // ���⼭ ������ ���� ����
            yield return new WaitForSeconds(0.5f);  // ���� �� ������ �� ������
        }

        // ��ȭ�� ���� ��� ������ �� �Ŀ� ��ų �Ϸ� �̺�Ʈ�� �߻���ŵ�ϴ�.
        isThrowingFlowers = false;
        skillInProgress = false;
        skillEnhanced = false;
        OnEnhancedSkillCompleted?.Invoke();
    }

    private IEnumerator FadeInFlower(GameObject flower)
    {
        Renderer flowerRenderer = flower.GetComponent<Renderer>();
        Material flowerMaterial = flowerRenderer.material;

        // �ʱ� ������ 0���� ����
        Color startColor = flowerMaterial.color;
        startColor.a = 0f;
        flowerMaterial.color = startColor;

        // ���̵� �� �ִϸ��̼�
        float fadeInDuration = 0.5f;
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            Color newColor = flowerMaterial.color;
            newColor.a = alpha;
            flowerMaterial.color = newColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }


    private void ThrowEnhancedFlowers()
    {
        if (skillEnhanced)
        {
            StartCoroutine(ThrowEnhancedFlowersCoroutine());

        }

    }
    


    private Vector3 PlayerPosition()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            return playerObject.transform.position;
        }
        else
        {
            return Vector3.zero;
        }
    }

    private IEnumerator LaunchFlower(GameObject flower)
    {
        Vector3 playerPosition = PlayerPosition();
        Vector3 startPosition = flower.transform.position;
        Vector3 playerDirection = (playerPosition - startPosition).normalized;
        Rigidbody2D flowerRb = flower.GetComponent<Rigidbody2D>();
        flowerRb.velocity = playerDirection * flowerSpeed;

        float angle = Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg + 90f;
        flower.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // ����Ʈ�� �߻��� ������ ����մϴ�.
        while (Vector3.Distance(flower.transform.position, playerPosition) > 0.5f)
        {
            yield return null;
        }

        // ���� ��ǥ ������ �����ϸ� ����Ʈ ����
        CreateImpactEffect(flower.transform.position);
        Destroy(flower); // �� ������Ʈ ���� (�ʿ��� ���)
    }



    private IEnumerator LaunchFlowerAfterDelay(GameObject flower, float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 playerPosition = PlayerPosition();
        Vector3 startPosition = flower.transform.position;
        Vector3 playerDirection = (playerPosition - startPosition).normalized;
        Rigidbody2D flowerRb = flower.GetComponent<Rigidbody2D>();
        flowerRb.velocity = playerDirection * flowerSpeed;

        float angle = Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg + 90f;
        flower.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // ����Ʈ�� �߻��� ������ ����մϴ�.
        while (Vector3.Distance(flower.transform.position, playerPosition) > 0.5f)
        {
            yield return null;
        }

        // ���� ��ǥ ������ �����ϸ� ����Ʈ ����
        CreateImpactEffect(flower.transform.position);
        Destroy(flower); // �� ������Ʈ ���� (�ʿ��� ���)
    }


    // ����Ʈ ���� �Լ�
    private void CreateImpactEffect(Vector3 position)
    {
        if (impactEffectPrefab != null)
        {
            GameObject impactEffect = Instantiate(impactEffectPrefab, position, Quaternion.identity);
            // ����Ʈ�� 3�� �Ŀ� �ı��ǵ��� ����
            Destroy(impactEffect, 3f);
        }
    }


    private IEnumerator EnhanceSkillAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        skillEnhanced = true;

        // ���� ���� ������ �ִ� ���°� �ƴ϶�� �ٷ� ��ȭ�� �� ������ ����
        if (!isThrowingFlowers)
        {
            ThrowEnhancedFlowers();
        }

    }

    public bool IsSkillActivated()
    {
        return isThrowingFlowers;
    }

    // �Ϲ� ��ų Ȱ��ȭ �޼���
    public void ActivateNormalSkill()
    {
        
        if (!isThrowingFlowers && !skillEnhanced && !skillInProgress)
        {
            StartCoroutine(ThrowFlowersCoroutine());
        }
    }

    // ��ȭ�� ��ų Ȱ��ȭ �޼���
    
    public void ActivateEnhancedSkill()
    {
        if (!isThrowingFlowers && !skillInProgress)
        {
            skillInProgress = true; // ��ų�� ���۵� �� �÷��׸� true�� ����

            StartCoroutine(ThrowEnhancedFlowersCoroutine());
        }
    }

    /*public void ActivateRandomSkill()
    {
        if (bossController.currentBossHealth < 50)
        {
            ActivateEnhancedSkill(); // ü���� 50 ������ �� ��ȭ�� ��ų ����
        }
        else
        {
            ActivateNormalSkill(); // �׷��� ������ �Ϲ� ��ų ����
        }
    }*/

}
