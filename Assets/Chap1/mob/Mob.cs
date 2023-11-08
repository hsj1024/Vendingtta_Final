using System.Collections;
using System.Threading;
using UnityEngine;

public class Mob : MonoBehaviour
{
    [Header("Mob Parameters")]
    [Tooltip("Speed at which the mob moves.")]
    public float moveSpeed;

    [Tooltip("Distance from the player at which the mob will attack.")]
    public float distanceToAttack = 3f;

    [Tooltip("Reference to the player's transform.")]
    public Transform player;

    [Tooltip("Health of the mob")]
    public float health = 5f;

    [Tooltip("Damage dealt by the mob's attack.")]
    public float attackDamage = 0f;

    [Tooltip("Cooldown time between attacks.")]
    public float attackCooldown = 1f;

    [Header("Knockback Settings")]
    [SerializeField]
    public float knockbackStrength = 100f;
    [SerializeField]
    public float knockbackDuration = 1f;

    private float nextAttackTime;
    private Animator animator;
    private bool isFacingRight;
    private Rigidbody2D rb;

    [Header("Claw Prefabs")]
    public GameObject mobClawRightPrefab;
    public GameObject mobClawLeftPrefab;

    [Header("Attack Settings")]
    [Tooltip("Delay before the claw ")]
    [SerializeField]
    private float clawSpawnDelay = 0.3f;

    public event System.Action OnDeath;

    private float attackAnimationLength = 0.5f;
    private float attackAnimationEndTime;

    [Header("Coin Drop Settings")]
    [Tooltip("Prefab for the dropped coin.")]
    public GameObject coinPrefab;

    [Tooltip("The animation clip for the coin drop.")]
    public AnimationClip coinDropAnimation;

    [Tooltip("The speed of the coin drop animation.")]
    [Range(0.1f, 3f)]
    public float coinAnimationSpeed = 1f;

    [Header("Death Animation Settings")]
    [Tooltip("Duration of the death animation.")]
    public float deathAnimationDuration = 2f;

    public static bool isPopHeadKillActive = false;
    public static bool isGlobalStop = false; // Ŭ���� �������� ����



    // ���Ͱ� �������� ������ �� �Լ��� ȣ���մϴ�.
    public void TakeDamage(float damage, Transform attacker, bool applyDamage = true)
    {
        if (applyDamage)
        {
            health -= damage;
            if (health <= 0)
            {
                Die();
                return;
            }
        }

        // �˹� ������ ������ ���� ���ο� ������� ����
        // �߰�: �����ڰ� �÷��̾��� ��쿡�� �˹��� �����մϴ�.
        if (rb != null && attacker.CompareTag("Coffee"))
        {
            StartCoroutine(Knockback(attacker));
        }
    }

    IEnumerator Knockback(Transform attacker)
    {
        // ȸ���� �����ϰ� �˹� ����
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        Vector2 knockbackDirection = (transform.position - attacker.position).normalized;
        rb.velocity = knockbackDirection * knockbackStrength;

        // �˹� ���� ���Ͱ� �̵��� �� �ֵ��� X�� Y ��ġ ������ �����մϴ�.
        yield return new WaitForSeconds(knockbackDuration);

        // �˹��� ������ ��� �������� �����ϰ� ������� ��ġ ������ �����մϴ�.
        rb.velocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
    }




    public void Die()
    {
        if (Random.Range(0, 10) < 0)
        {
            
            isGlobalStop = true; // �ٸ� ���͵��� ���ߵ��� �մϴ�.
            StartCoroutine(PopHeadKill());
        }
        else
        {
            StartCoroutine(DeathAnimation());
        }
    }

    IEnumerator PopHeadKill()
    {
        isPopHeadKillActive = true;
        // �ٸ� ���͵��� ���ߵ��� �մϴ�.
        
        Mob.isGlobalStop = true; // Mob1 Ŭ�������� �����ϰ� ����

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        // �÷��̾ �����ϱ� ������ �ݺ�
        while (isPopHeadKillActive)
        {
            renderer.color = Color.red;
            yield return new WaitForSeconds(1f);
            renderer.color = Color.white;
            yield return new WaitForSeconds(1f);
        }
    }


    public void AttackByPlayer()
    {
        // PopHeadKill ���¿��� �÷��̾�� ���ݹ����� ��� ��� ó��
        if (isPopHeadKillActive)
        {
            isPopHeadKillActive = false; // PopHeadKill ���� ����
            StopCoroutine("PopHeadKill"); // ������ �ڷ�ƾ ����
                                          // �ٸ� ���͵��� �ٽ� ������ �� �ֵ��� �մϴ�.
           
            Mob.isGlobalStop = false; // Mob1 Ŭ�������� �����ϰ� ����
            StartCoroutine(DeathAnimation());
        }
    }




