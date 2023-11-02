using System.Collections;
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
    [Tooltip("Prefab for the claw when the mob is facing right.")]
    public GameObject mobClawRightPrefab;

    [Tooltip("Prefab for the claw when the mob is facing left.")]
    public GameObject mobClawLeftPrefab;

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

    private bool isPopHeadKillState = false;

    // 몬스터가 데미지를 받으면 이 함수를 호출합니다.
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

        // 넉백 로직은 데미지 적용 여부와 상관없이 실행
        if (rb != null)
        {
            StartCoroutine(Knockback(attacker));
        }
    }

    IEnumerator Knockback(Transform attacker)
    {
        Vector2 knockbackDirection = (transform.position - attacker.position).normalized;
        rb.velocity = knockbackDirection * knockbackStrength;
        yield return new WaitForSeconds(knockbackDuration);
        rb.velocity = Vector2.zero;
    }



    public void Die()
    {
        if (Random.Range(0, 10) < 1)
        {
            // Die with pop head animation
            StartCoroutine(PopHeadKill());
        }
        else
        {
            StartCoroutine(DeathAnimation());
        }
    }

    IEnumerator PopHeadKill()
    {
        Time.timeScale = 0;
        float blinkDuration = 1.0f;

        while (true)
        {
            GetComponent<SpriteRenderer>().color = Color.red;
            yield return new WaitForSecondsRealtime(blinkDuration);
            GetComponent<SpriteRenderer>().color = Color.white;
            yield return new WaitForSecondsRealtime(blinkDuration);
        }
    }

    public void AttackByPlayer()
    {
        if (isPopHeadKillState)
        {
            // 팝헤드킬 상태에서 플레이어가 공격하면 즉시 사망
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

            // 투명도를 점점 낮춥니다.
            Color currentColor = spriteRenderer.color;
            currentColor.a = Mathf.Lerp(originalColor.a, 0, ratio);
            spriteRenderer.color = currentColor;

            // 아래로 움직입니다.
            Vector3 newPosition = originalPosition;
            newPosition.y -= ratio;  // 아래로 움직이는 정도를 조절하려면 이 값을 조절하세요.
            transform.position = newPosition;

            // 코인을 떨어트립니다. (최초 한 번만)
            if (ratio > 0 && coinPrefab != null)
            {
                if (isPopHeadKillState)
                {
                    DropCoin(2);  // 팝헤드킬 상태일 때 코인 두 개 드랍
                }
                else
                {
                    DropCoin();  // 일반 상태일 때 코인 한 개 드랍
                }
                coinPrefab = null;  // 코인을 한 번만 떨어트리도록 합니다.
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
        if (player == null) return;
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
            Invoke("SpawnClawWithDelay", 0.3f);
        }
    }

    void SpawnClawWithDelay()
    {
        if (player == null) return;

        GameObject clawPrefab = isFacingRight ? mobClawRightPrefab : mobClawLeftPrefab;
        Instantiate(clawPrefab, transform.position, Quaternion.identity, transform);
    }

    void UpdateAttackAnimation()
    {
        if (Time.time >= attackAnimationEndTime)
        {
            animator.SetBool("IsAttackingSchool", false);
        }
    }
}
