using UnityEngine;

// این اسکریپت رو روی همون آبجکت متن راهنما (MovementHint) بچسبون
// تا وقتی بازیکن با کلیدهای چپ/راست (یا A/D) واقعاً حرکت نکرده، این پیام رو نشون بده
public class MovementHint : MonoBehaviour
{
    [Tooltip("اگه true باشه، به‌محض فشردن کلید (حتی بدون رها کردنش) مخفی می‌شه")]
    public bool hideImmediately = true;

    [Tooltip("اختیاری: به‌جای مخفی‌شدن فوری، یه‌کم محو بشه (0 = فوری)")]
    public float fadeOutDuration = 0f;

    private CanvasGroup canvasGroup;
    private bool hasMoved = false;

    void Awake()
    {
        // اگه CanvasGroup نداشته باشه، خودش اضافه می‌کنه (برای امکان فید کردن)
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    void Update()
    {
        if (hasMoved) return;

        // بررسی حرکت واقعی روی محور افقی (همون چیزی که PlayerController هم استفاده می‌کنه)
        float moveInput = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(moveInput) > 0.01f)
        {
            hasMoved = true;

            if (fadeOutDuration > 0f)
            {
                StartCoroutine(FadeOutAndDisable());
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    System.Collections.IEnumerator FadeOutAndDisable()
    {
        float t = 0f;
        float startAlpha = canvasGroup.alpha;

        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t / fadeOutDuration);
            yield return null;
        }

        gameObject.SetActive(false);
    }
}