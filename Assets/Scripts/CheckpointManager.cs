using UnityEngine;

// این اسکریپت رو روی همون آبجکت "GameManager" که قبلا ساختی بچسبون
public class CheckpointManager : MonoBehaviour
{
    [Header("Characters")]
    public GameObject aria;
    public GameObject shadow;

    [Header("Checkpoint")]
    [Tooltip("نقطه‌ای که بازیکن بعد از شکست بهش برمی‌گرده - یک GameObject خالی بساز و اینجا بگذار")]
    public Transform currentCheckpoint;

    // فاصله‌ی نسبی سایه از آریا رو حفظ می‌کنیم تا بعد از ریست هم رعایت شه
    private Vector3 shadowOffsetFromAria;

    void Start()
    {
        if (aria != null && shadow != null)
        {
            shadowOffsetFromAria = shadow.transform.position - aria.transform.position;
        }
    }

    public void RespawnAtCheckpoint()
    {
        if (currentCheckpoint == null) return;

        Rigidbody2D ariaRb = aria.GetComponent<Rigidbody2D>();
        Rigidbody2D shadowRb = shadow.GetComponent<Rigidbody2D>();

        // متوقف کردن سرعت فعلی قبل از جابه‌جایی (وگرنه ممکنه بعد از تله‌پورت پرتاب شن)
        if (ariaRb != null) ariaRb.velocity = Vector2.zero;
        if (shadowRb != null) shadowRb.velocity = Vector2.zero;

        aria.transform.position = currentCheckpoint.position;
        shadow.transform.position = currentCheckpoint.position + shadowOffsetFromAria;
    }

    // این متد رو بعدا از یک Trigger چک‌پوینت جدید صدا می‌زنیم تا موقعیت رو آپدیت کنه
    public void SetCheckpoint(Transform newCheckpoint)
    {
        currentCheckpoint = newCheckpoint;
    }
}