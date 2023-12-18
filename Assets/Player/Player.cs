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
    public HealthBar healthBar;// healthbar ����
    public float health = 100f; // health ���� ���
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
    public float knockbackStrength = 10f; // �˹��� ����
    [SerializeField]
    public float knockbackDuration = 0.5f; // �˹��� ���ӽð�

    [Header("Coffee Bullet")]
    public GameObject coffeePrefab;  // Coffee Prefab
    public Transform coffeeSpawnPoint; // Coffee�� �߻��� ��ġ
    public float coffeeSpeed = 300.0f; // Coffee�� �ӵ�

    [Header("Health Recovery")]
    public GameObject healthRecoveryEffectPrefab;  // ȸ�� ����Ʈ ������
    public float recoveryEffectDuration = 1.0f;  // ����Ʈ ���� �ð�

    [Header("Health Recovery Settings")]
    [SerializeField]
    public float healthRecoveryAmount = 20f;  // ȸ������ �����ϴ� ����

    private GameObject currentEffect;  // ���� Ȱ��ȭ�� ȸ�� ����Ʈ

    [Header("Coffee Bullet")]
    public float coffeeFireCooldown = 0.5f; // Ŀ�� ĵ �߻� ��Ÿ��
    private float lastFireTime = 0.0f; // ������ Ŀ�� ĵ �߻� �ð�

    public float bounceForce = 10f;
    public float destroyDelay = 2f; // ������Ʈ�� �ı��ϱ� ���� ��ٸ� �ð� (��)

    public float yOffset = 0f;
    public float xOffset = 0f;

    public GameObject dieUI; // �� ������ DIE �ؽ�Ʈ�� �̹����� ����ִ� GameObject�� �����մϴ�.
    public Die dieScript;

    public Mob targetMonster; // Ÿ�� ���� ����
    public Mob2 targetMonster2;
    public Mob3 targetMonster3;
    public float popHeadKillDistance = 5.0f; // �����ų Ȱ��ȭ �Ÿ�
    private bool isPopHeadKillActive = false;
    [Header("Pop Head Kill Settings")]
    [Tooltip("The sprite object for PopHeadKill")]
    public GameObject popHeadKillSpriteObject; 



    private void Start()
    {
        InitializeComponents();
        currentEffect = Instantiate(healthRecoveryEffectPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity, transform);
        currentEffect.SetActive(false); // �ʱ⿡�� ��Ȱ��ȭ

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
            // ������Ʈ�� ������ٵ� �����ɴϴ�.
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // ������Ʈ�� ��ġ�� �÷��̾��� ��ġ�� ������� ƨ�ܳ��� ������ ����մϴ�.
                Vector2 bounceDirection = (collision.transform.position - transform.position).normalized;

                // ������ٵ� ���� �����Ͽ� ������Ʈ�� ƨ�� ������ �մϴ�.
                rb.AddForce(bounceDirection * bounceForce, ForceMode2D.Impulse);

                // ���� �ð� �Ŀ� ������Ʈ�� �ı��մϴ�.
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
        health = Mathf.Min(health, 10000f); // ü���� 100�� �ʰ��� �� ����
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
            // �����ų Ȱ��ȭ�� ���õ� �߰� ����
            isPopHeadKillActive = true;
        }
        else
        {
            // �����ų ��Ȱ��ȭ�� ���õ� �߰� ����
            isPopHeadKillActive = false;
        }
    }

    public void SetTargetMonster2(Mob2 newTarget)
    {
        targetMonster2 = newTarget;
        if (targetMonster2 != null)
        {
            // �����ų Ȱ��ȭ�� ���õ� �߰� ����
            isPopHeadKillActive = true;
        }
        else
        {
            // �����ų ��Ȱ��ȭ�� ���õ� �߰� ����
            isPopHeadKillActive = false;
        }
    }

    public void SetTargetMonster3(Mob3 newTarget)
    {
        targetMonster3 = newTarget;
        if (targetMonster3 != null)
        {
            // �����ų Ȱ��ȭ�� ���õ� �߰� ����
            isPopHeadKillActive = true;
        }
        else
        {
            // �����ų ��Ȱ��ȭ�� ���õ� �߰� ����
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
        // ī�޶� Ȯ��
        MoveCamera cameraScript = Camera.main.GetComponent<MoveCamera>();
        if (cameraScript != null)
        {
            cameraScript.StartCloseUp(targetMonster.transform); // ���Ϳ��� ī�޶� Ȯ��
        }

        // �����ų �̹��� ǥ��
        PopHeadKillImage();

        yield return new WaitForSeconds(1); // 1�� ����

        // ���� ��� ó��
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

        // ��������Ʈ ��ġ�� �÷��̾� ��ġ ���� ����
        popHeadKillSpriteObject.transform.position = transform.position;

        popHeadKillSpriteObject.SetActive(true); // ��������Ʈ Ȱ��ȭ

        popHeadKillSpriteObject.SetActive(false); // ��������Ʈ ��Ȱ��ȭ
    }


void AttackMonster()
    {
        if (targetMonster != null)
        {
            // ���� ��� ó��
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
            // �÷��̾�� �������� ���� �ʰ� �˹鸸 �����մϴ�.
        }
        else if (collision.CompareTag("Claw_right") || collision.CompareTag("Claw_left"))
        {
            // Claw�� ����� �� (ü�� ���ҿ� �˹� �߻�)
            TakeDamage(20f, collision.transform);
        }

        else if (collision.CompareTag("Slash_right") || collision.CompareTag("Slash_left"))
        {
            // Slash�� ����� �� (ü�� ���ҿ� �˹� �߻�)
            TakeDamage(20f, collision.transform);
        }

        else if (collision.CompareTag("Coin"))
        {
            //Debug.Log("Coin collision detected.");
            Destroy(collision.gameObject);
            coin++;
            UIManager.instance.AddCoin(); // UIManager�� AddCoin �Լ��� ȣ���մϴ�.
        }
        else if (collision.CompareTag("RedMonster"))
        {
            // ���� ����� ����� �� (ü�� ���ҿ� �˹� �߻�)
            TakeDamage(20f, collision.transform);
        }
        else if (collision.CompareTag("flower"))
        {
            // ��ȭ ��ų�� ����� �� (ü�� ���ҿ� �˹� �߻�)
            TakeDamage(20f, collision.transform);
        }
        else if (collision.CompareTag("Flower"))
        {
            // ��ȭ ��ų�� ����� �� (ü�� ���ҿ� �˹� �߻�)
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
            // ���� ���� ����
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

        // yOffset ���� ����Ͽ� Ŀ���� �ʱ� Y ��ġ�� ����
        float newYOffset = 0.8f; // ���ϴ� ���̷� yOffset�� ����

        // �÷��̾��� ���� ��ġ�� �������� Ŀ�� ĵ�� �߻��� ��ġ�� ����մϴ�.
        Vector3 playerPosition = transform.position;

        // Ŀ�� ĵ�� �ʱ� ��ġ�� �����մϴ�. ���⼭ x ���� ������ �� �ֽ��ϴ�.
        float playerWidth = 0.5f; // �÷��̾��� �ʺ� (���� ũ��)�� �����ϼ���.
        float xOffset = playerWidth / 2.0f; // �÷��̾� �߽ɺη� x ��ǥ�� �����մϴ�.

        Vector3 initialPosition = new Vector3(playerPosition.x + xOffset, playerPosition.y + newYOffset, playerPosition.z);

        // �÷��̾��� ������ ���⿡ ���� Ŀ�� ĵ�� �߻� ������ �����մϴ�.
        Vector2 fireDirection = new Vector2(0, 0); // �ʱ�ȭ

        if (lastDirection == "Right")
        {
            fireDirection = new Vector2(1, 0); // ���������� �߻�
        }
        else if (lastDirection == "Left")
        {
            fireDirection = new Vector2(-1, 0); // �������� �߻�
        }

        GameObject coffee = Instantiate(coffeePrefab, initialPosition, Quaternion.identity);
        Rigidbody2D coffeeRb = coffee.GetComponent<Rigidbody2D>();

        coffeeRb.velocity = fireDirection * coffeeSpeed;
        coffeeRb.angularVelocity = 500f; // ȸ�� �ӵ� ����

        // Coffee Ŭ������ �߻� ���� ������ �����մϴ�.
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
            healthBar.TakeDamage((int)damage); // HealthBar�� TakeDamage �޼��� ȣ�� only when player is not invincible

            StartCoroutine(FlashDamage());
            StartCoroutine(Invincibility());

            if (health <= 0)
                Die();
            //Debug.Log("Health is zero or below!");

            // �˹� �ڷ�ƾ ����
            StartCoroutine(Knockback(attacker));
        }
    }

    // Knockback �ڷ�ƾ ����
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