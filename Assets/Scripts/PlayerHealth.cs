using UnityEngine;
using UnityEngine.UI;

// این اسکریپت رو روی همون آبجکت "GameManager" بچسبون
// خودش سه‌تا آیکون قلب پیکسلی می‌سازه (بدون نیاز به هیچ عکسی) بالا-سمت‌چپ صفحه
// نکته مهم: فقط وقتی LoseHeart() صدا زده بشه یه قلب کم می‌شه
// این متد رو فقط برای برخورد "آریا" صدا بزن، نه سایه
public class PlayerHealth : MonoBehaviour
{
    [Header("Settings")]
    public int maxHearts = 3;

    [Header("Colors")]
    public Color fullHeartColor = new Color(0.86f, 0.15f, 0.2f, 1f);
    public Color emptyHeartColor = new Color(0.25f, 0.25f, 0.28f, 0.9f);

    [Header("On Zero Hearts")]
    [Tooltip("اگه true باشه، وقتی جون تموم شد کل صحنه دوباره لود می‌شه")]
    public bool reloadSceneOnZeroHearts = true;
    public float reloadDelay = 1f;

    private int currentHearts;
    private Image[] heartImages;

    void Awake()
    {
        currentHearts = maxHearts;
        BuildUI();
    }

    // این متد رو فقط از برخورد آریا با خطر صدا بزن (مثلا از HazardZone)
    public void LoseHeart()
    {
        if (currentHearts <= 0) return;

        currentHearts--;
        heartImages[currentHearts].color = emptyHeartColor;

        if (currentHearts <= 0 && reloadSceneOnZeroHearts)
        {
            Invoke(nameof(ReloadScene), reloadDelay);
        }
    }

    public int GetCurrentHearts()
    {
        return currentHearts;
    }

    void ReloadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    void BuildUI()
    {
        GameObject canvasObj = new GameObject("HealthHUD_Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject container = new GameObject("HeartContainer");
        container.transform.SetParent(canvasObj.transform, false);
        RectTransform containerRt = container.AddComponent<RectTransform>();
        containerRt.anchorMin = new Vector2(0f, 1f);
        containerRt.anchorMax = new Vector2(0f, 1f);
        containerRt.pivot = new Vector2(0f, 1f);
        containerRt.anchoredPosition = new Vector2(40, -40);
        containerRt.sizeDelta = new Vector2(220, 56);

        Sprite heartSprite = BuildHeartSprite();
        heartImages = new Image[maxHearts];

        float spacing = 56f;
        for (int i = 0; i < maxHearts; i++)
        {
            GameObject heartObj = new GameObject("Heart_" + i);
            heartObj.transform.SetParent(container.transform, false);

            RectTransform rt = heartObj.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 0.5f);
            rt.anchorMax = new Vector2(0f, 0.5f);
            rt.pivot = new Vector2(0f, 0.5f);
            rt.sizeDelta = new Vector2(48, 48);
            rt.anchoredPosition = new Vector2(i * spacing, 0);

            Image img = heartObj.AddComponent<Image>();
            img.sprite = heartSprite;
            img.color = fullHeartColor;

            heartImages[i] = img;
        }
    }

    // یه اسپرایت قلب ۸در۸ پیکسلی می‌سازه (بدون نیاز به هیچ فایل عکسی)
    // FilterMode روی Point تا لبه‌هاش تیز و پیکسلی بمونه، نه محو
    Sprite BuildHeartSprite()
    {
        int[,] pattern = new int[8, 8]
        {
            { 0,1,1,0,0,1,1,0 },
            { 1,1,1,1,1,1,1,1 },
            { 1,1,1,1,1,1,1,1 },
            { 1,1,1,1,1,1,1,1 },
            { 0,1,1,1,1,1,1,0 },
            { 0,0,1,1,1,1,0,0 },
            { 0,0,0,1,1,0,0,0 },
            { 0,0,0,0,0,0,0,0 },
        };

        int size = 8;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                // یونیتی از پایین به بالا پیکسل می‌خونه، برای همین ردیف رو معکوس می‌کنیم
                int value = pattern[size - 1 - y, x];
                tex.SetPixel(x, y, value == 1 ? Color.white : new Color(0, 0, 0, 0));
            }
        }
        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
}
