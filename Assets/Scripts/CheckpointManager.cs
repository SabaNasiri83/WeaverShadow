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

    [Tooltip("فاصله‌ی سایه نسبت به آریا موقع ریست شدن - یه عدد کوچیک و امن بذار (نه فاصله‌ی اولیه‌ی صحنه که ممکنه سایه رو بیرون از زمین بندازه)")]
    public Vector3 shadowRespawnOffset = new Vector3(0.5f, 0f, 0f);

    public void RespawnAria()
    {
        if (currentCheckpoint == null || aria == null) return;

        Rigidbody2D ariaRb = aria.GetComponent<Rigidbody2D>();
        Vector3 ariaTargetPos = currentCheckpoint.position;

        if (ariaRb != null)
        {
            ariaRb.velocity = Vector2.zero;
            ariaRb.angularVelocity = 0f;
            ariaRb.position = ariaTargetPos;
        }
        else
        {
            aria.transform.position = ariaTargetPos;
        }

        Physics2D.SyncTransforms();
    }

    public void RespawnShadow()
    {
        if (currentCheckpoint == null || shadow == null) return;

        Rigidbody2D shadowRb = shadow.GetComponent<Rigidbody2D>();
        Vector3 shadowTargetPos = currentCheckpoint.position + shadowRespawnOffset;

        if (shadowRb != null)
        {
            shadowRb.velocity = Vector2.zero;
            shadowRb.angularVelocity = 0f;
            shadowRb.position = shadowTargetPos;
        }
        else
        {
            shadow.transform.position = shadowTargetPos;
        }

        Physics2D.SyncTransforms();
    }

    // نگه داشته شده برای سازگاری با قبل - هر دو کاراکتر رو با هم ریست می‌کنه
    // دیگه جایی توی پروژه صداش نمی‌زنیم؛ به‌جاش از RespawnAria() یا RespawnShadow() جدا استفاده کن
    public void RespawnAtCheckpoint()
    {
        RespawnAria();
        RespawnShadow();
    }

    // این متد رو بعدا از یک Trigger چک‌پوینت جدید صدا می‌زنیم تا موقعیت رو آپدیت کنه
    public void SetCheckpoint(Transform newCheckpoint)
    {
        currentCheckpoint = newCheckpoint;
    }
}