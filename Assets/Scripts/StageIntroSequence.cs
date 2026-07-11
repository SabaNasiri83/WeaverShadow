using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

// این اسکریپت رو روی یک GameObject خالی جدید به اسم "StageIntro" بچسبون
// (توصیه: همون اول Hierarchy مرحله، کنار GameManager)
// خودش یک Canvas و دو تا متن TextMeshPro می‌سازه، نیازی به ساخت دستی UI نیست
public class StageIntroSequence : MonoBehaviour
{
    [Header("Texts")]
    [Tooltip("متن اول - اسم مرحله")]
    public string titleText = "STAGE 2";

    [Tooltip("متن دوم - هدف مرحله (انگلیسی)")]
    [TextArea]
    public string subtitleText = "FIND 5 KEYS TO UNLOCK THE DOOR TO THE NEXT STAGE";

    [Header("Pixel Font")]
    [Tooltip("فونت پیکسلی TMP رو اینجا بکش. اگه خالی بمونه از فونت پیش‌فرض استفاده می‌شه")]
    public TMP_FontAsset pixelFont;

    [Header("Timing")]
    public float fadeDuration = 0.6f;
    public float titleHoldDuration = 1.3f;
    public float gapBetweenTexts = 0.3f;
    public float subtitleHoldDuration = 2.2f;

    [Header("Style")]
    public float titleFontSize = 90f;
    public float subtitleFontSize = 36f;
    public Color textColor = Color.white;

    [Header("Position")]
    [Tooltip("هر چقدر این عدد بزرگتر باشه، هر دو متن با هم بیشتر میرن سمت بالای صفحه")]
    public float groupVerticalOffset = 200f;

    private Canvas canvas;
    private TextMeshProUGUI titleTMP;
    private TextMeshProUGUI subtitleTMP;

    void Awake()
    {
        BuildUI();
    }

    void Start()
    {
        StartCoroutine(PlaySequence());
    }

    void BuildUI()
    {
        GameObject canvasObj = new GameObject("StageIntro_Canvas");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 200; // بالاتر از هر UI دیگه‌ای باشه

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        titleTMP = CreateText("TitleText", titleFontSize, new Vector2(0, 40 + groupVerticalOffset));
        titleTMP.text = titleText;

        subtitleTMP = CreateText("SubtitleText", subtitleFontSize, new Vector2(0, -40 + groupVerticalOffset));
        subtitleTMP.text = subtitleText;

        SetAlpha(titleTMP, 0f);
        SetAlpha(subtitleTMP, 0f);
    }

    TextMeshProUGUI CreateText(string name, float fontSize, Vector2 anchoredPos)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(canvas.transform, false);

        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(1600, 200);
        rt.anchoredPosition = anchoredPos;

        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        if (pixelFont != null) tmp.font = pixelFont;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = textColor;
        tmp.enableWordWrapping = false;

        return tmp;
    }

    IEnumerator PlaySequence()
    {
        // فید این تیتر مرحله
        yield return Fade(titleTMP, 0f, 1f, fadeDuration);
        yield return new WaitForSeconds(titleHoldDuration);
        yield return Fade(titleTMP, 1f, 0f, fadeDuration);

        yield return new WaitForSeconds(gapBetweenTexts);

        // فید این هدف مرحله
        yield return Fade(subtitleTMP, 0f, 1f, fadeDuration);
        yield return new WaitForSeconds(subtitleHoldDuration);
        yield return Fade(subtitleTMP, 1f, 0f, fadeDuration);

        // دیگه لازم نیست - کل کانواس رو حذف کن
        Destroy(canvas.gameObject);
    }

    IEnumerator Fade(TextMeshProUGUI tmp, float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            SetAlpha(tmp, Mathf.Lerp(from, to, t / duration));
            yield return null;
        }
        SetAlpha(tmp, to);
    }

    void SetAlpha(TextMeshProUGUI tmp, float alpha)
    {
        Color c = tmp.color;
        c.a = alpha;
        tmp.color = c;
    }
}