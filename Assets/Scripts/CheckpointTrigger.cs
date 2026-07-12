using UnityEngine;

// این اسکریپت رو روی هر نقطه‌ای که می‌خوای چک‌پوینت باشه بچسبون
// نیاز به Box Collider2D با تیک "Is Trigger" فعال
// می‌تونی چندتا از این توی صحنه داشته باشی (یکی وسط مپ، یکی قبل مشعل، و...)
public class CheckpointTrigger : MonoBehaviour
{
    [Header("References")]
    [Tooltip("خالی بمونه خودش توی صحنه پیدا می‌کنه")]
    public CheckpointManager checkpointManager;

    [Header("Layers")]
    [Tooltip("فقط عبور آریا باعث آپدیت شدن چک‌پوینت می‌شه")]
    public LayerMask ariaLayer;

    [Header("Optional")]
    [Tooltip("اگه خالی بمونه، خودِ همین آبجکت به‌عنوان نقطه‌ی ریسپاون استفاده می‌شه. اگه می‌خوای دقیق‌تر جای ریسپاون رو جدا کنترل کنی، یه Transform دیگه اینجا بده")]
    public Transform respawnPoint;

    private bool activated = false;

    void Start()
    {
        if (checkpointManager == null) checkpointManager = FindObjectOfType<CheckpointManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & ariaLayer) == 0) return;
        if (checkpointManager == null) return;

        Transform target = respawnPoint != null ? respawnPoint : transform;
        checkpointManager.SetCheckpoint(target);
        activated = true;
    }

    // برای دیدن چک‌پوینت‌های فعال‌شده توی Scene view (فقط بصری)
    void OnDrawGizmos()
    {
        Gizmos.color = activated ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}
