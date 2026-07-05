using UnityEngine;

// این اسکریپت رو روی آبجکت "JumpHintZone" (همون‌جایی که Box Collider 2D با Is Trigger روشن داره) بچسبون
public class JumpHint : MonoBehaviour
{
    [Tooltip("آبجکت متن هشدار (مثلاً JumpHint) که باید نشون/مخفی بشه")]
    public GameObject hintUI;

    [Tooltip("اگه true باشه، بعد از یک‌بار پریدن، این هشدار دیگه هیچ‌وقت دوباره ظاهر نمی‌شه حتی اگه بازیکن دوباره وارد ناحیه بشه")]
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

        // به‌محض فشردن دکمه‌ی پرش (Space به‌صورت پیش‌فرض توی Input Manager)، پیام مخفی می‌شه
        if (Input.GetButtonDown("Jump"))
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

        // اگه بازیکن بدون پریدن از ناحیه خارج شد (مثلاً برگشت عقب)، پیام رو مخفی کن
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