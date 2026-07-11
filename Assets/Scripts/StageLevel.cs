using UnityEngine;
using System.Collections;

// این اسکریپت رو روی همون آبجکت متن راهنما (MovementHint) بچسبون
// تا وقتی بازی شروع میشه، پیام رو نشون بده و بعد از ۲ ثانیه محو بشه
public class StageLevel: MonoBehaviour
{
    [Tooltip("مدت زمانی که متن قبل از محو شدن نمایش داده میشه (به ثانیه)")]
    public float displayDuration = 2f;

    [Tooltip("مدت زمان محو شدن (به ثانیه)")]
    public float fadeOutDuration = 1f;

    private CanvasGroup canvasGroup;

    void Awake()
    {
        // اگه CanvasGroup نداشته باشه، خودش اضافه می‌کنه (برای امکان فید کردن)
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // تنظیم شفافیت اولیه به ۱ (کاملاً قابل مشاهده)
        canvasGroup.alpha = 1f;
    }

    void Start()
    {
        // شروع پروسه: منتظر بمون، بعد محو کن
        StartCoroutine(WaitAndFadeOut());
    }

    IEnumerator WaitAndFadeOut()
    {
        // اول به مدت displayDuration صبر کن (مثلاً ۲ ثانیه)
        yield return new WaitForSeconds(displayDuration);

        // بعد شروع به محو شدن کن
        yield return StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float t = 0f;
        float startAlpha = canvasGroup.alpha;

        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t / fadeOutDuration);
            yield return null;
        }

        // بعد از محو شدن کامل، GameObject رو غیرفعال کن
        gameObject.SetActive(false);
    }
}