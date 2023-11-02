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
    public float health = 5f; // ������ ü��

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
    public float deathAnimationDuration = 2f;  // ������µ� �ɸ��� �ð�. �ν����Ϳ��� ���� �����ϰ� �մϴ�.




    // ���Ͱ� �������� ������ �� �Լ��� ȣ���մϴ�.
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

        // �˹� ������ ������ ���� ���ο� ������� ����
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

            // ������ ���� ����ϴ�.
            Color currentColor = GetComponent<SpriteRenderer>().color;
            currentColor.a = Mathf.Lerp(originalColor.a, 0, ratio);
            GetComponent<SpriteRenderer>().color = currentColor;

            // �Ʒ��� �����Դϴ�.
            Vector3 newPosition = originalPosition;
            newPosition.y -= ratio;  // �Ʒ��� �����̴� ������ �����Ϸ��� �� ���� �����ϼ���.
            transform.position = newPosition;

            // ������ ����Ʈ���ϴ�. (���� �� ����)
            if (ratio > 0 && coinPrefab != null)
            {
                DropCoin();
                coinPrefab = null;  // ������ �� ���� ����Ʈ������ �մϴ�.
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

        // ���� ���������� y ��ǥ�� �����մϴ�.
        Vector3 dropPosition = transform.position + new Vector3(0, -0.5f, 0);

        GameObject coin = Instantiate(coinPrefab, dropPosition, Quaternion.identity);
        coin.transform.parent = null;

        // ������ ���� ���� ������ y ��ǥ�� ���Դϴ�.
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

        // �� �κ��� �߰��մϴ�. RigidbodyType�� Dynamic���� ����.
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0; // �߷��� ����

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
        // if (collision.gameObject.tag == "obj")  // "obj" �±׸� ���� ������Ʈ�� �浹 ��
        //{
            // �������� 0���� �ϰ�, �˹鸸 ����
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
            // playerComponent.TakeDamage(attackDamage, transform);  // �� �κ��� �ּ� ó�� �Ǵ� �����մϴ�.
            Invoke("SpawnClawWithDelay", 0.3f);  // Ŭ�ΰ� ��Ÿ���� �������� �ִ� �Լ�
        }
    }


    void SpawnClawWithDelay()
    {
        // isFacingRight ������ ���� ����� �������� �����մϴ�.
        GameObject prefabToSpawn = isFacingRight ? mobClawRightPrefab : mobClawLeftPrefab;
        Vector3 spawnPosition = transform.position;

        // �� �κ��� �ܼ��� isFacingRight�� ���� ��ġ�� �����մϴ�.
        if (isFacingRight)
        {
            spawnPosition.x += 1.5f;
        }
        else
        {
            spawnPosition.x -= 1.5f;
        }

        // �������� �����մϴ�.
        GameObject clawInstance = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);


        ClawBehaviour clawBehaviour = clawInstance.AddComponent<ClawBehaviour>();
        clawBehaviour.damage = 20f;  // Ŭ���� ���ݷ��� 20���� ����

        // Ŭ�ΰ� ������ �ڽ� ��ü�� ���� �ʵ��� �մϴ�.
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

