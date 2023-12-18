using System.Collections;
using System.Threading;
using System.Collections.Generic;//
using UnityEditor.Experimental.GraphView;//
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;//
using UnityEngine.UI;//



public class Player : MonoBehaviour
{

    [Header("Movement Settings")]
    [SerializeField]
    private float moveSpeed = 5.0f;
    private Rigidbody2D rb;
    private Animator animator;
    private string lastDirection = "Right";

    [Header("Player Stats")]
    public HealthBar healthBar;// healthbar 연결
    public float health = 100f; // health 변수 사용
    public int coin;


    [Header("Damage Flashing")]
    private SpriteRenderer spriteRenderer;
    public float flashDuration = 1f;
    public float flashSpeed = 0.1f;
    public float flashAlpha = 0.5f;
    public int flashCount = 2;

    [Header("Invincibility")]
    public float invincibilityDuration = 1f;
    private bool isInvincible = false;

    [Header("Knockback Settings")]
    [SerializeField]
    public float knockbackStrength = 10f; // 넉백의 강도
    [SerializeField]
    public float knockbackDuration = 0.5f; // 넉백의 지속시간

    [Header("Coffee Bullet")]
    public GameObject coffeePrefab;  // Coffee Prefab
    public Transform coffeeSpawnPoint; // Coffee를 발사할 위치
    public float coffeeSpeed = 300.0f; // Coffee의 속도

    [Header("Health Recovery")]
    public GameObject healthRecoveryEffectPrefab;  // 회복 이펙트 프리팹
    public float recoveryEffectDuration = 1.0f;  // 이펙트 지속 시간

    [Header("Health Recovery Settings")]
    [SerializeField]
    public float healthRecoveryAmount = 20f;  // 회복량을 저장하는 변수

    private GameObject currentEffect;  // 현재 활성화된 회복 이펙트

    [Header("Coffee Bullet")]
    public float coffeeFireCooldown = 0.5f; // 커피 캔 발사 쿨타임
    private float lastFireTime = 0.0f; // 마지막 커피 캔 발사 시간

    public float bounceForce = 10f;
    public float destroyDelay = 2f; // 오브젝트를 파괴하기 전에 기다릴 시간 (초)

    public float yOffset = 0f;
    public float xOffset = 0f;

    public GameObject dieUI; // 이 변수에 DIE 텍스트와 이미지가 들어있는 GameObject를 연결합니다.
    public Die dieScript;

    public Mob targetMonster; // 타겟 몬스터 참조
    public Mob2 targetMonster2;
    public Mob3 targetMonster3;
    public float popHeadKillDistance = 5.0f; // 팝헤드킬 활성화 거리
    private bool isPopHeadKillActive = false;
    [Header("Pop Head Kill Settings")]
    [Tooltip("The sprite object for PopHeadKill")]
    public GameObject popHeadKillSpriteObject; 



    private void Start()
    {
        InitializeComponents();
        currentEffect = Instantiate(healthRecoveryEffectPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity, transform);
        currentEffect.SetActive(false); // 초기에는 비활성화

    }

