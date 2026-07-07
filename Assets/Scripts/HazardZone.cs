using UnityEngine;

// این اسکریپت رو روی محدوده‌ی سمی (منطقه‌ی سبزرنگ) بچسبون
// نیاز به Box Collider 2D (یا هر Collider2D دیگه) با تیک "Is Trigger" فعال
public class HazardZone : MonoBehaviour
{
    [Tooltip("رفرنس به آبجکتی که اسکریپت CheckpointManager روشه (معمولا GameManager)")]
    public CheckpointManager checkpointManager;

    [Tooltip("فقط این لایه‌ها باعث سوختن/مرگ می‌شن - آریا و سایه رو تیک بزن")]
    public LayerMask affectedLayers;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & affectedLayers) == 0) return;

        if (checkpointManager != null)
        {
            checkpointManager.RespawnAtCheckpoint();
        }
    }
}
