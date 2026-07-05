using UnityEngine;

// این اسکریپت رو روی آبجکت Shadow بچسبون
// چون سایه از همون اسپرایت‌ها و همون Animator Controller آریا استفاده می‌کنه،
// این اسکریپت فقط رنگ رندرر رو تیره می‌کنه تا ظاهرش شبیه سایه بشه (بدون نیاز به اسپرایت‌های جدا)
[RequireComponent(typeof(SpriteRenderer))]
public class ShadowSilhouette : MonoBehaviour
{
    [Header("Silhouette Look")]
    [Tooltip("رنگ نهایی سایه - نزدیک به سیاه، کمی آبی/بنفش برای حس مرموز")]
    public Color silhouetteColor = new Color(0.05f, 0.05f, 0.1f, 1f);

    [Tooltip("افکت لرزش/اعوجاج جزئی هنگام کنترل سایه (اختیاری - حس دنیای معکوس رو تشدید می‌کنه)")]
    public bool subtleWobble = true;
    public float wobbleAmount = 0.02f;
    public float wobbleSpeed = 8f;

    private SpriteRenderer spriteRenderer;
    private Vector3 baseScale;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = silhouetteColor;
        baseScale = transform.localScale;
    }

    void Update()
    {
        if (subtleWobble)
        {
            float wobble = 1f + Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount;
            transform.localScale = new Vector3(baseScale.x * wobble, baseScale.y, baseScale.z);
        }
    }
}
