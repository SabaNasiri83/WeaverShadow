using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Collections;

// این اسکریپت رو روی آبجکت باس بچسبون
// باس از اول توی صحنه هست ولی غیرفعاله - وقتی مشعل روشن بشه (TorchTrigger.onLit) ظاهر می‌شه
public class BossController : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 5;
    private int currentHealth;

    [Header("Movement (مثل EnemyPatrol)")]
    [Tooltip("دو نقطه که باس بینشون رفت‌وآمد می‌کنه")]
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 1.5f;

    [Header("Walk Animation")]
    [Tooltip("فریم‌های راه رفتن باس - هماهنگ با حرکتش پخش می‌شن")]
    public Sprite[] walkFrames;
    [Tooltip("چند فریم در ثانیه")]
    public float framesPerSecond = 8f;

    [Header("Burn Aria On Touch")]
    [Tooltip("لایه‌ی آریا - اگه بهش بخوره می‌سوزه")]
    public LayerMask ariaLayer;
    [Tooltip("خالی بمونه خودش پیدا می‌کنه")]
    public PlayerHealth playerHealth;
    [Tooltip("خالی بمونه خودش پیدا می‌کنه")]
    public CheckpointManager checkpointManager;
    [Tooltip("بعد از هر بار سوزوندن، این‌مدت (ثانیه) دیگه آسیب نزنه")]
    public float hitCooldown = 1f;

    [Header("Hit Feedback")]
    public float hitFlashDuration = 0.1f;
    public Color hitFlashColor = Color.red;

    [Header("Health Display (بدون نوار - فقط متن)")]
    [Tooltip("اگه فعال باشه، بالای صفحه یه متن مثل Boss: 3/5 نشون می‌ده")]
    public bool showHealthCounter = true;
    public Color healthTextColor = Color.white;
    public TMP_FontAsset pixelFont;

    [Header("Death")]
    [Tooltip("افکت/پارتیکل اختیاری که موقع مرگ توی همون نقطه ساخته می‌شه")]
    public GameObject deathEffectPrefab;
    public float disableDelayAfterDeath = 0.5f;

    [Header("Sounds (اختیاری)")]
    [Tooltip("صدای ضربه خوردن - پیشنهاد: impactPunch_medium")]
    public RandomSFX hitSfx;
    [Tooltip("صدای مرگ - پیشنهاد: impactBell_heavy")]
    public RandomSFX deathSfx;

    [Header("Events")]
    public UnityEvent onDeath;

    private SpriteRenderer spriteRenderer;
    private bool isDead = false;
    private bool onCooldown = false;

    private Transform currentTarget;
    private int currentFrame;
    private float frameTimer;

    private TextMeshProUGUI healthCounterText;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;

        if (playerHealth == null) playerHealth = FindObjectOfType<PlayerHealth>();
        if (checkpointManager == null) checkpointManager = FindObjectOfType<CheckpointManager>();

        if (showHealthCounter) BuildHealthCounterUI();

        // تا وقتی مشعل روشن نشده، باس توی صحنه دیده نمی‌شه
        gameObject.SetActive(false);
    }

    // این متد رو TorchTrigger از طریق onLit صدا می‌زنه
    public void Spawn()
    {
        gameObject.SetActive(true);
        isDead = false;
        currentHealth = maxHealth;
        currentTarget = pointB != null ? pointB : pointA;

        if (healthCounterText != null)
        {
            healthCounterText.gameObject.SetActive(true);
            UpdateHealthCounter();
        }
    }

    void Update()
    {
        if (isDead) return;
        Patrol();
        AnimateWalk();
    }

    void Patrol()
    {
        if (pointA == null || pointB == null || currentTarget == null) return;

        transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, moveSpeed * Time.deltaTime);

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = currentTarget.position.x < transform.position.x;
        }

        if (Vector2.Distance(transform.position, currentTarget.position) < 0.1f)
        {
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
        }
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

    // این متد رو ShadowThrow وقتی سایه‌ی پرتاب‌شده بهش برخورد کنه صدا می‌زنه
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        UpdateHealthCounter();
        StartCoroutine(FlashHit());
        if (hitSfx != null) hitSfx.Play();

        if (currentHealth <= 0) Die();
    }

    IEnumerator FlashHit()
    {
        if (spriteRenderer == null) yield break;
        Color original = spriteRenderer.color;
        spriteRenderer.color = hitFlashColor;
        yield return new WaitForSeconds(hitFlashDuration);
        spriteRenderer.color = original;
    }

    void Die()
    {
        isDead = true;

        if (deathSfx != null) deathSfx.Play();

        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        if (healthCounterText != null) healthCounterText.gameObject.SetActive(false);

        onDeath?.Invoke();
        StartCoroutine(DieAndDisable());
    }

    IEnumerator DieAndDisable()
    {
        yield return new WaitForSeconds(disableDelayAfterDeath);
        gameObject.SetActive(false);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        HandleAriaTouch(collision.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        HandleAriaTouch(other.gameObject);
    }

    void HandleAriaTouch(GameObject other)
    {
        if (isDead || onCooldown) return;
        if (((1 << other.layer) & ariaLayer) == 0) return;

        onCooldown = true;

        if (playerHealth != null) playerHealth.LoseHeart();
        if (checkpointManager != null) checkpointManager.RespawnAria();

        Invoke(nameof(ResetCooldown), hitCooldown);
    }

    void ResetCooldown()
    {
        onCooldown = false;
    }

    // برای دیدن مسیر حرکت باس توی Scene view
    void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }

    // ---------- UI شمارنده‌ی جون (بدون نوار، فقط متن) ----------

    void BuildHealthCounterUI()
    {
        GameObject canvasObj = new GameObject("BossHealthHUD_Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject textObj = new GameObject("BossHealthText");
        textObj.transform.SetParent(canvasObj.transform, false);
        RectTransform textRt = textObj.AddComponent<RectTransform>();
        textRt.anchorMin = new Vector2(0.5f, 1f);
        textRt.anchorMax = new Vector2(0.5f, 1f);
        textRt.pivot = new Vector2(0.5f, 1f);
        textRt.anchoredPosition = new Vector2(0, -40);
        textRt.sizeDelta = new Vector2(400, 60);

        healthCounterText = textObj.AddComponent<TextMeshProUGUI>();
        if (pixelFont != null) healthCounterText.font = pixelFont;
        healthCounterText.fontSize = 32;
        healthCounterText.alignment = TextAlignmentOptions.Center;
        healthCounterText.color = healthTextColor;

        healthCounterText.gameObject.SetActive(false); // تا اسپاون نشده نشون نده
    }

    void UpdateHealthCounter()
    {
        if (healthCounterText == null) return;
        healthCounterText.text = "Boss: " + Mathf.Max(currentHealth, 0) + " / " + maxHealth;
    }
}