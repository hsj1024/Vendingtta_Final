using System.Collections;
using UnityEngine;
//using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;

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
    public float health = 5f; // 몬스터의 체력

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
    public float deathAnimationDuration = 2f;  // 사라지는데 걸리는 시간. 인스펙터에서 조절 가능하게 합니다.




    // 몬스터가 데미지를 받으면 이 함수를 호출합니다.
    public void TakeDamage(float damage, Transform attacker, bool applyDamage = true)
    {
        if (applyDamage)
        {
            //Debug.Log("TakeDamage called");

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
        StartCoroutine(DeathAnimation());
    }

    IEnumerator DeathAnimation()
    {
        float elapsedTime = 0f;
        Vector3 originalPosition = transform.position;
        Color originalColor = GetComponent<SpriteRenderer>().color;

        while (elapsedTime < deathAnimationDuration)
        {
            float ratio = elapsedTime / deathAnimationDuration;

            // 투명도를 점점 낮춥니다.
            Color currentColor = GetComponent<SpriteRenderer>().color;
            currentColor.a = Mathf.Lerp(originalColor.a, 0, ratio);
            GetComponent<SpriteRenderer>().color = currentColor;

            // 아래로 움직입니다.
            Vector3 newPosition = originalPosition;
            newPosition.y -= ratio;  // 아래로 움직이는 정도를 조절하려면 이 값을 조절하세요.
            transform.position = newPosition;

            // 코인을 떨어트립니다. (최초 한 번만)
            if (ratio > 0 && coinPrefab != null)
            {
                DropCoin();
                coinPrefab = null;  // 코인을 한 번만 떨어트리도록 합니다.
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        OnDeath?.Invoke();
        Destroy(gameObject);
    }


    void DropCoin()
    {
        if (coinPrefab == null) return;

        // 땅에 떨어지도록 y 좌표를 조절합니다.
        Vector3 dropPosition = transform.position + new Vector3(0, -0.5f, 0);

        GameObject coin = Instantiate(coinPrefab, dropPosition, Quaternion.identity);
        coin.transform.parent = null;

        // 동전이 땅에 닿을 때까지 y 좌표를 줄입니다.
        StartCoroutine(DropCoinToGround(coin));
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

    //void OnCollisionEnter2D(Collision2D collision)
    //{
        // if (collision.gameObject.tag == "obj")  // "obj" 태그를 가진 오브젝트와 충돌 시
        //{
            // 데미지를 0으로 하고, 넉백만 적용
           // TakeDamage(0f, collision.transform, false);
        //}
    //}




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
            // playerComponent.TakeDamage(attackDamage, transform);  // 이 부분을 주석 처리 또는 제거합니다.
            Invoke("SpawnClawWithDelay", 0.3f);  // 클로가 나타나고 데미지를 주는 함수
        }
    }


    void SpawnClawWithDelay()
    {
        // isFacingRight 변수에 따라 사용할 프리팹을 선택합니다.
        GameObject prefabToSpawn = isFacingRight ? mobClawRightPrefab : mobClawLeftPrefab;
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
        GameObject clawInstance = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);


        ClawBehaviour clawBehaviour = clawInstance.AddComponent<ClawBehaviour>();
        clawBehaviour.damage = 20f;  // 클로의 공격력을 20으로 설정

        // 클로가 몬스터의 자식 객체가 되지 않도록 합니다.
        clawInstance.transform.parent = null;

        Destroy(clawInstance, 0.3f);
    }


    void UpdateAttackAnimation()
    {
        if (Time.time >= attackAnimationEndTime && animator.GetBool("IsAttackingSchool"))
        {
            animator.SetBool("IsAttackingSchool", false);
        }
    }

    private class ClawBehaviour : MonoBehaviour
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

