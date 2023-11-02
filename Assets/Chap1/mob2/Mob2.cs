using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

public class Mob2 : MonoBehaviour
{
    [Header("Mob Parameters")]
    public float moveSpeed;
    public float distanceToAttack = 3f;
    public Transform player;
    public float health = 5f;
    public float attackDamage = 0f;
    public float attackCooldown = 1f;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackStrength = 100f;
    [SerializeField] private float knockbackDuration = 1f;

    private float nextAttackTime;
    private Animator animator;
    private bool isFacingRight;
    private Rigidbody2D rb;

    [Header("Slash Prefabs")]
    public GameObject mobSlashLeftPrefab;
    public GameObject mobSlashRightPrefab;


    public event System.Action OnDeath;

    private float attackAnimationLength = 0.5f;
    private float attackAnimationEndTime;

    [Header("Death Animation Settings")]
    public float deathAnimationDuration = 2f;

    [Header("Coin Drop Settings")]
    [Tooltip("Prefab for the dropped coin.")]
    public GameObject coinPrefab;

    [Tooltip("The animation clip for the coin drop.")]
    public AnimationClip coinDropAnimation;

    [Tooltip("The speed of the coin drop animation.")]
    [Range(0.1f, 3f)]
    public float coinAnimationSpeed = 1f;


    private bool isPopHeadKillState = false;


    public void TakeDamage(float damage, Transform attacker, bool applyDamage = true)
    {
        if (applyDamage)
        {
            Debug.Log("TakeDamage called with damage: " + damage);

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
    IEnumerator PopHeadKill()
    {
        Time.timeScale = 0;
        yield return new WaitForSeconds(3);

        if (isPopHeadKillState)
        {
            StartCoroutine(DeathAnimation());
        }
    }

    public void Die()
    {
        if (Random.Range(0, 10) < 10)
        {
            // 팝헤드킬 상태로 설정
            isPopHeadKillState = true;
            // 팝헤드킬 애니메이션 실행
            StartCoroutine(PopHeadKill());
        }
        else
        {
            StartCoroutine(DeathAnimation());
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
            //Debug.LogError("Player object not found!");
            return;
        }
        player = playerObject.transform;
        //Debug.Log("Player object found.");
    }

    void Awake()
    {
        moveSpeed = Random.Range(1.0f, 2.5f);

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        // 이 부분을 추가합니다. RigidbodyType을 Dynamic으로 설정.
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0; // 중력을 무시

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            //Debug.LogError("Animator not found on " + gameObject.name);
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

        //Debug.Log("MoveControl method called.");

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

        if (player.position.x > transform.position.x && !isFacingRight)
        {
            //Debug.Log("Setting isFacingRight to true");
            transform.rotation = Quaternion.Euler(0, 180, 0);
            isFacingRight = true;
        }
        else if (player.position.x < transform.position.x && isFacingRight)
        {
            //Debug.Log("Setting isFacingRight to false");
            transform.rotation = Quaternion.Euler(0, 0, 0);
            isFacingRight = false;
        }
    }

    void AttackControl()
    {
        if (Vector3.Distance(transform.position, player.position) < distanceToAttack && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            attackAnimationEndTime = Time.time + attackAnimationLength;

            animator.SetBool("IsAttackingSchool", true);
            Invoke("SpawnSlashithDelay", 0.7f);  // 슬래쉬가 나타나고 데미지를 주는 함수
        }
    }

    void SpawnSlashithDelay()
    {
        // isFacingRight 변수에 따라 사용할 프리팹을 선택합니다.
        GameObject prefabToSpawn = isFacingRight ? mobSlashRightPrefab : mobSlashLeftPrefab;
        Vector3 spawnPosition = transform.position;

        // 이 부분은 단순히 isFacingRight에 따라 위치를 결정합니다.
        if (isFacingRight)
        {
            spawnPosition.x += 1.5f;
        }
        else
        {
            spawnPosition.x -= 1.5f;
        }

        // 프리팹을 생성합니다.
        GameObject slashInstance = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);


        SlashBehaviour slashBehaviour = slashInstance.AddComponent<SlashBehaviour>();
        slashBehaviour.damage = 20f;  // 클로의 공격력을 20으로 설정

        // 몬스터의 자식 객체가 되지 않도록 합니다.
        slashInstance.transform.parent = null;

        Destroy(slashInstance, 0.3f);
    }

    void UpdateAttackAnimation()
    {
        if (Time.time >= attackAnimationEndTime && animator.GetBool("IsAttackingSchool"))
        {
            animator.SetBool("IsAttackingSchool", false);
        }
    }

    private class SlashBehaviour : MonoBehaviour
    {
        public float damage = 20f;

        void OnTriggerEnter2D(Collider2D collision)
        {
            Player playerComponent = collision.GetComponent<Player>();
            if (playerComponent != null)
            {
                playerComponent.TakeDamage(damage, transform);
            }
        }
    }
}
