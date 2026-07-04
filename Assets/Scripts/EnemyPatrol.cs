using UnityEngine;

// این اسکریپت رو روی خود آبجکت دشمن بچسبون
public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    [Tooltip("دو یا چند نقطه که دشمن بینشون رفت‌وآمد می‌کنه")]
    public Transform pointA;
    public Transform pointB;

    [Header("Settings")]
    public float speed = 2f;

    [Header("Walk Animation")]
    [Tooltip("فریم‌های راه رفتن رو به ترتیب اینجا بکش (walk0, walk1, walk2...)")]
    public Sprite[] walkFrames;
    [Tooltip("چند فریم در ثانیه عوض بشه - عدد بزرگ‌تر یعنی سریع‌تر")]
    public float framesPerSecond = 8f;

    private Transform currentTarget;
    private SpriteRenderer spriteRenderer;
    private int currentFrame;
    private float frameTimer;

    void Start()
    {
        currentTarget = pointB;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // حرکت به سمت هدف فعلی
        transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);

        // برگردوندن ظاهر دشمن بر اساس جهت حرکت (اختیاری، برای زیبایی بصری)
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = currentTarget.position.x < transform.position.x;
        }

        // اگه به هدف رسید، هدف رو عوض کن
        if (Vector2.Distance(transform.position, currentTarget.position) < 0.1f)
        {
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
        }

        AnimateWalk();
    }

    void AnimateWalk()
    {
        if (walkFrames == null || walkFrames.Length == 0 || spriteRenderer == null) return;

        frameTimer += Time.deltaTime;
        if (frameTimer >= 1f / framesPerSecond)
        {
            frameTimer = 0f;
            currentFrame = (currentFrame + 1) % walkFrames.Length;
            spriteRenderer.sprite = walkFrames[currentFrame];
        }
    }

    // برای دیدن مسیر حرکت دشمن توی Scene view (فقط بصری، تاثیری روی بازی نداره)
    void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}