using UnityEngine;

// این اسکریپت رو کنار PlayerController، روی خود آریا یا سایه بچسبون
// نیاز به Rigidbody2D روی همون آبجکت - جدا از PlayerController کار می‌کنه، بهش دست نمی‌زنه
[RequireComponent(typeof(Rigidbody2D))]
public class FootstepPlayer : MonoBehaviour
{
    [Header("Footstep Clips")]
    [Tooltip("چندتا نسخه‌ی مختلف صدای قدم (مثلا footstep_grass_000 تا 004)")]
    public AudioClip[] footstepClips;
    [Tooltip("حداقل سرعت افقی که لازمه تا حساب بشه داره راه می‌ره")]
    public float minSpeedToStep = 0.3f;
    [Tooltip("فاصله‌ی زمانی بین هر قدم (ثانیه) - عدد کوچیک‌تر یعنی قدم‌های تندتر")]
    public float stepInterval = 0.35f;
    [Range(0f, 1f)] public float footstepVolume = 0.6f;

    [Header("Jump Clips")]
    [Tooltip("چندتا نسخه‌ی مختلف صدای پرش")]
    public AudioClip[] jumpClips;
    [Range(0f, 1f)] public float jumpVolume = 0.8f;

    [Header("Pitch Variation")]
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;

    [Header("Ground Check (برای تشخیص لحظه‌ی پرش)")]
    [Tooltip("همون آبجکت Ground Check که روی PlayerController گذاشتی رو اینجا هم بده")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private AudioSource source;
    private float stepTimer = 0f;
    private bool wasGrounded = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        source = GetComponent<AudioSource>();
        if (source == null) source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
    }

    void Update()
    {
        HandleFootsteps();
        HandleJumpSound();
    }

    void HandleFootsteps()
    {
        bool isGroundedNow = IsGrounded();
        bool isMoving = isGroundedNow && Mathf.Abs(rb.velocity.x) > minSpeedToStep;

        if (isMoving)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                PlayRandom(footstepClips, footstepVolume);
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0f; // تا وقتی دوباره راه بره، همون لحظه‌ی اول صدا پخش بشه نه با تاخیر
        }
    }

    void HandleJumpSound()
    {
        bool isGroundedNow = IsGrounded();

        // لحظه‌ای که از زمین جدا شده و داره بالا می‌ره = لحظه‌ی پرش
        if (wasGrounded && !isGroundedNow && rb.velocity.y > 0.1f)
        {
            PlayRandom(jumpClips, jumpVolume);
        }

        wasGrounded = isGroundedNow;
    }

    bool IsGrounded()
    {
        if (groundCheck == null) return true;
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void PlayRandom(AudioClip[] clips, float volume)
    {
        if (clips == null || clips.Length == 0) return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];
        source.pitch = Random.Range(minPitch, maxPitch);
        source.PlayOneShot(clip, volume);
    }
}