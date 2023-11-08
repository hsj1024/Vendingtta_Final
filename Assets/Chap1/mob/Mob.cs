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
    public static bool isGlobalStop = false; // 클래스 레벨에서 정의



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
        // 추가: 공격자가 플레이어일 경우에만 넉백을 적용합니다.
        if (rb != null && attacker.CompareTag("Coffee"))
        {
            StartCoroutine(Knockback(attacker));
        }
    }

    IEnumerator Knockback(Transform attacker)
    {
        // 회전을 고정하고 넉백 시작
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        Vector2 knockbackDirection = (transform.position - attacker.position).normalized;
        rb.velocity = knockbackDirection * knockbackStrength;

        // 넉백 동안 몬스터가 이동할 수 있도록 X와 Y 위치 고정을 해제합니다.
        yield return new WaitForSeconds(knockbackDuration);

        // 넉백이 끝나면 모든 움직임을 중지하고 원래대로 위치 고정을 적용합니다.
        rb.velocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
    }




    public void Die()
    {
        if (Random.Range(0, 10) < 0)
        {
            
            isGlobalStop = true; // 다른 몬스터들이 멈추도록 합니다.
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
        // 다른 몬스터들이 멈추도록 합니다.
        
        Mob.isGlobalStop = true; // Mob1 클래스에도 동일하게 적용

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        // 플레이어가 공격하기 전까지 반복
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
        // PopHeadKill 상태에서 플레이어에게 공격받으면 즉시 사망 처리
        if (isPopHeadKillActive)
        {
            isPopHeadKillActive = false; // PopHeadKill 상태 해제
            StopCoroutine("PopHeadKill"); // 깜빡임 코루틴 정지
                                          // 다른 몬스터들이 다시 움직일 수 있도록 합니다.
           
            Mob.isGlobalStop = false; // Mob1 클래스에도 동일하게 적용
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
                if (isPopHeadKillActive)
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
        rb.mass = 10f; // Mass 값을 높여서 다른 오브젝트에 의한 밀림을 방지합니다.
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
        // 플레이어가 없으면 아무것도 하지 않음
        if (player == null) return;

        // 팝헤드킬 상태가 아니면서 전체 정지 상태일 때 움직임과 공격을 멈춤
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
        // isFacingRight 변수에 따라 사용할 프리팹을 선택합니다.
        GameObject prefabToSpawn = isFacingRight ? mobClawRightPrefab : mobClawLeftPrefab;
        Vector3 spawnPosition = transform.position;

        // 몬스터가 바라보는 방향에 따라 생성 위치를 조정합니다.
        spawnPosition.x += isFacingRight ? 1.5f : -1.5f;

        GameObject clawInstance = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        ClawBehaviour clawBehaviour = clawInstance.AddComponent<ClawBehaviour>();
        clawBehaviour.damage = attackDamage;
        StartCoroutine(EnableClawColliderAfterDelay(clawBehaviour, 0.3f));
        // claw가 몬스터의 자식 객체가 되지 않도록 합니다 (몬스터가 움직일 때 같이 움직이지 않도록)
        clawInstance.transform.parent = null;

        Destroy(clawInstance, 0.3f); // 짧은 시간 후에 claw를 파괴합니다.
    }

    IEnumerator EnableClawColliderAfterDelay(ClawBehaviour claw, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (claw != null) // Claw 객체가 여전히 유효한지 확인
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
        if (this == null || gameObject == null) return; // 이 객체가 삭제된 경우 함수를 바로 종료합니다.

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
