using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// این اسکریپت رو روی یک Trigger جدا (نه خودِ گرافیک نردبون) بچسبون
// که دقیقاً جلوی نردبون، از پایین تا بالای اون کشیده شده
// نیاز به Box Collider 2D با تیک "Is Trigger" فعال
public class LadderClimb : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("فقط این لایه‌ها می‌تونن از نردبون بالا برن - آریا و سایه رو تیک بزن")]
    public LayerMask climberLayer;

    [Tooltip("سرعت بالا رفتن (واحد در ثانیه) - فقط وقتی کلید بالا نگه داشته بشه")]
    public float climbSpeed = 3f;

    [Tooltip("نقطه‌ای که کاراکتر باید بهش برسه (بالای نردبون)")]
    public Transform topPoint;

    [Header("Climb Animation")]
    [Tooltip("فریم‌های انیمیشن بالا رفتن (climb0, climb1)")]
    public Sprite[] climbFrames;
    [Tooltip("هر فریم چقدر طول بکشه")]
    public float frameDuration = 0.15f;

    private class ClimbData
    {
        public Coroutine climbCoroutine;
        public Coroutine animCoroutine;
        public float originalGravity;
        public RigidbodyType2D originalBodyType;
        public Animator animator;
        public bool animatorWasEnabled;
    }

    private readonly HashSet<Collider2D> collidersInZone = new HashSet<Collider2D>();
    private readonly Dictionary<Collider2D, ClimbData> activeClimbs = new Dictionary<Collider2D, ClimbData>();

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & climberLayer) == 0) return;
        collidersInZone.Add(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        collidersInZone.Remove(other);
        // توجه: عمداً دیگه بالارفتنِ درحال‌انجام رو قطع نمی‌کنیم -
        // چون اگه محدوده‌ی Trigger دقیقاً تا ارتفاع LadderTop نره، خروج زودهنگام از Trigger
        // باعث می‌شد کاراکتر قبل از رسیدن به بالا، نصفه‌کاره رها بشه.
        // حالا هر بار که بالا رفتن شروع بشه، تا رسیدن کامل به LadderTop ادامه پیدا می‌کنه.
    }

    void Update()
    {
        bool holdingUp = Input.GetAxisRaw("Vertical") > 0.5f;
        if (!holdingUp) return;

        foreach (var col in collidersInZone)
        {
            if (col == null) continue;
            if (activeClimbs.ContainsKey(col)) continue;

            PlayerController controller = col.GetComponent<PlayerController>();
            if (controller == null || !controller.enabled) continue; // فقط کاراکتر فعال الان کنترل داره

            Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
            if (rb == null || topPoint == null) continue;

            ClimbData data = new ClimbData();
            data.climbCoroutine = StartCoroutine(ClimbWhileHeld(col, controller, rb, data));
            activeClimbs[col] = data;
        }
    }

    IEnumerator ClimbWhileHeld(Collider2D col, PlayerController controller, Rigidbody2D rb, ClimbData data)
    {
        controller.isClimbing = true;
        data.originalGravity = rb.gravityScale;
        data.originalBodyType = rb.bodyType;
        rb.bodyType = RigidbodyType2D.Kinematic; // موقتا Kinematic می‌کنیم تا هیچ Collider جامدی جلوی بالا رفتن رو نگیره
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;

        SpriteRenderer spriteRenderer = col.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) spriteRenderer = col.GetComponentInChildren<SpriteRenderer>();

        data.animator = col.GetComponent<Animator>();
        if (data.animator == null) data.animator = col.GetComponentInChildren<Animator>();

        if (data.animator != null)
        {
            data.animatorWasEnabled = data.animator.enabled;
            data.animator.enabled = false; // موقتا Animator رو خاموش می‌کنیم تا فریم‌های دستی جایگزینش بشن
        }

        if (spriteRenderer != null && climbFrames != null && climbFrames.Length > 0)
        {
            data.animCoroutine = StartCoroutine(PlayClimbFrames(spriteRenderer));
        }

        while (rb.position.y < topPoint.position.y)
        {
            bool stillHoldingUp = Input.GetAxisRaw("Vertical") > 0.5f;
            rb.velocity = Vector2.zero;

            if (stillHoldingUp)
            {
                Vector2 newPos = rb.position + Vector2.up * climbSpeed * Time.fixedDeltaTime;
                rb.MovePosition(newPos);
            }
            // اگه کلید بالا رها بشه، همینجا معلق می‌مونه (نه سقوط، نه ادامه‌ی مسیر) تا دوباره نگهش داره

            yield return new WaitForFixedUpdate();
        }

        rb.position = new Vector2(rb.position.x, topPoint.position.y);
        FinishClimb(col, data);
    }

    IEnumerator PlayClimbFrames(SpriteRenderer spriteRenderer)
    {
        int i = 0;
        while (true)
        {
            spriteRenderer.sprite = climbFrames[i % climbFrames.Length];
            i++;
            yield return new WaitForSeconds(frameDuration);
        }
    }

    void FinishClimb(Collider2D col, ClimbData data)
    {
        if (data.animCoroutine != null) StopCoroutine(data.animCoroutine);

        if (data.animator != null)
        {
            data.animator.enabled = data.animatorWasEnabled;
        }

        Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = data.originalGravity;
            rb.bodyType = data.originalBodyType;
        }

        PlayerController controller = col.GetComponent<PlayerController>();
        if (controller != null) controller.isClimbing = false;

        activeClimbs.Remove(col);
    }
}