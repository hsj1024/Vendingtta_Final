using UnityEngine;
using System.Collections;

public class BossFlowerThrow : MonoBehaviour
{
    public GameObject flowerPrefab;
    public Transform headTransform;
    public float flowerSpeed = 20f;
    public int numberOfFlowers = 3;
    public int enhancedNumberOfFlowers = 5;
    public float flowerSpacing = 1.0f;
    private bool skillEnhanced = false;
    private bool isThrowingFlowers = false;

    public delegate void SkillCompleted();
    public event SkillCompleted OnSkillCompleted;
    private BossController bossController;
    public enum BossSkillType
    {
        GhostAttack,
        BossFlowerThrow,
        FakeAttackSkill
    }

    public void ActivateEnhancedSkill()
    {
        // ��ȭ�� ��ų�� Ȱ��ȭ�ϴ� ����
        // ��: �� ���� ���� �����ϰ� ������ ����
        skillEnhanced = true;
        ThrowEnhancedFlowers();
    }

    private void Start()
    {
        // BossController �̱��� �ν��Ͻ��� ����
        //bossController = BossController.Instance;
        // ���� ���� �� �⺻ �� ������ ����
        ThrowFlowers();

        
    }

    private void Update()
    {
        if (skillEnhanced & !isThrowingFlowers)
        {

            Debug.Log("Flower Skill completed, spawning next boss...");
            

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

        for (int i = 0; i < numberOfFlowers; i++)
        {
            Vector3 flowerPosition = headTransform.position + Vector3.right * (i - (numberOfFlowers - 1) * 0.5f) * flowerSpacing;
            GameObject flower = Instantiate(flowerPrefab, flowerPosition, Quaternion.identity);
            StartCoroutine(LaunchFlower(flower));
        }

        // ��� ���� ���� �Ŀ� isThrowingFlowers ���¸� ������Ʈ�մϴ�.
        yield return new WaitForSeconds(3f); // �� �ð��� ���� ��� ������ �� �ɸ��� �ð��� ������� �����ؾ� �մϴ�.
        isThrowingFlowers = false;

        // ��ų�� ��ȭ�� ���¶�� ��ȭ�� �� ������ ����
        if (skillEnhanced)
        {
            ThrowEnhancedFlowers();
        }
    }

    private void ThrowEnhancedFlowers()
    {
        if (skillEnhanced)
        {
                StartCoroutine(ThrowEnhancedFlowersCoroutine());
            
        }
        
    }
    private IEnumerator ThrowEnhancedFlowersCoroutine()
    {
        isThrowingFlowers = true;

        int numPetals = enhancedNumberOfFlowers;
        float petalAngle = 360f / numPetals;

        for (int i = 0; i < numPetals; i++)
        {
            float angle = i * petalAngle;
            Vector3 flowerPosition = headTransform.position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * flowerSpacing;
            GameObject flower = Instantiate(flowerPrefab, flowerPosition, Quaternion.identity);
            StartCoroutine(LaunchFlowerAfterDelay(flower, i * 0.5f));
        }

        // ��� ��ȭ�� ���� ���� �� ��ٸ��ϴ�.
        yield return new WaitForSeconds(numPetals * 0.5f + 1f);

        isThrowingFlowers = false;

        // ��ȭ�� ���� ��� ������ �� �Ŀ� ��ų �Ϸ� �̺�Ʈ�� �߻���ŵ�ϴ�.
        OnSkillCompletedInternal();
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
            Vector3 playerDirection = (PlayerPosition() - flower.transform.position).normalized;
            Rigidbody2D flowerRb = flower.GetComponent<Rigidbody2D>();
            flowerRb.velocity = playerDirection * flowerSpeed;

            float angle = Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg + 90f;
            flower.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            yield return null;
        }

        private IEnumerator LaunchFlowerAfterDelay(GameObject flower, float delay)
        {
            yield return new WaitForSeconds(delay);

            Vector3 playerDirection = (PlayerPosition() - flower.transform.position).normalized;
            Rigidbody2D flowerRb = flower.GetComponent<Rigidbody2D>();
            flowerRb.velocity = playerDirection * flowerSpeed;

            float angle = Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg + 90f;
            flower.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
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

        private void OnSkillCompletedInternal()
        {
            OnSkillCompleted?.Invoke();
            skillEnhanced = false; // ��ų ��Ȱ��ȭ
        }

        public void ActivateRandomSkill()
        {
            if (bossController != null && bossController.currentBossHealth <= 50)
            {
                // ü���� 50 ������ �� ��ȭ�� ��ų ����
                ThrowEnhancedFlowers();
            }
            else
            {
                // ü���� 50 �̻��� �� �Ϲ� ��ų ����
                ThrowFlowers();
            }
        }



    /*public void ActivateRandomSkill()
    {
        if (skillEnhanced || bossController.CurrentHealth <= 50)
        {
            // ü���� 50 ���ϰų� ��ų�� �̹� ��ȭ�� ��� ��ȭ�� ��ų ����
            ThrowEnhancedFlowers();
        }
        else
        {
            // ü���� 50 �̻��� �� �Ϲ� ��ų ����
            ThrowFlowers();
        }
    }*/


}
