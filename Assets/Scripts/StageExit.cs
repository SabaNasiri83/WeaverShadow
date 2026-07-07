using UnityEngine;
using UnityEngine.SceneManagement;

// این اسکریپت رو روی یک GameObject جدید بچسبون (مثلا "StageExitZone")
// که بعد از در قرار می‌گیره، با Box Collider 2D و تیک Is Trigger فعال
public class StageExit : MonoBehaviour
{
    [Header("Scene To Load")]
    [Tooltip("اسم دقیق صحنه‌ی بعدی (باید توی File > Build Settings اضافه شده باشه)")]
    public string nextSceneName;

    [Header("Layers")]
    [Tooltip("لایه‌ی کاراکتر آریا (Player)")]
    public LayerMask ariaLayer;
    [Tooltip("لایه‌ی کاراکتر سایه (Shadow)")]
    public LayerMask shadowLayer;

    [Header("Optional")]
    [Tooltip("قبل از رفتن به مرحله بعد، این‌مدت صبر کن (مثلا برای نمایش یه پیام یا افکت)")]
    public float delayBeforeLoad = 0.5f;

    private bool ariaInZone = false;
    private bool shadowInZone = false;
    private bool hasTriggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;

        if (((1 << other.gameObject.layer) & ariaLayer) != 0)
        {
            ariaInZone = true;
        }
        else if (((1 << other.gameObject.layer) & shadowLayer) != 0)
        {
            shadowInZone = true;
        }

        CheckBothPresent();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (hasTriggered) return;

        if (((1 << other.gameObject.layer) & ariaLayer) != 0)
        {
            ariaInZone = false;
        }
        else if (((1 << other.gameObject.layer) & shadowLayer) != 0)
        {
            shadowInZone = false;
        }
    }

    void CheckBothPresent()
    {
        if (hasTriggered) return;
        if (!ariaInZone || !shadowInZone) return;

        hasTriggered = true;
        Invoke(nameof(LoadNextScene), delayBeforeLoad);
    }

    void LoadNextScene()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogWarning("StageExit: اسم صحنه‌ی بعدی خالیه! توی Inspector مقداردهی کن.");
            return;
        }
        SceneManager.LoadScene(nextSceneName);
    }
}