    private void Awake()
    {
        coffeePrefab = GameObject.FindGameObjectWithTag("Coffee");
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("CoffeeSpawnPoint");

        if (coffeePrefab == null)
        {
            Debug.LogError("Coffee prefab is not set!");
        }

        if (spawnPoint != null)
        {
            coffeeSpawnPoint = spawnPoint.transform;
        }
        else
        {
            Debug.LogError("Coffee spawn point is not set!");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("obj"))
        {
            // 오브젝트의 리지드바디를 가져옵니다.
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // 오브젝트의 위치와 플레이어의 위치를 기반으로 튕겨나갈 방향을 계산합니다.
                Vector2 bounceDirection = (collision.transform.position - transform.position).normalized;

                // 리지드바디에 힘을 적용하여 오브젝트를 튕겨 나가게 합니다.
                rb.AddForce(bounceDirection * bounceForce, ForceMode2D.Impulse);

                // 일정 시간 후에 오브젝트를 파괴합니다.
                Destroy(collision.gameObject, destroyDelay);
            }
        }
    }


    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.mass = 1.0f;
        rb.drag = 1.0f;
        ResetAnimatorParameters();
    }

    private void ResetAnimatorParameters()
    {
        animator.SetBool("IsAttackingRight", false);
        animator.SetBool("IsAttackingLeft", false);
        animator.SetBool("IsWalkingRight", false);
        animator.SetBool("IsWalkingLeft", false);
    }

    private void HandleRecovery()
    {
        if (Input.GetKeyDown(KeyCode.Q) && UIManager.instance.GetCoinCount() >= 20)
        {
            RecoverHealth();
            DeductCostAndNotify();
            ActivateEffectAndDisableAfterTime();
        }
        else if (Input.GetKeyDown(KeyCode.Q) && UIManager.instance.GetCoinCount() < 20)
        {
            //Debug.Log("Not enough coins to recover health.");
        }
    }

    private void RecoverHealth()
    {
        health += healthRecoveryAmount;
        health = Mathf.Min(health, 10000f); // 체력은 100을 초과할 수 없음
        healthBar.RecoverHealth((int)healthRecoveryAmount);
    }



    private void DeductCostAndNotify()
    {
        UIManager.instance.AddCoin(-20);
        UIManager.instance.UpdateCoinText();
    }

    private void ActivateEffectAndDisableAfterTime()
    {
        if (currentEffect != null) currentEffect.SetActive(true);
        StartCoroutine(DisableEffectAfterTime(recoveryEffectDuration));
    }



    private IEnumerator DisableEffectAfterTime(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (currentEffect != null) currentEffect.SetActive(false);
    }


    private void Update()
    {
        PlayerController();

        if (isPopHeadKillActive)
        {
            AttemptPopHeadKill();
        }
        else
        {
            HandleMovement();
        }


    }

    public void SetTargetMonster(Mob newTarget)
    {
        targetMonster = newTarget;
        if (targetMonster != null)
        {
            // 팝헤드킬 활성화와 관련된 추가 로직
            isPopHeadKillActive = true;
        }
        else
        {
            // 팝헤드킬 비활성화와 관련된 추가 로직
            isPopHeadKillActive = false;
        }
    }

    public void SetTargetMonster2(Mob2 newTarget)
    {
        targetMonster2 = newTarget;
        if (targetMonster2 != null)
        {
            // 팝헤드킬 활성화와 관련된 추가 로직
            isPopHeadKillActive = true;
        }
        else
        {
            // 팝헤드킬 비활성화와 관련된 추가 로직
            isPopHeadKillActive = false;
        }
    }

    public void SetTargetMonster3(Mob3 newTarget)
    {
        targetMonster3 = newTarget;
        if (targetMonster3 != null)
        {
            // 팝헤드킬 활성화와 관련된 추가 로직
            isPopHeadKillActive = true;
        }
        else
        {
            // 팝헤드킬 비활성화와 관련된 추가 로직
            isPopHeadKillActive = false;
        }
    }

    private void AttemptPopHeadKill()
    {
        if (targetMonster != null && isPopHeadKillActive)
        {
            float distance = Vector2.Distance(transform.position, targetMonster.transform.position);
            if (distance <= popHeadKillDistance && Input.GetKeyDown(KeyCode.Space))
            {
                //Debug.Log("Space key pressed for PopHeadKill");

                StartCoroutine(PerformPopHeadKill());
            }
        }
    }

    private IEnumerator PerformPopHeadKill()
    {
        // 카메라 확대
        MoveCamera cameraScript = Camera.main.GetComponent<MoveCamera>();
        if (cameraScript != null)
        {
            cameraScript.StartCloseUp(targetMonster.transform); // 몬스터에게 카메라 확대
        }

        // 팝헤드킬 이미지 표시
        PopHeadKillImage();

        yield return new WaitForSeconds(1); // 1초 지연

        // 몬스터 사망 처리
        if (targetMonster != null)
        {
            targetMonster.Die();
        }
    }

    private void PopHeadKillImage()
    {
        StartCoroutine(FadePopHeadKillSprite());
    }

    private IEnumerator FadePopHeadKillSprite()
    {
        SpriteRenderer spriteRenderer = popHeadKillSpriteObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) yield break;

        // 스프라이트 위치를 플레이어 위치 위로 설정
        popHeadKillSpriteObject.transform.position = transform.position;

        popHeadKillSpriteObject.SetActive(true); // 스프라이트 활성화

        popHeadKillSpriteObject.SetActive(false); // 스프라이트 비활성화
    }


