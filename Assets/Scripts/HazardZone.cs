using UnityEngine;

// این اسکریپت رو روی محدوده‌ی خطر (مثلا ToxicZone) بچسبون
// نیاز به Box Collider 2D (یا هر Collider2D دیگه) با تیک "Is Trigger" فعال
//
// تغییر مهم نسبت به قبل: حالا این اسکریپت تشخیص می‌ده کدوم کاراکتر بهش خورده
// - اگه آریا بخوره: هم یه قلب کم می‌شه، هم برمی‌گرده به چک‌پوینت
// - اگه سایه بخوره: فقط برمی‌گرده به چک‌پوینت (هیچ قلبی کم نمی‌شه)
public class HazardZone : MonoBehaviour
{
    [Header("References")]
    [Tooltip("رفرنس به آبجکتی که اسکریپت CheckpointManager روشه (معمولا GameManager)")]
    public CheckpointManager checkpointManager;

    [Tooltip("رفرنس به آبجکتی که اسکریپت PlayerHealth روشه (معمولا همون GameManager)")]
    public PlayerHealth playerHealth;

    [Header("Layers")]
    [Tooltip("لایه‌ی کاراکتر آریا (Player) - برخوردش باعث کم شدن جون هم می‌شه")]
    public LayerMask ariaLayer;

    [Tooltip("لایه‌ی کاراکتر سایه (Shadow) - برخوردش فقط ریست می‌کنه، جون کم نمی‌شه")]
    public LayerMask shadowLayer;

    void OnTriggerEnter2D(Collider2D other)
    {
        bool isAria = ((1 << other.gameObject.layer) & ariaLayer) != 0;
        bool isShadow = ((1 << other.gameObject.layer) & shadowLayer) != 0;

        if (!isAria && !isShadow) return;

        if (isAria)
        {
            if (playerHealth != null) playerHealth.LoseHeart();
            if (checkpointManager != null) checkpointManager.RespawnAria();
        }
        else if (isShadow)
        {
            if (checkpointManager != null) checkpointManager.RespawnShadow();
        }
    }
}