using UnityEngine;
using System.Collections;

// این اسکریپت رو روی یک Trigger (کلید فشاری) بچسبون
// نیاز به Box Collider 2D با تیک "Is Trigger" فعال
public class PressurePlate : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("این کلید فقط با کدوم لایه فعال بشه؟ مثلا فقط Player یا فقط Shadow")]
    public LayerMask activatorLayer;

    [Header("Status (Read Only)")]
    public bool isPressed = false;

    private SpriteRenderer spriteRenderer;
    public Color pressedColor = Color.green;
    public Color unpressedColor = Color.red;

    [Header("Glow Halo")]
    [Tooltip("رنگ هاله‌ی نور وقتی کلید فشرده میشه")]
    public Color glowColor = new Color(1f, 0.95f, 0.4f, 0.8f);
    [Tooltip("شعاع هاله (واحد یونیتی)")]
    public float glowRadius = 1.2f;
    [Tooltip("سرعت محو/ظاهر شدن هاله (ثانیه)")]
    public float glowFadeDuration = 0.3f;

    private SpriteRenderer glowRenderer;
    private Coroutine glowCoroutine;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        CreateGlowHalo();
        UpdateVisual();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & activatorLayer) != 0)
        {
            isPressed = true;
            UpdateVisual();
            SetGlow(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & activatorLayer) != 0)
        {
            isPressed = false;
            UpdateVisual();
            SetGlow(false);
        }
    }

    void UpdateVisual()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = isPressed ? pressedColor : unpressedColor;
        }
    }

    // یه بافت دایره‌ای نرم (Radial Gradient) رو موقع اجرا می‌سازه - نیازی به هیچ عکسی نیست
    void CreateGlowHalo()
    {
        int size = 128;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float maxDist = size / 2f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center) / maxDist;
                float alpha = Mathf.Clamp01(1f - dist);
                alpha = alpha * alpha; // افت نرم‌تر لبه‌ها
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }
        tex.Apply();

        Sprite glowSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size / (2f * glowRadius));

        GameObject glowObj = new GameObject("GlowHalo");
        glowObj.transform.SetParent(transform);
        glowObj.transform.localPosition = Vector3.zero;
        glowObj.transform.localScale = Vector3.one;

        glowRenderer = glowObj.AddComponent<SpriteRenderer>();
        glowRenderer.sprite = glowSprite;
        glowRenderer.color = new Color(glowColor.r, glowColor.g, glowColor.b, 0f);

        // هاله باید پشت خودِ کلید دیده بشه، نه جلوش
        if (spriteRenderer != null)
        {
            glowRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
            glowRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;
        }

        glowObj.SetActive(false);
    }

    void SetGlow(bool show)
    {
        if (glowRenderer == null) return;

        if (glowCoroutine != null) StopCoroutine(glowCoroutine);

        if (show) glowRenderer.gameObject.SetActive(true);
        glowCoroutine = StartCoroutine(FadeGlow(show));
    }

    IEnumerator FadeGlow(bool fadeIn)
    {
        float startAlpha = glowRenderer.color.a;
        float targetAlpha = fadeIn ? glowColor.a : 0f;
        float t = 0f;

        while (t < glowFadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(startAlpha, targetAlpha, t / glowFadeDuration);
            glowRenderer.color = new Color(glowColor.r, glowColor.g, glowColor.b, a);
            yield return null;
        }

        glowRenderer.color = new Color(glowColor.r, glowColor.g, glowColor.b, targetAlpha);

        if (!fadeIn)
        {
            glowRenderer.gameObject.SetActive(false);
        }
    }
}