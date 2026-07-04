using UnityEngine;

// این اسکریپت رو روی یک آبجکت فرزند دشمن (مخروط نور) بچسبون
// نیاز به Collider2D با تیک "Is Trigger" فعال
public class LightDetector : MonoBehaviour
{
    [Tooltip("فقط با این لایه‌ها برخورد رو تشخیص بده (باید Shadow باشه)")]
    public LayerMask detectLayer;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & detectLayer) != 0)
        {
            // پیدا کردن مدیر چک‌پوینت توی صحنه و صدا زدن شکست بازیکن
            CheckpointManager checkpointManager = FindObjectOfType<CheckpointManager>();
            if (checkpointManager != null)
            {
                checkpointManager.RespawnAtCheckpoint();
            }
        }
    }
}