using UnityEngine;

// این اسکریپت رو روی آبجکت BG_Sky بچسبون (همون آبجکتی که SpriteRenderer با عکس sky داره)
// خودش با شروع بازی، اندازه‌ی عکس رو دقیقاً برابر دید دوربین می‌کنه تا هیچ‌جا خالی نمونه
[RequireComponent(typeof(SpriteRenderer))]
public class FitBackgroundToCamera : MonoBehaviour
{
    [Tooltip("یه‌کم بزرگ‌تر از دید دوربین بگیر تا موقع حرکت دوربین جایی خالی نمونه (پیشنهاد: 1.2 تا 1.5)")]
    public float extraMargin = 1.3f;

    void Start()
    {
        Fit();
    }

    void Fit()
    {
        Camera cam = Camera.main;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (cam == null || sr == null || sr.sprite == null) return;

        float camHeight = cam.orthographicSize * 2f;
        float camWidth = camHeight * cam.aspect;

        float spriteWidth = sr.sprite.bounds.size.x;
        float spriteHeight = sr.sprite.bounds.size.y;

        float scaleX = (camWidth * extraMargin) / spriteWidth;
        float scaleY = (camHeight * extraMargin) / spriteHeight;

        transform.localScale = new Vector3(scaleX, scaleY, 1f);
    }
}