using UnityEngine;
using System.Collections;

public class Coffee : MonoBehaviour
{
    public Sprite coffeeCrashSprite;
    public float attackDamage = 2f;
    public float destroyTime = 2f;
    public float fallSpeed = 2f;
    public float bounceBackForce = 20f;
    public float delayBeforeFall = 0.1f;
    public float fadeTime = 0.1f;
    public Vector2 launchDirection;
    public float distanceToFade = 4f; // 플레이어와의 거리 기준
    private bool hasCollided = false;
    private bool isFading = false;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; // 초기에는 중력이 작용하지 않도록 설정
    }

    private void Update()
    {
        if (!isFading && Vector3.Distance(startPosition, transform.position) >= distanceToFade)
        {
            StartCoroutine(FadeOutAndDestroy());
        }
    }

    private IEnumerator FadeOutAndDestroy()
    {
        isFading = true;
        float elapsed = 0f;
        Color startColor = spriteRenderer.color;
        while (elapsed < fadeTime)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 투명화 완료 후 커피 파괴
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasCollided) return;

        if (collision.collider.CompareTag("Monster"))
        {
            Vector2 bounceDirection = new Vector2(-launchDirection.x, 1).normalized * bounceBackForce;
            rb.velocity = bounceDirection;
            hasCollided = true;
            spriteRenderer.sprite = coffeeCrashSprite;
            rb.gravityScale = 1;
            Mob mobComponent = collision.collider.GetComponent<Mob>();
            Mob2 mob2Component = collision.collider.GetComponent<Mob2>();
            Mob3 mob3Component = collision.collider.GetComponent<Mob3>();

           

            if (mobComponent != null)
            {
                mobComponent.TakeDamage(attackDamage, transform);
            }

            if (mob2Component != null)
            {
                mob2Component.TakeDamage(attackDamage, transform);
            }

            if (mob3Component != null)
            {
                mob3Component.TakeDamage(attackDamage, transform);
            }

            StartCoroutine(DelayAndDestroy());
        }

        else if (collision.collider.CompareTag("Boss"))
        {
            // "Boss"와의 충돌 처리 코드
            Vector2 bounceDirection = new Vector2(-launchDirection.x, 1).normalized * bounceBackForce;
            rb.velocity = bounceDirection;
            hasCollided = true;
            spriteRenderer.sprite = coffeeCrashSprite;
            rb.gravityScale = 1;
            // 보스에게 데미지 입히기
            BossController boss = collision.collider.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(attackDamage);
            }

            StartCoroutine(DelayAndDestroy());
        }
    }


    private IEnumerator DelayAndDestroy()
    {
        yield return new WaitForSeconds(delayBeforeFall);
        if (gameObject != null)
        {
            rb.velocity = new Vector2(0, -fallSpeed);
            yield return new WaitForSeconds(destroyTime);

            // 코루틴 완료 후 커피 파괴
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }
    }
}
