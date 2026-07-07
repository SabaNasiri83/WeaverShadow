using UnityEngine;

// این اسکریپت رو روی آبجکت "ShadowSwitchHintZone" (یه Box Collider 2D با Is Trigger روشن، نزدیک اولین برخورد با Shadow) بچسبون
public class ShadowSwitchHint : MonoBehaviour
{
    [Tooltip("آبجکت متن هشدار (مثلاً ShadowSwitchHint) که باید نشون/مخفی بشه")]
    public GameObject hintUI;

    [Tooltip("اگه true باشه، بعد از یک‌بار سوییچ کردن، این هشدار دیگه هیچ‌وقت دوباره ظاهر نمی‌شه")]
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

        // همون کلیدی که ShadowSwitcher برای سوییچ استفاده می‌کنه: Q
        if (Input.GetKeyDown(KeyCode.Q))
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
