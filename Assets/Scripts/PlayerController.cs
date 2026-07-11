using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    [Tooltip("وقتی خم شده (Duck) هستیم، حرکت افقی با این ضریب کند می‌شه")]
    public float duckSpeedMultiplier = 0.5f;

    [Header("Double Tap Jump")]
    [Tooltip("اگه توی این بازه زمانی (ثانیه) بعد از پرش اول، دوباره کلید پرش زده بشه، پرش دوم بلندتر انجام می‌شه")]
    public float doubleTapWindow = 0.35f;
    [Tooltip("ضریب ارتفاع پرش دوم نسبت به پرش عادی (مثلا 1.4 یعنی ۴۰٪ بلندتر)")]
    public float doubleJumpForceMultiplier = 1.4f;

    private float firstJumpPressTime = -10f;
    private bool usedDoubleJump = false;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    [Header("Duck Collider")]
    [Tooltip("اگه خالی بمونه، خودش سعی می‌کنه BoxCollider2D روی همین آبجکت رو پیدا کنه")]
    public BoxCollider2D bodyCollider;
    [Tooltip("موقع خم‌شدن، ارتفاع Collider به این نسبت از ارتفاع اصلی کوچیک می‌شه (مثلاً 0.5 یعنی نصف)")]
    public float duckColliderHeightMultiplier = 0.5f;

    private Vector2 standingColliderSize;
    private Vector2 standingColliderOffset;
    private bool wasDucking;

    [Header("Animation")]
    [Tooltip("اگه خالی بمونه، خودش سعی می‌کنه روی همین آبجکت یا فرزندش پیدا کنه")]
    public Animator animator;

    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    private bool isDucking;

    [Header("Ladder")]
    [Tooltip("وقتی true باشه، اسکریپت دیگه‌ای (مثل LadderClimb) کنترل حرکت رو دست گرفته - این اسکریپت هیچ حرکتی اعمال نمی‌کنه")]
    public bool isClimbing = false;

    // نام دقیق پارامترهای Animator Controller (باید با AriaAnimatorSetup هماهنگ باشه)
    private static readonly int SpeedParam = Animator.StringToHash("Speed");
    private static readonly int GroundedParam = Animator.StringToHash("Grounded");
    private static readonly int VSpeedParam = Animator.StringToHash("VSpeed");
    private static readonly int DuckingParam = Animator.StringToHash("Ducking");
    private static readonly int HurtParam = Animator.StringToHash("Hurt");

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (animator == null) animator = GetComponentInChildren<Animator>();

        if (bodyCollider == null) bodyCollider = GetComponent<BoxCollider2D>();
        if (bodyCollider != null)
        {
            standingColliderSize = bodyCollider.size;
            standingColliderOffset = bodyCollider.offset;
        }
    }

    void Update()
    {
        if (isClimbing) return; // موقع بالا رفتن از نردبون، این اسکریپت هیچ کنترلی اعمال نمی‌کنه

        moveInput = Input.GetAxisRaw("Horizontal");

        // خم شدن/نشستن: کلید پایین یا S، فقط وقتی روی زمینیم
        isDucking = isGrounded && Input.GetAxisRaw("Vertical") < -0.5f;

        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        // هر بار که پاش به زمین می‌رسه، اجازه‌ی پرش دوم دوباره فعال می‌شه
        if (isGrounded)
        {
            usedDoubleJump = false;
        }

        HandleJumpInput();

        UpdateDuckCollider();
        UpdateAnimator();
    }

    void HandleJumpInput()
    {
        if (!Input.GetButtonDown("Jump") || isDucking) return;

        if (isGrounded)
        {
            // پرش اول - ارتفاع عادی
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            firstJumpPressTime = Time.time;
            usedDoubleJump = false;
        }
        else if (!usedDoubleJump && Time.time - firstJumpPressTime <= doubleTapWindow)
        {
            // پرش دوم - فقط اگه توی بازه‌ی زمانی کوتاه بعد از پرش اول دوباره زده بشه، بلندتر می‌پره
            rb.velocity = new Vector2(rb.velocity.x, jumpForce * doubleJumpForceMultiplier);
            usedDoubleJump = true;
        }
    }

    void FixedUpdate()
    {
        if (isClimbing) return; // Rigidbody رو LadderClimb مستقیم کنترل می‌کنه

        float speedMultiplier = isDucking ? duckSpeedMultiplier : 1f;
        rb.velocity = new Vector2(moveInput * moveSpeed * speedMultiplier, rb.velocity.y);
    }

    // موقع خم‌شدن، Collider رو کوچیک می‌کنه تا واقعاً زیر مانع‌های کوتاه جا بشه
    // لبه‌ی پایین Collider ثابت می‌مونه (روی زمین) تا کاراکتر توی زمین فرو نره
    void UpdateDuckCollider()
    {
        if (bodyCollider == null) return;
        if (isDucking == wasDucking) return;

        if (isDucking)
        {
            float newHeight = standingColliderSize.y * duckColliderHeightMultiplier;
            float heightDiff = standingColliderSize.y - newHeight;
            bodyCollider.size = new Vector2(standingColliderSize.x, newHeight);
            bodyCollider.offset = new Vector2(standingColliderOffset.x, standingColliderOffset.y - heightDiff / 2f);
        }
        else
        {
            bodyCollider.size = standingColliderSize;
            bodyCollider.offset = standingColliderOffset;
        }

        wasDucking = isDucking;
    }

    void UpdateAnimator()
    {
        if (animator == null) return;

        animator.SetFloat(SpeedParam, Mathf.Abs(moveInput));
        animator.SetBool(GroundedParam, isGrounded);
        animator.SetFloat(VSpeedParam, rb.velocity.y);
        animator.SetBool(DuckingParam, isDucking);
    }

    // این متد رو از جاهایی مثل CheckpointManager یا LightDetector صدا بزن تا انیمیشن ضربه‌خوردن پخش شه
    public void TriggerHurt()
    {
        if (animator != null) animator.SetTrigger(HurtParam);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}