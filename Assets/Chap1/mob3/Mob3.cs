using System.Collections;
using UnityEngine;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;

public class Mob3 : MonoBehaviour
{
    [Header("Mob Parameters")]
    public float moveSpeed;
    public float distanceToAttack = 3f;
    public Transform player;
    public float health = 9f;
    public float attackDamage = 0f;
    public float attackCooldown = 1.5f;

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

    private float attackAnimationLength = 0.8f;
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


    public static bool isPopHeadKillActive = false;
    public static bool isGlobalStop = false; // Ŭ���� �������� ����

    public GameObject spacePrefab; // Space ������ ����
    public GameObject spacePressPrefab; // Space_Press ������ ����
    private GameObject currentSpaceObject; // ���� Ȱ��ȭ�� Space ��ü
    public Vector3 prefabOffset; // �ν����Ϳ��� ���� ������ ������ ��ġ ������


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

        if (rb != null && attacker.CompareTag("Player")) // 'Player' �±׸� ���� �����ڿ��Ը� �˹� ����
        {
            StartCoroutine(Knockback(attacker));
        }
    }

    IEnumerator Knockback(Transform attacker)
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        Vector2 knockbackDirection = (transform.position - attacker.position).normalized;
        rb.velocity = knockbackDirection * knockbackStrength;

        yield return new WaitForSeconds(knockbackDuration);

        rb.velocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void OnPopHeadKill()
    {
        Debug.Log("OnPopHeadKill called in Mob");
        Die();
    }

    public void EnterPopHeadKillState()
    {
        isPopHeadKillActive = true;
        // �߰����� ���� (��: �ִϸ��̼� ���)...
    }

    IEnumerator PopHeadKill()
    {
        isPopHeadKillActive = true;
        isGlobalStop = true;
        FreezeAllMobs(true);

        // �����ų ���� ���� �� �÷��̾��� targetMonster ����
        var playerComponent = FindObjectOfType<Player>();
        if (playerComponent != null)
        {
            playerComponent.SetTargetMonster3(this);
        }


        // �����ų ���� ���� �� ī�޶� Ȯ��
        MoveCamera cameraScript = Camera.main.GetComponent<MoveCamera>();
        if (cameraScript != null)
        {
            cameraScript.StartCloseUp(transform); // ī�޶� �� ���Ϳ��� Ȯ��
        }

        // �����ų ���� ���� �� space ��ü Ȱ��ȭ
        currentSpaceObject.SetActive(true);

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        Color originalColor = renderer.color;
        renderer.color = Color.red;

        bool toggle = false;
        while (isPopHeadKillActive)
        {
            Destroy(currentSpaceObject);
            GameObject prefabToInstantiate = toggle ? spacePrefab : spacePressPrefab;
            currentSpaceObject = Instantiate(prefabToInstantiate, transform.position + prefabOffset, Quaternion.identity);
            currentSpaceObject.transform.SetParent(transform);

            toggle = !toggle;
            yield return new WaitForSeconds(0.5f);
        }

        // �÷��̾��� targetMonster ����
        if (playerComponent != null)
        {
            playerComponent.SetTargetMonster(null);
        }

        // �����ų ���� ���� �� ī�޶� �������
        if (cameraScript != null)
        {
            cameraScript.EndCloseUp(player.transform); // player�� Transform ������Ʈ�� ����
        }
        // �����ų ���� ���� �� space ��ü ��Ȱ��ȭ
        currentSpaceObject.SetActive(false);

        // ���� �������� ����
        renderer.color = originalColor;

        FreezeAllMobs(false);
        isGlobalStop = false;

        yield break;
    }


    void FreezeAllMobs(bool shouldFreeze)
    {
        // ��� ���Ϳ� ���� ������ ��� ����� ���� �� �κ��� ä��ϴ�.
        // ���� ���, ��� ���Ͱ� 'Mob' �±׸� ������ �ִٸ� �Ʒ��� ���� �� �� �ֽ��ϴ�:
        foreach (var mob in GameObject.FindGameObjectsWithTag("Monster"))
        {
            var rb = mob.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.constraints = shouldFreeze ? RigidbodyConstraints2D.FreezeAll : RigidbodyConstraints2D.FreezeRotation;
            }
        }
    }

    public void Die()
    {
        if (isPopHeadKillActive)
        {
            Debug.Log("Die method called in Mob");
            // �����ų ������ ���� ��� ó��
            isPopHeadKillActive = false;
            Mob3.isGlobalStop = false;
            DropCoin(2); // �����ų ������ �� ���� 2�� ���
            StartCoroutine(DeathAnimation());
        }
        else
        {
            // �Ϲ� ������ ���� ��� ó��
            if (Random.Range(0, 10) < 10)
            {
                isGlobalStop = true; // �ٸ� ���͵��� ���ߵ��� �մϴ�.
                StartCoroutine(PopHeadKill());
            }
            else
            {
                // StartCoroutine(DeathAnimation()); // ���� �ִϸ��̼� ���� �ڵ� �ּ� ó��
                Destroy(gameObject);
            }
        }
    }

    public void AttackByPlayer()
    {
        if (isPopHeadKillActive)
        {
            isPopHeadKillActive = false;
            StopCoroutine("PopHeadKill"); // PopHeadKill �ڷ�ƾ�� �����մϴ�.
            Mob.isGlobalStop = false; // �ٸ� ���͵��� ������ �� �ֵ��� �մϴ�.

            // ī�޶� ���� ���·� �ǵ����ϴ�.
            MoveCamera cameraScript = Camera.main.GetComponent<MoveCamera>();
            if (cameraScript != null)
            {
                cameraScript.EndCloseUp(player.transform);
            }

            // space ��ü�� ��Ȱ��ȭ�մϴ�.
            if (currentSpaceObject != null)
            {
                currentSpaceObject.SetActive(false);
            }

            StartCoroutine(DeathAnimation());
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // �÷��̾�� �浹������, ���Ͱ� ���� ���°� �ƴϸ� �ƹ��� ������ ���� �ʽ��ϴ�.
            if (!animator.GetBool("IsAttackingSchool"))
            {
                // �ƹ� ���۵� ���� �ʽ��ϴ�.
                return;
            }

            // ���� ���Ͱ� ���� ���¶��, ���⿡ �ٸ� ���� ������ �߰��մϴ�.
        }
    }

    IEnumerator DeathAnimation()
    {
        //Debug.Log("DeathAnimation started");
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
            //Debug.LogError("Player object not found!");
            return;
        }
        player = playerObject.transform;
        //Debug.Log("Player object found.");
    }

    void Awake()
    {
        moveSpeed = Random.Range(1.0f, 2.5f); // �̵� �ӵ��� �������� �����մϴ�.

        rb = GetComponent<Rigidbody2D>();
        if (rb == null) // ���� Rigidbody2D ������Ʈ�� ������ �߰��մϴ�.
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        // Rigidbody�� ������ �����մϴ�.
        rb.bodyType = RigidbodyType2D.Dynamic; // ���Ͱ� ���� ���� ���� �������� �ʵ��� �մϴ�.
        rb.gravityScale = 0; // �߷��� ������ ���� �ʵ��� �մϴ�.
        rb.mass = 10f; // ������ �浹�� ���� �и��� �����ϱ� ���� ������ ���Դϴ�.

        // ��ġ �� ȸ�� ������ �����մϴ�.
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        animator = GetComponent<Animator>();
        if (animator == null) // Animator ������Ʈ�� ������ �α׸� ����ϰ� �޼��带 �����մϴ�.
        {
            Debug.LogError("Animator component not found on the mob object.");
            return;
        }

        // ���Ͱ� �θ� ��ü�� �ڽ��� ���, �θ���� ������ �����մϴ�.
        if (transform.parent != null)
        {
            transform.parent = null;
        }

        // ���Ͱ� �� ��ȯ �ÿ� �ı����� �ʵ��� �����մϴ�.
        DontDestroyOnLoad(gameObject);
    }


    void Update()
    {
        // ��ü ���� ���� �Ǵ� PopHeadKill ������ ���, �Ʒ��� ��� �ൿ�� �����մϴ�.
        if (Mob.isGlobalStop) // Mob Ŭ������ ���ǵ� isGlobalStop�� ����
        {
            // �̰����� �߰����� ���� ������ �ʿ��� ��� ����
            return;
        }

        // �÷��̾ ������ �ƹ��͵� ���� ����
        if (player == null) return;

        LookAtPlayer();
        MoveControl();
        AttackControl();
        UpdateAttackAnimation();
    }

    void LookAtPlayer()
    {
        if (player == null) return;

        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;

        if (transform.position.x > player.position.x && isFacingRight)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFacingRight = false;
        }
        else if (transform.position.x < player.position.x && !isFacingRight)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFacingRight = true;
        }
    }

    void MoveControl()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        float distanceY = moveSpeed * Time.deltaTime;
        float distanceX = Mathf.Abs(player.position.x - transform.position.x);

        // �÷��̾��� ��ġ�� ���� �׻� �̵��ϵ��� ����
        Vector2 newPos = rb.position + (Vector2)direction * distanceY;
        rb.MovePosition(newPos);
        animator.SetBool("IsWalkingSchool", true);

        // ���Ͱ� �÷��̾ �ٶ󺸵��� �ϴ� ����
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

    void AttackControl()
    {
        if (Vector3.Distance(transform.position, player.position) < distanceToAttack && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            attackAnimationEndTime = Time.time + attackAnimationLength;

            animator.SetBool("IsAttackingSchool", true);
            Invoke("SpawnSlashithDelay", 0.7f);  // �������� ��Ÿ���� �������� �ִ� �Լ�
        }
    }

    void SpawnSlashWithDelay()
    {
        // isFacingRight ������ ���� ����� �������� �����մϴ�.
        GameObject prefabToSpawn = isFacingRight ? mobSlashRightPrefab : mobSlashLeftPrefab;
        Vector3 spawnPosition = transform.position;

        // ���Ͱ� �ٶ󺸴� ���⿡ ���� ���� ��ġ�� �����մϴ�.
        spawnPosition.x += isFacingRight ? 1.5f : -1.5f;

        GameObject slashInstance = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        SlashBehaviour slashBehaviour = slashInstance.GetComponent<SlashBehaviour>();
        if (slashBehaviour == null)
        {
            slashBehaviour = slashInstance.AddComponent<SlashBehaviour>();
        }
        slashBehaviour.damage = attackDamage;

        // ���Ͱ� Ŭ�ο츦 ��ȯ�ϴ� ���� ��ġ�� ������ŵ�ϴ�.
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;

        StartCoroutine(EnableSlashColliderAfterDelay(slashBehaviour, 0.3f));

        // Ŭ�ΰ� ������ �ڽ� ��ü�� ���� �ʵ��� �մϴ� (���Ͱ� ������ �� ���� �������� �ʵ���)
        slashInstance.transform.parent = null;

        // ��ġ ������ �����ϴ� ������ �߰��մϴ�.
        StartCoroutine(UnfreezeAfterDelay(0.3f));

        Destroy(slashInstance, 0.3f); // ª�� �ð� �Ŀ� claw�� �ı��մϴ�.
    }

    IEnumerator UnfreezeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // ���Ͱ� �ٽ� ������ �� �ֵ��� ��ġ ������ �����մϴ�.
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    IEnumerator EnableSlashColliderAfterDelay(SlashBehaviour Slash, float delay)
    {
        yield return new WaitForSeconds(delay);

    }



    void UpdateAttackAnimation()
    {
        if (Time.time >= attackAnimationEndTime && animator.GetBool("IsAttackingSchool"))
        {
            animator.SetBool("IsAttackingSchool", false);
        }
    }

    public class SlashBehaviour : MonoBehaviour
    {
        public float damage = 20f;
        private Collider2D slashCollider;
        private bool isColliderEnabled = false;

        private void Awake()
        {
            slashCollider = GetComponent<Collider2D>();
            // DisableCollider();
        }

    }
}