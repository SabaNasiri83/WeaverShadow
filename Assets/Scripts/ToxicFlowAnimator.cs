using UnityEngine;
using System.Collections;

// این اسکریپت رو روی خودِ آبجکتِ «جریان مواد سمی» بچسبون
// (همون تیکه‌ای که از لوله می‌ریزه پایین - نه کل لوله، فقط بخش ریزشِ متحرک)
// نیاز به SpriteRenderer روی همین آبجکت
public class ToxicFlowAnimator : MonoBehaviour
{
    [Header("Animation Frames")]
    [Tooltip("فریم‌های پشت‌سرهم انیمیشن ریختن مواد - به ترتیب پخش و لوپ می‌شن")]
    public Sprite[] flowFrames;
    [Tooltip("هر فریم چقدر طول بکشه (ثانیه)")]
    public float frameDuration = 0.1f;

    [Header("Status (Read Only)")]
    public bool isFlowing = true;

    [Header("Collider (اختیاری)")]
    [Tooltip("اگه کالایدرِ این جریان روی همین آبجکته، خالی بذار (خودکار پیدا می‌شه). اگه کالایدر روی یه آبجکت دیگه‌ست (مثلاً پرنت مشترک همه‌ی تایل‌ها)، خودت بکشش اینجا")]
    public Collider2D flowCollider;

    private SpriteRenderer spriteRenderer;
    private Coroutine animCoroutine;
    private int frameIndex = 0;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // اگه دستی چیزی ست نشده بود، توی همین آبجکت دنبالش بگرد
        if (flowCollider == null) flowCollider = GetComponent<Collider2D>();
    }

    void Start()
    {
        if (isFlowing)
        {
            if (spriteRenderer != null) spriteRenderer.enabled = true;
            if (flowCollider != null) flowCollider.enabled = true;
            animCoroutine = StartCoroutine(PlayLoop());
        }
        else
        {
            if (spriteRenderer != null) spriteRenderer.enabled = false;
            if (flowCollider != null) flowCollider.enabled = false;
        }
    }

    // این متد رو ToxicLever صدا می‌زنه وقتی سایه کنار اهرم می‌ایسته
    public void StopFlow()
    {
        if (!isFlowing) return;
        isFlowing = false;

        if (animCoroutine != null) StopCoroutine(animCoroutine);
        // دیگه چیزی از لوله نمی‌ریزه - هم تصویر هم کالایدرش خاموش می‌شه
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (flowCollider != null) flowCollider.enabled = false;
    }

    // این متد رو ToxicLever صدا می‌زنه وقتی سایه از کنار اهرم دور می‌شه
    public void StartFlow()
    {
        if (isFlowing) return;
        isFlowing = true;

        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (flowCollider != null) flowCollider.enabled = true;
        if (animCoroutine != null) StopCoroutine(animCoroutine);
        animCoroutine = StartCoroutine(PlayLoop());
    }

    IEnumerator PlayLoop()
    {
        if (flowFrames == null || flowFrames.Length == 0) yield break;

        while (true)
        {
            spriteRenderer.sprite = flowFrames[frameIndex];
            frameIndex = (frameIndex + 1) % flowFrames.Length;
            yield return new WaitForSeconds(frameDuration);
        }
    }
}