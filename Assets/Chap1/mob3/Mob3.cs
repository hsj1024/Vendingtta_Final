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
    public static bool isGlobalStop = false; // 클래스 레벨에서 정의

    public GameObject spacePrefab; // Space 프리팹 참조
    public GameObject spacePressPrefab; // Space_Press 프리팹 참조
    private GameObject currentSpaceObject; // 현재 활성화된 Space 객체
    public Vector3 prefabOffset; // 인스펙터에서 조절 가능한 프리팹 위치 오프셋


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

        if (rb != null && attacker.CompareTag("Player")) // 'Player' 태그를 가진 공격자에게만 넉백 적용
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
        // 추가적인 로직 (예: 애니메이션 재생)...
    }

    IEnumerator PopHeadKill()
    {
        isPopHeadKillActive = true;
        isGlobalStop = true;
        FreezeAllMobs(true);

        // 팝헤드킬 상태 시작 시 플레이어의 targetMonster 설정
        var playerComponent = FindObjectOfType<Player>();
        if (playerComponent != null)
        {
            playerComponent.SetTargetMonster3(this);
        }


        // 팝헤드킬 상태 시작 시 카메라 확대
        MoveCamera cameraScript = Camera.main.GetComponent<MoveCamera>();
        if (cameraScript != null)
        {
            cameraScript.StartCloseUp(transform); // 카메라를 이 몬스터에게 확대
        }

        // 팝헤드킬 상태 시작 시 space 객체 활성화
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

        // 플레이어의 targetMonster 설정
        if (playerComponent != null)
        {
            playerComponent.SetTargetMonster(null);
        }

        // 팝헤드킬 상태 종료 시 카메라 원래대로
        if (cameraScript != null)
        {
            cameraScript.EndCloseUp(player.transform); // player의 Transform 컴포넌트를 전달
        }
        // 팝헤드킬 상태 종료 시 space 객체 비활성화
        currentSpaceObject.SetActive(false);

        // 원래 색상으로 복귀
        renderer.color = originalColor;

        FreezeAllMobs(false);
        isGlobalStop = false;

        yield break;
    }


    void FreezeAllMobs(bool shouldFreeze)
    {
        // 모든 몬스터에 대한 참조를 얻는 방법에 따라 이 부분을 채웁니다.
        // 예를 들어, 모든 몬스터가 'Mob' 태그를 가지고 있다면 아래와 같이 할 수 있습니다:
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
            // 팝헤드킬 상태일 때의 사망 처리
            isPopHeadKillActive = false;
            Mob3.isGlobalStop = false;
            DropCoin(2); // 팝헤드킬 상태일 때 코인 2개 드랍
            StartCoroutine(DeathAnimation());
        }
        else
        {
            // 일반 상태일 때의 사망 처리
            if (Random.Range(0, 10) < 10)
            {
                isGlobalStop = true; // 다른 몬스터들이 멈추도록 합니다.
                StartCoroutine(PopHeadKill());
            }
            else
            {
                // StartCoroutine(DeathAnimation()); // 이전 애니메이션 관련 코드 주석 처리
                Destroy(gameObject);
            }
        }
    }

    public void AttackByPlayer()
    {
        if (isPopHeadKillActive)
        {
            isPopHeadKillActive = false;
            StopCoroutine("PopHeadKill"); // PopHeadKill 코루틴을 정지합니다.
            Mob.isGlobalStop = false; // 다른 몬스터들이 움직일 수 있도록 합니다.

            // 카메라를 원래 상태로 되돌립니다.
            MoveCamera cameraScript = Camera.main.GetComponent<MoveCamera>();
            if (cameraScript != null)
            {
                cameraScript.EndCloseUp(player.transform);
            }

            // space 객체를 비활성화합니다.
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
            // 플레이어와 충돌했지만, 몬스터가 공격 상태가 아니면 아무런 반응을 하지 않습니다.
            if (!animator.GetBool("IsAttackingSchool"))
            {
                // 아무 동작도 하지 않습니다.
                return;
            }

            // 만약 몬스터가 공격 상태라면, 여기에 다른 반응 로직을 추가합니다.
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
            //Debug.LogError("Player object not found!");
            return;
        }
        player = playerObject.transform;
        //Debug.Log("Player object found.");
    }

    void Awake()
    {
        moveSpeed = Random.Range(1.0f, 2.5f); // 이동 속도를 무작위로 설정합니다.

        rb = GetComponent<Rigidbody2D>();
        if (rb == null) // 만약 Rigidbody2D 컴포넌트가 없으면 추가합니다.
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        // Rigidbody의 설정을 조정합니다.
        rb.bodyType = RigidbodyType2D.Dynamic; // 몬스터가 물리 힘에 의해 움직이지 않도록 합니다.
        rb.gravityScale = 0; // 중력의 영향을 받지 않도록 합니다.
        rb.mass = 10f; // 물리적 충돌에 의한 밀림을 방지하기 위해 질량을 높입니다.

        // 위치 및 회전 고정을 설정합니다.
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        animator = GetComponent<Animator>();
        if (animator == null) // Animator 컴포넌트가 없으면 로그를 출력하고 메서드를 종료합니다.
        {
            Debug.LogError("Animator component not found on the mob object.");
            return;
        }

        // 몬스터가 부모 객체의 자식일 경우, 부모와의 연결을 해제합니다.
        if (transform.parent != null)
        {
            transform.parent = null;
        }

        // 몬스터가 씬 전환 시에 파괴되지 않도록 설정합니다.
        DontDestroyOnLoad(gameObject);
    }


    void Update()
    {
        // 전체 정지 상태 또는 PopHeadKill 상태일 경우, 아래의 모든 행동을 중지합니다.
        if (Mob.isGlobalStop) // Mob 클래스에 정의된 isGlobalStop을 참조
        {
            // 이곳에서 추가적인 정지 조건이 필요한 경우 구현
            return;
        }

        // 플레이어가 없으면 아무것도 하지 않음
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

        // 플레이어의 위치를 향해 항상 이동하도록 변경
        Vector2 newPos = rb.position + (Vector2)direction * distanceY;
        rb.MovePosition(newPos);
        animator.SetBool("IsWalkingSchool", true);

        // 몬스터가 플레이어를 바라보도록 하는 로직
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
            Invoke("SpawnSlashithDelay", 0.7f);  // 슬래쉬가 나타나고 데미지를 주는 함수
        }
    }

    void SpawnSlashWithDelay()
    {
        // isFacingRight 변수에 따라 사용할 프리팹을 선택합니다.
        GameObject prefabToSpawn = isFacingRight ? mobSlashRightPrefab : mobSlashLeftPrefab;
        Vector3 spawnPosition = transform.position;

        // 몬스터가 바라보는 방향에 따라 생성 위치를 조정합니다.
        spawnPosition.x += isFacingRight ? 1.5f : -1.5f;

        GameObject slashInstance = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        SlashBehaviour slashBehaviour = slashInstance.GetComponent<SlashBehaviour>();
        if (slashBehaviour == null)
        {
            slashBehaviour = slashInstance.AddComponent<SlashBehaviour>();
        }
        slashBehaviour.damage = attackDamage;

        // 몬스터가 클로우를 소환하는 동안 위치를 고정시킵니다.
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;

        StartCoroutine(EnableSlashColliderAfterDelay(slashBehaviour, 0.3f));

        // 클로가 몬스터의 자식 객체가 되지 않도록 합니다 (몬스터가 움직일 때 같이 움직이지 않도록)
        slashInstance.transform.parent = null;

        // 위치 고정을 해제하는 로직을 추가합니다.
        StartCoroutine(UnfreezeAfterDelay(0.3f));

        Destroy(slashInstance, 0.3f); // 짧은 시간 후에 claw를 파괴합니다.
    }

    IEnumerator UnfreezeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // 몬스터가 다시 움직일 수 있도록 위치 고정을 해제합니다.
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