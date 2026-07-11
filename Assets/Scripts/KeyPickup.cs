using UnityEngine;

// این اسکریپت رو روی خود آبجکت کلید (مثلا keyYellow) بچسبون
// نیاز به Box Collider 2D یا Circle Collider 2D با تیک "Is Trigger" فعال
public class KeyPickup : MonoBehaviour
{
    [Tooltip("کدوم لایه‌ها می‌تونن این کلید رو بردارن - Player و Shadow رو تیک بزن")]
    public LayerMask collectorLayer;

    [Header("Optional")]
    [Tooltip("افکت یا صدای کوچیک هنگام برداشتن کلید (اختیاری - می‌تونی خالی بذاری)")]
    public GameObject pickupEffectPrefab;

    private KeyManager keyManager;
    private bool collected = false;

    void Start()
    {
        keyManager = FindObjectOfType<KeyManager>();

        // مطمئن می‌شیم کالایدر روی حالت Trigger باشه
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;
        if (((1 << other.gameObject.layer) & collectorLayer) == 0) return;

        collected = true;

        if (keyManager != null)
        {
            keyManager.CollectKey();
        }
        else
        {
            Debug.LogWarning("KeyPickup: هیچ KeyManager توی صحنه پیدا نشد!");
        }

        if (pickupEffectPrefab != null)
        {
            Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