    IEnumerator DeathAnimation()
    {
        Debug.Log("DeathAnimation started");
        float elapsedTime = 0f;
        Vector3 originalPosition = transform.position;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color originalColor = spriteRenderer.color;

        while (elapsedTime < deathAnimationDuration)
        {
            float ratio = elapsedTime / deathAnimationDuration;

            // ������ ���� ����ϴ�.
            Color currentColor = spriteRenderer.color;
            currentColor.a = Mathf.Lerp(originalColor.a, 0, ratio);
            spriteRenderer.color = currentColor;

            // �Ʒ��� �����Դϴ�.
            Vector3 newPosition = originalPosition;
            newPosition.y -= ratio;  // �Ʒ��� �����̴� ������ �����Ϸ��� �� ���� �����ϼ���.
            transform.position = newPosition;

            // ������ ����Ʈ���ϴ�. (���� �� ����)
            if (ratio > 0 && coinPrefab != null)
            {
                if (isPopHeadKillActive)
                {
                    DropCoin(2);  // �����ų ������ �� ���� �� �� ���
                }
                else
                {
                    DropCoin();  // �Ϲ� ������ �� ���� �� �� ���
                }
                coinPrefab = null;  // ������ �� ���� ����Ʈ������ �մϴ�.
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        OnDeath?.Invoke();
        Destroy(gameObject);
    }

    void DropCoin(int amount = 1)
    {
        if (coinPrefab == null) return;

        for (int i = 0; i < amount; i++)
        {
            Vector3 dropPosition = transform.position + new Vector3(0, -0.5f, 0);
            GameObject coin = Instantiate(coinPrefab, dropPosition, Quaternion.identity);
            coin.transform.parent = null;
            StartCoroutine(DropCoinToGround(coin));
        }
    }

    IEnumerator DropCoinToGround(GameObject coin)
    {
        while (coin.transform.position.y > 0)
        {
            coin.transform.position -= new Vector3(0, 0.05f, 0);
            yield return new WaitForSeconds(0.01f);
        }
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            return;
        }
        player = playerObject.transform;
    }

    void Awake()
    {
        moveSpeed = Random.Range(1.0f, 2.5f);
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0;
        rb.mass = 10f; // Mass ���� ������ �ٸ� ������Ʈ�� ���� �и��� �����մϴ�.
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            return;
        }
        if (transform.parent != null)
        {
            transform.parent = null;
        }
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // �÷��̾ ������ �ƹ��͵� ���� ����
        if (player == null) return;

        // �����ų ���°� �ƴϸ鼭 ��ü ���� ������ �� �����Ӱ� ������ ����
        if (!isPopHeadKillActive && Mob.isGlobalStop) return;

        MoveControl();
        AttackControl();
        UpdateAttackAnimation();
    }

    void MoveControl()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        float distanceY = moveSpeed * Time.deltaTime;
        float distanceX = Mathf.Abs(player.position.x - transform.position.x);

        if (distanceX > 1.5f)
        {
            if (player.position.x > transform.position.x && !isFacingRight)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
                isFacingRight = true;
            }
            else if (player.position.x < transform.position.x && isFacingRight)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                isFacingRight = false;
            }
        }
        if (Vector3.Distance(transform.position, player.position) >= distanceToAttack)
        {
            Vector2 newPos = rb.position + (Vector2)direction * distanceY;
            rb.MovePosition(newPos);
            animator.SetBool("IsWalkingSchool", true);
        }
        else
        {
            animator.SetBool("IsWalkingSchool", false);
        }
    }

    void AttackControl()
    {
        if (Vector3.Distance(transform.position, player.position) < distanceToAttack && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            attackAnimationEndTime = Time.time + attackAnimationLength;
            animator.SetBool("IsAttackingSchool", true);
            // Invoke the method with the delay specified in the Inspector
            Invoke("SpawnClawWithDelay", clawSpawnDelay);
        }
    }

    void SpawnClawWithDelay()
    {
        // isFacingRight ������ ���� ����� �������� �����մϴ�.
        GameObject prefabToSpawn = isFacingRight ? mobClawRightPrefab : mobClawLeftPrefab;
        Vector3 spawnPosition = transform.position;

        // ���Ͱ� �ٶ󺸴� ���⿡ ���� ���� ��ġ�� �����մϴ�.
        spawnPosition.x += isFacingRight ? 1.5f : -1.5f;

        GameObject clawInstance = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        ClawBehaviour clawBehaviour = clawInstance.AddComponent<ClawBehaviour>();
        clawBehaviour.damage = attackDamage;
        StartCoroutine(EnableClawColliderAfterDelay(clawBehaviour, 0.3f));
        // claw�� ������ �ڽ� ��ü�� ���� �ʵ��� �մϴ� (���Ͱ� ������ �� ���� �������� �ʵ���)
        clawInstance.transform.parent = null;

        Destroy(clawInstance, 0.3f); // ª�� �ð� �Ŀ� claw�� �ı��մϴ�.
    }

    IEnumerator EnableClawColliderAfterDelay(ClawBehaviour claw, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (claw != null) // Claw ��ü�� ������ ��ȿ���� Ȯ��
        {
            claw.EnableCollider();
        }
    }


    void UpdateAttackAnimation()
    {
        if (Time.time >= attackAnimationEndTime)
        {
            animator.SetBool("IsAttackingSchool", false);
        }
    }
}
public class ClawBehaviour : MonoBehaviour
{
    public float damage = 20f;
    private Collider2D clawCollider;
    private bool isColliderEnabled = false;

    private void Awake()
    {
        clawCollider = GetComponent<Collider2D>();
        //DisableCollider();
    }

    public void EnableCollider()
    {
        if (this == null || gameObject == null) return; // �� ��ü�� ������ ��� �Լ��� �ٷ� �����մϴ�.

        isColliderEnabled = true;
        clawCollider.enabled = true;
    }


    

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isColliderEnabled) return;

        Player playerComponent = collision.GetComponent<Player>();
        if (playerComponent != null)
        {
            playerComponent.TakeDamage(damage, transform);
        }
    }
}
