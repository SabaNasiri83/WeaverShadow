using UnityEngine;

// این کامپوننت به‌صورت خودکار توسط EnemyLightWave روی هاله‌ی نور ساخته می‌شه
// نیازی نیست دستی جایی بچسبونیش
public class HaloShadowDetector : MonoBehaviour
{
    public LayerMask shadowLayer;
    public CheckpointManager checkpointManager;

    void Start()
    {
        if (checkpointManager == null) checkpointManager = FindObjectOfType<CheckpointManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // درست مثل LightDetector - فقط سایه رو تشخیص می‌ده، فقط ریست می‌کنه، قلب کم نمی‌کنه
        if (((1 << other.gameObject.layer) & shadowLayer) != 0)
        {
            if (checkpointManager != null)
            {
                checkpointManager.RespawnShadow();
            }
        }
    }
}