void AttackMonster()
    {
        if (targetMonster != null)
        {
            // 몬스터 사망 처리
            targetMonster.Die();
        }
        else
        {
            Debug.LogError("Target monster is not set.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
            GetComponent<Rigidbody2D>().AddForce(knockbackDirection * knockbackStrength, ForceMode2D.Impulse);
            // 플레이어에게 데미지를 주지 않고 넉백만 적용합니다.
        }
        else if (collision.CompareTag("Claw_right") || collision.CompareTag("Claw_left"))
        {
            // Claw에 닿았을 때 (체력 감소와 넉백 발생)
            TakeDamage(20f, collision.transform);
        }

        else if (collision.CompareTag("Slash_right") || collision.CompareTag("Slash_left"))
        {
            // Slash에 닿았을 때 (체력 감소와 넉백 발생)
            TakeDamage(20f, collision.transform);
        }

        else if (collision.CompareTag("Coin"))
        {
            //Debug.Log("Coin collision detected.");
            Destroy(collision.gameObject);
            coin++;
            UIManager.instance.AddCoin(); // UIManager의 AddCoin 함수를 호출합니다.
        }
        else if (collision.CompareTag("RedMonster"))
        {
            // 빨간 잡몹에 닿았을 때 (체력 감소와 넉백 발생)
            TakeDamage(20f, collision.transform);
        }
        else if (collision.CompareTag("flower"))
        {
            // 국화 스킬에 닿았을 때 (체력 감소와 넉백 발생)
            TakeDamage(20f, collision.transform);
        }
        else if (collision.CompareTag("Flower"))
        {
            // 국화 스킬에 닿았을 때 (체력 감소와 넉백 발생)
            TakeDamage(20f, collision.transform);
        }
        else if (collision.CompareTag("elec"))
        {
            TakeDamage(20f, collision.transform);

        }

    }
    

    private void PlayerController()
    {

        HandleRecovery();
        HandleMovement();
        HandleAttack();
    }



    private void HandleMovement()
    {
        Vector2 movement = Vector2.zero;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            movement.x = -1f;
            lastDirection = "Left";
            animator.SetBool("IsWalkingLeft", true);
            animator.SetBool("IsWalkingRight", false);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            movement.x = 1f;
            lastDirection = "Right";
            animator.SetBool("IsWalkingRight", true);
            animator.SetBool("IsWalkingLeft", false);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            movement.y = 1f;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            movement.y = -1f;
        }

        rb.velocity = movement.normalized * moveSpeed;
    }

    private void HandleAttack()
    {
        if (isPopHeadKillActive && Input.GetKeyDown(KeyCode.Space))
        {
            AttackMonster();
        }
        else
        {
            // 기존 공격 로직
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Main_attack_right") ||
                animator.GetCurrentAnimatorStateInfo(0).IsName("Main_attack_left"))
            {
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                {
                    animator.SetBool("IsAttackingRight", false);
                    animator.SetBool("IsAttackingLeft", false);

                    if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        animator.SetBool("IsWalkingLeft", true);
                    }
                    else if (Input.GetKey(KeyCode.RightArrow))
                    {
                        animator.SetBool("IsWalkingRight", true);
                    }
                }
            }
            else
            {
                if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.W)) && Time.time - lastFireTime >= coffeeFireCooldown)
                {
                    lastFireTime = Time.time;

                    if (lastDirection == "Right")
                    {
                        animator.SetBool("IsAttackingRight", true);
                        animator.SetBool("IsWalkingRight", false);
                        animator.SetBool("IsAttackingLeft", false);
                    }
                    else
                    {
                        animator.SetBool("IsAttackingLeft", true);
                        animator.SetBool("IsWalkingLeft", false);
                        animator.SetBool("IsAttackingRight", false);
                    }

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        FireCoffee();
                    }
                }
            }
        }
    }

    private void FireCoffee()
    {
        if (coffeePrefab == null || coffeeSpawnPoint == null)
        {
            Debug.LogError("Coffee prefab or spawn point is not set!");
            return;
        }

        // yOffset 값을 사용하여 커피의 초기 Y 위치를 조정
        float newYOffset = 0.8f; // 원하는 높이로 yOffset을 설정

        // 플레이어의 현재 위치를 기준으로 커피 캔을 발사할 위치를 계산합니다.
        Vector3 playerPosition = transform.position;

        // 커피 캔의 초기 위치를 설정합니다. 여기서 x 값을 설정할 수 있습니다.
        float playerWidth = 0.5f; // 플레이어의 너비 (가로 크기)를 설정하세요.
        float xOffset = playerWidth / 2.0f; // 플레이어 중심부로 x 좌표를 조정합니다.

        Vector3 initialPosition = new Vector3(playerPosition.x + xOffset, playerPosition.y + newYOffset, playerPosition.z);

        // 플레이어의 마지막 방향에 따라 커피 캔의 발사 방향을 결정합니다.
        Vector2 fireDirection = new Vector2(0, 0); // 초기화

        if (lastDirection == "Right")
        {
            fireDirection = new Vector2(1, 0); // 오른쪽으로 발사
        }
        else if (lastDirection == "Left")
        {
            fireDirection = new Vector2(-1, 0); // 왼쪽으로 발사
        }

        GameObject coffee = Instantiate(coffeePrefab, initialPosition, Quaternion.identity);
        Rigidbody2D coffeeRb = coffee.GetComponent<Rigidbody2D>();

        coffeeRb.velocity = fireDirection * coffeeSpeed;
        coffeeRb.angularVelocity = 500f; // 회전 속도 설정

        // Coffee 클래스에 발사 방향 정보를 저장합니다.
        Coffee coffeeScript = coffee.GetComponent<Coffee>();
        if (coffeeScript != null)
        {
            coffeeScript.launchDirection = fireDirection;
        }
    }



    public void TakeDamage(float damage, Transform attacker)
    {
        if (!isInvincible)
        {
            health -= damage;
            healthBar.TakeDamage((int)damage); // HealthBar의 TakeDamage 메서드 호출 only when player is not invincible

            StartCoroutine(FlashDamage());
            StartCoroutine(Invincibility());

            if (health <= 0)
                Die();
            //Debug.Log("Health is zero or below!");

            // 넉백 코루틴 실행
            StartCoroutine(Knockback(attacker));
        }
    }

    // Knockback 코루틴 변경
    IEnumerator Knockback(Transform attacker)
    {
        Vector2 knockbackDirection = (transform.position - attacker.position).normalized;
        float knockbackEndTime = Time.time + knockbackDuration;

        while (Time.time < knockbackEndTime)
        {
            rb.position += knockbackDirection * knockbackStrength * Time.deltaTime;
            yield return null;
        }
    }


    IEnumerator Invincibility()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    IEnumerator FlashDamage()
    {
        Color originalColor = spriteRenderer.color;
        Color flashColor = new Color(0f, 0f, 0f, flashAlpha);
        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashSpeed);

            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashSpeed);
        }
    }

    void Die()
    {
        dieScript.PlayerDied();
        Debug.Log("Die");
    }
}