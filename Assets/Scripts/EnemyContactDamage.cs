using UnityEngine;

// این اسکریپت رو کنار EnemyPatrol روی خود آبجکت دشمن بچسبون
// نیاز به یه Collider2D جدا (مثلا BoxCollider2D یا CircleCollider2D) با تیک "Is Trigger" فعال
// این کالایدر باید دور بدن دشمن باشه (نه نورافکنش - اون قسمت جداست)
public class EnemyContactDamage : MonoBehaviour
{
    [Header("References")]
    [Tooltip("اگه خالی بمونه، خودش توی صحنه دنبالش می‌گرده")]
    public PlayerHealth playerHealth;
    [Tooltip("اگه خالی بمونه، خودش توی صحنه دنبالش می‌گرده")]
    public CheckpointManager checkpointManager;

    [Header("Layers")]
    [Tooltip("لایه‌ی آریا (Player) - فقط برخورد باهاش قلب کم می‌کنه و ریست می‌شه")]
    public LayerMask ariaLayer;

    [Header("Settings")]
    [Tooltip("بعد از یه ضربه، این‌مدت (ثانیه) دیگه ضربه نمی‌خوره - جلوگیری از کم شدن چندباره‌ی جون توی یه لحظه")]
    public float hitCooldown = 1f;

    private bool onCooldown = false;

    void Start()
    {
        if (playerHealth == null) playerHealth = FindObjectOfType<PlayerHealth>();
        if (checkpointManager == null) checkpointManager = FindObjectOfType<CheckpointManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (onCooldown) return;
        if (((1 << other.gameObject.layer) & ariaLayer) == 0) return;

        onCooldown = true;

        if (playerHealth != null) playerHealth.LoseHeart();
        if (checkpointManager != null) checkpointManager.RespawnAria();

        Invoke(nameof(ResetCooldown), hitCooldown);
    }

    void ResetCooldown()
    {
        onCooldown = false;
    }
}