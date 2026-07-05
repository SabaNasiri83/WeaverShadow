using UnityEngine;

// این اسکریپت رو روی آبجکت "DuckHintZone" (یه Box Collider 2D با Is Trigger روشن، قبل از مانع) بچسبون
public class DuckHint : MonoBehaviour
{
    [Tooltip("آبجکت متن هشدار (مثلاً DuckHint) که باید نشون/مخفی بشه")]
    public GameObject hintUI;

    [Tooltip("اگه true باشه، بعد از یک‌بار خم‌شدن، این هشدار دیگه هیچ‌وقت دوباره ظاهر نمی‌شه")]
    public bool onlyShowOnce = true;

    private bool playerInZone = false;
    private bool alreadyTriggered = false;

    void Start()
    {
        if (hintUI != null) hintUI.SetActive(false);
    }

    void Update()
    {
        if (!playerInZone) return;
        if (onlyShowOnce && alreadyTriggered) return;

        // همون شرطی که PlayerController برای تشخیص خم‌شدن استفاده می‌کنه: S یا فلش پایین
        if (Input.GetAxisRaw("Vertical") < -0.5f)
        {
            HideHint();
            alreadyTriggered = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (onlyShowOnce && alreadyTriggered) return;

        playerInZone = true;
        if (hintUI != null) hintUI.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInZone = false;

        if (!alreadyTriggered)
        {
            HideHint();
        }
    }

    void HideHint()
    {
        if (hintUI != null) hintUI.SetActive(false);
    }
}
