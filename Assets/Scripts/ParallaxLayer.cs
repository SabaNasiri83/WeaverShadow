using UnityEngine;

// این اسکریپت رو روی هر لایه‌ی پس‌زمینه (Sky, Mountains, Hills, Clouds) جدا بچسبون
// هر لایه با یه سرعت متفاوت حرکت می‌کنه تا حس عمق (Parallax) ایجاد شه
public class ParallaxLayer : MonoBehaviour
{
    [Header("Parallax Settings")]
    [Tooltip("۰ = کاملاً ثابت (مثل آسمون خیلی دور) / ۱ = دقیقاً هم‌سرعت با دوربین (مثل خود زمین بازی)\nمقادیر پیشنهادی: آسمون=0, کوه‌های دور=0.2, تپه‌ها=0.5, ابرها=0.1")]
    [Range(0f, 1f)]
    public float parallaxFactor = 0.3f;

    private Transform cam;
    private Vector3 lastCamPosition;

    void Start()
    {
        cam = Camera.main.transform;
        lastCamPosition = cam.position;
    }

    void LateUpdate()
    {
        Vector3 delta = cam.position - lastCamPosition;
        transform.position += new Vector3(delta.x * parallaxFactor, delta.y * parallaxFactor, 0f);
        lastCamPosition = cam.position;
    }
}
