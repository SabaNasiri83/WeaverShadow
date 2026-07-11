using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

// این اسکریپت رو روی همون آبجکت "GameManager" بچسبون
// خودش یه UI بالا-سمت‌راست می‌سازه: آیکون کلید + شمارنده (مثلا 0 / 5)
public class KeyManager : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("تعداد کل کلیدهایی که توی این مرحله پخش کردی")]
    public int totalKeys = 5;

    [Header("UI")]
    [Tooltip("اسپرایت آیکون کلید (مثلا Sprites/Tiles/Common_Interactive/keyYellow)")]
    public Sprite keyIconSprite;
    [Tooltip("فونت پیکسلی TMP (اختیاری - اگه خالی بمونه فونت پیش‌فرض استفاده می‌شه)")]
    public TMP_FontAsset pixelFont;
    public Color textColor = Color.white;

    [Header("Events")]
    [Tooltip("وقتی همه‌ی کلیدها جمع شدن این رویداد صدا زده می‌شه - مثلا برای باز کردن در مرحله بعد وصلش کن")]
    public UnityEvent onAllKeysCollected;

    private int collected = 0;
    private TextMeshProUGUI counterText;

    void Awake()
    {
        BuildUI();
    }

    // این متد رو KeyPickup صدا می‌زنه وقتی بازیکن یه کلید برمی‌داره
    public void CollectKey()
    {
        collected = Mathf.Min(collected + 1, totalKeys);
        UpdateCounterText();

        if (collected >= totalKeys)
        {
            onAllKeysCollected?.Invoke();
        }
    }

    void UpdateCounterText()
    {
        if (counterText != null)
        {
            counterText.text = collected + " / " + totalKeys;
        }
    }

    void BuildUI()
    {
        GameObject canvasObj = new GameObject("KeyHUD_Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        // یک کانتینر بالا-راست
        GameObject container = new GameObject("KeyContainer");
        container.transform.SetParent(canvasObj.transform, false);
        RectTransform containerRt = container.AddComponent<RectTransform>();
        containerRt.anchorMin = new Vector2(1f, 1f);
        containerRt.anchorMax = new Vector2(1f, 1f);
        containerRt.pivot = new Vector2(1f, 1f);
        containerRt.anchoredPosition = new Vector2(-40, -40);
        containerRt.sizeDelta = new Vector2(260, 70);

        // آیکون کلید
        GameObject iconObj = new GameObject("KeyIcon");
        iconObj.transform.SetParent(container.transform, false);
        RectTransform iconRt = iconObj.AddComponent<RectTransform>();
        iconRt.anchorMin = new Vector2(0f, 0.5f);
        iconRt.anchorMax = new Vector2(0f, 0.5f);
        iconRt.pivot = new Vector2(0f, 0.5f);
        iconRt.sizeDelta = new Vector2(56, 56);
        iconRt.anchoredPosition = Vector2.zero;

        Image iconImg = iconObj.AddComponent<Image>();
        iconImg.sprite = keyIconSprite;
        iconImg.preserveAspect = true;

        // متن شمارنده
        GameObject textObj = new GameObject("KeyCountText");
        textObj.transform.SetParent(container.transform, false);
        RectTransform textRt = textObj.AddComponent<RectTransform>();
        textRt.anchorMin = new Vector2(0f, 0.5f);
        textRt.anchorMax = new Vector2(0f, 0.5f);
        textRt.pivot = new Vector2(0f, 0.5f);
        textRt.sizeDelta = new Vector2(170, 60);
        textRt.anchoredPosition = new Vector2(66, 0);

        counterText = textObj.AddComponent<TextMeshProUGUI>();
        if (pixelFont != null) counterText.font = pixelFont;
        counterText.fontSize = 34;
        counterText.enableAutoSizing = true;
        counterText.fontSizeMin = 18;
        counterText.fontSizeMax = 34;
        counterText.enableWordWrapping = false;
        counterText.overflowMode = TextOverflowModes.Overflow;
        counterText.alignment = TextAlignmentOptions.Left;
        counterText.color = textColor;

        UpdateCounterText();
    }
}