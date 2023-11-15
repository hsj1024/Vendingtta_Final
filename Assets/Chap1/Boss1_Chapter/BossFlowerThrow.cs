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
        // 강화된 스킬을 활성화하는 로직
        // 예: 더 많은 꽃을 생성하고 던지는 로직
        skillEnhanced = true;
        ThrowEnhancedFlowers();
    }

    private void Start()
    {
        // BossController 싱글톤 인스턴스를 참조
        //bossController = BossController.Instance;
        // 게임 시작 시 기본 꽃 던지기 실행
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

        // 모든 꽃을 던진 후에 isThrowingFlowers 상태를 업데이트합니다.
        yield return new WaitForSeconds(3f); // 이 시간은 꽃을 모두 던지는 데 걸리는 시간을 기반으로 설정해야 합니다.
        isThrowingFlowers = false;

        // 스킬이 강화된 상태라면 강화된 꽃 던지기 실행
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

        // 모든 강화된 꽃을 던진 후 기다립니다.
        yield return new WaitForSeconds(numPetals * 0.5f + 1f);

        isThrowingFlowers = false;

        // 강화된 꽃을 모두 던지고 난 후에 스킬 완료 이벤트를 발생시킵니다.
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

            // 현재 꽃을 던지고 있는 상태가 아니라면 바로 강화된 꽃 던지기 실행
            if (!isThrowingFlowers)
            {
                ThrowEnhancedFlowers();
            }

        }

        private void OnSkillCompletedInternal()
        {
            OnSkillCompleted?.Invoke();
            skillEnhanced = false; // 스킬 비활성화
        }

        public void ActivateRandomSkill()
        {
            if (bossController != null && bossController.currentBossHealth <= 50)
            {
                // 체력이 50 이하일 때 강화된 스킬 실행
                ThrowEnhancedFlowers();
            }
            else
            {
                // 체력이 50 이상일 때 일반 스킬 실행
                ThrowFlowers();
            }
        }



    /*public void ActivateRandomSkill()
    {
        if (skillEnhanced || bossController.CurrentHealth <= 50)
        {
            // 체력이 50 이하거나 스킬이 이미 강화된 경우 강화된 스킬 실행
            ThrowEnhancedFlowers();
        }
        else
        {
            // 체력이 50 이상일 때 일반 스킬 실행
            ThrowFlowers();
        }
    }*/


}
