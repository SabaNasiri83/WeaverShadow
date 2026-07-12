using UnityEngine;
using System.Collections;
using TMPro;

// این اسکریپت رو روی یه Canvas بذار (یا بساز) که یه TextMeshProUGUI وسط صفحه داره
// از هر اسکریپت دیگه‌ای می‌تونی با MessageBanner.Instance.Show("متن") صداش بزنی
public class MessageBanner : MonoBehaviour
{
    public static MessageBanner Instance;

    [Header("UI")]
    [Tooltip("متن TMP که پیام روش نشون داده می‌شه (باید از اول توی صحنه باشه)")]
    public TextMeshProUGUI messageText;

    [Header("Timing")]
    public float defaultDisplayDuration = 2f;
    public float fadeDuration = 0.5f;

    private CanvasGroup canvasGroup;
    private Coroutine showCoroutine;

    void Awake()
    {
        Instance = this;

        if (messageText == null) return;

        canvasGroup = messageText.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = messageText.gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        messageText.gameObject.SetActive(false);
    }

    // مثلاً: MessageBanner.Instance.Show("Boss Fight!");
    public void Show(string msg)
    {
        Show(msg, defaultDisplayDuration);
    }

    public void Show(string msg, float duration)
    {
        if (messageText == null) return;

        if (showCoroutine != null) StopCoroutine(showCoroutine);
        showCoroutine = StartCoroutine(ShowRoutine(msg, duration));
    }

    IEnumerator ShowRoutine(string msg, float duration)
    {
        messageText.text = msg;
        messageText.gameObject.SetActive(true);
        canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(duration);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        messageText.gameObject.SetActive(false);
    }
}
