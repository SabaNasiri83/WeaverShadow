using UnityEngine;

// این اسکریپت رو به‌جای ParallaxLayer روی BG_Mountains و BG_Hills بگذار
// (اونایی که چند تا بچه/کپی از یه عکس دارن و باید بی‌نهایت تکرار شن)
//
// نحوه‌ی چیدمان بچه‌ها: فقط کافیه چندتا کپی از عکس (مثلاً 3 یا 4 تا) رو
// دقیقاً کنار هم (بدون فاصله، لبه‌به‌لبه) روی محور X بچینی. خودِ اسکریپت
// بقیه‌ش رو حل می‌کنه و بی‌نهایت تکرارشون می‌کنه.
public class InfiniteParallaxLayer : MonoBehaviour
{
    [Header("Parallax Settings")]
    [Tooltip("۰ = بی‌حرکت، ۱ = هم‌سرعت با دوربین")]
    [Range(0f, 1f)]
    public float parallaxFactor = 0.3f;

    [Header("Infinite Scroll")]
    [Tooltip("چقدر جلوتر از دیدِ دوربین یه تکه رو دوباره بفرستیم اونور (مقدار پیش‌فرض معمولاً کافیه)")]
    public float extraBuffer = 2f;

    private Transform cam;
    private Vector3 lastCamPosition;
    private SpriteRenderer[] pieces;
    private float totalWidth;

    void Start()
    {
        cam = Camera.main.transform;
        lastCamPosition = cam.position;

        pieces = GetComponentsInChildren<SpriteRenderer>();
        if (pieces.Length == 0)
        {
            Debug.LogWarning(name + ": هیچ SpriteRenderer ای زیرش پیدا نشد. باید چندتا آبجکت بچه با عکس داشته باشه.");
            return;
        }

        // مرتب‌سازی بچه‌ها از چپ به راست بر اساس موقعیت X
        System.Array.Sort(pieces, (a, b) => a.transform.position.x.CompareTo(b.transform.position.x));

        // چیدمان خودکار: دیگه لازم نیست با چشم دقیق کنار هم بچینیشون.
        // هر تکه رو دقیقاً بعد از تکه‌ی قبلی می‌چسبونیم (لبه‌به‌لبه، بدون فاصله و بدون هم‌پوشانی)
        // این کار مشکل شکاف/هم‌پوشانی که از چیدمان دستی ناشی می‌شه رو کاملاً حذف می‌کنه.
        float cursor = pieces[0].bounds.min.x; // از لبه‌ی چپ اولین تکه شروع می‌کنیم (جایی که خودت گذاشتیش)
        foreach (var piece in pieces)
        {
            float halfWidth = piece.bounds.size.x / 2f;
            float targetCenterX = cursor + halfWidth;
            float deltaX = targetCenterX - piece.transform.position.x;
            piece.transform.position += new Vector3(deltaX, 0f, 0f);
            cursor += piece.bounds.size.x;
        }

        // محاسبه‌ی عرض کل ردیف (حالا که خودمون دقیق چیدیمشون، این عدد کاملاً درسته)
        float leftEdge = pieces[0].bounds.min.x;
        float rightEdge = pieces[pieces.Length - 1].bounds.max.x;
        totalWidth = rightEdge - leftEdge;
    }

    void LateUpdate()
    {
        if (pieces == null || pieces.Length == 0) return;

        // حرکت پارالاکس معمولی
        Vector3 delta = cam.position - lastCamPosition;
        transform.position += new Vector3(delta.x * parallaxFactor, delta.y * parallaxFactor, 0f);
        lastCamPosition = cam.position;

        float camHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;

        if (totalWidth <= 0.01f) return; // جلوگیری از حلقه‌ی بی‌پایان اگه فقط یه تکه باشه یا عرضش صفر باشه

        foreach (var piece in pieces)
        {
            // به‌جای یه بار جابه‌جایی، تا وقتی کامل داخل محدوده نیاد ادامه بده
            // (این کار جلوی چشمک‌زدن/رفت‌وبرگشت رو می‌گیره، مخصوصاً وقتی totalWidth کوچیکه)
            int safety = 0;
            while (piece.transform.position.x - cam.position.x < -(camHalfWidth + extraBuffer) && safety < 50)
            {
                piece.transform.position += new Vector3(totalWidth, 0f, 0f);
                safety++;
            }
            safety = 0;
            while (piece.transform.position.x - cam.position.x > (camHalfWidth + extraBuffer) && safety < 50)
            {
                piece.transform.position -= new Vector3(totalWidth, 0f, 0f);
                safety++;
            }
        }
    }
}