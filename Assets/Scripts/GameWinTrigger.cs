using UnityEngine;
using System.Collections;

// این اسکریپت رو روی یک GameObject جدید بچسبون (مثلا "WinZone")
// جلوی در خروجی، با Box Collider 2D و تیک Is Trigger فعال
// وقتی هم آریا هم سایه هم‌زمان توی این محدوده باشن، پیغام برد میاد و بازی تموم می‌شه
public class GameWinTrigger : MonoBehaviour
{
    [Header("Layers")]
    public LayerMask ariaLayer;
    public LayerMask shadowLayer;

    [Header("Message")]
    public string winMessage = "You Win!";
    [Tooltip("چقدر بعد از نمایش پیغام صبر کنه قبل از متوقف کردن بازی")]
    public float delayBeforeEnd = 2.5f;

    private bool ariaInZone = false;
    private bool shadowInZone = false;
    private bool hasTriggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;

        if (((1 << other.gameObject.layer) & ariaLayer) != 0) ariaInZone = true;
        else if (((1 << other.gameObject.layer) & shadowLayer) != 0) shadowInZone = true;

        CheckBothPresent();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (hasTriggered) return;

        if (((1 << other.gameObject.layer) & ariaLayer) != 0) ariaInZone = false;
        else if (((1 << other.gameObject.layer) & shadowLayer) != 0) shadowInZone = false;
    }

    void CheckBothPresent()
    {
        if (hasTriggered) return;
        if (!ariaInZone || !shadowInZone) return;

        hasTriggered = true;
        StartCoroutine(WinSequence());
    }

    IEnumerator WinSequence()
    {
        if (MessageBanner.Instance != null) MessageBanner.Instance.Show(winMessage, delayBeforeEnd);

        yield return new WaitForSeconds(delayBeforeEnd);

        // بازی رو متوقف کن (می‌تونی به‌جاش SceneManager.LoadScene("MainMenu") یا "Credits" هم صدا بزنی)
        Time.timeScale = 0f;
    }
}
