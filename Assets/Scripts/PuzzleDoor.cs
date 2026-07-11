using UnityEngine;
using System.Collections;

// این اسکریپت رو روی خود آبجکت "در" (Door) بچسبون
public class PuzzleDoor : MonoBehaviour
{
    [Header("Required Plates")]
    [Tooltip("هر دو کلید باید همزمان فعال باشن تا در باز شه")]
    public PressurePlate plateA; // مثلا کلیدی که فقط آریا فعالش می‌کنه
    public PressurePlate plateB; // مثلا کلیدی که فقط سایه فعالش می‌کنه

    [Header("Behavior")]
    [Tooltip("اگه true باشه، بعد از یک‌بار باز شدن، در دیگه هیچ‌وقت بسته نمی‌شه حتی اگه کاراکترها از روی کلیدها کنار برن")]
    public bool stayOpenPermanently = true;

    [Header("Sprites")]
    public Sprite closedSprite;
    public Sprite openSprite;

    [Header("Key Reward")]
    [Tooltip("پریفب کلیدی که وقتی در باز شد ازش بیرون میاد (باید اسکریپت KeyPickup روش باشه)")]
    public GameObject keyPrefab;
    [Tooltip("موقعیت شروع کلید نسبت به مرکز در (معمولا وسط در، پایین‌تر از سقفش)")]
    public Vector3 keySpawnLocalOffset = Vector3.zero;
    [Tooltip("موقعیت نهایی کلید بعد از بیرون اومدن از در (نسبت به مرکز در)")]
    public Vector3 keyExitLocalOffset = new Vector3(0f, -0.8f, 0f);
    [Tooltip("مدت زمان بیرون اومدن کلید از در (ثانیه)")]
    public float keyPopDuration = 1.1f;

    private bool keySpawned = false;

    [Header("Transition")]
    [Tooltip("مدت زمان فِید بین دو حالت (ثانیه)")]
    public float fadeDuration = 0.25f;

    private BoxCollider2D doorCollider;
    private SpriteRenderer spriteRenderer;
    private bool isOpen = false;
    private Coroutine fadeCoroutine;

    void Start()
    {
        doorCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null && closedSprite != null)
        {
            spriteRenderer.sprite = closedSprite;
        }
    }

    void Update()
    {
        if (isOpen && stayOpenPermanently) return; // یک‌بار باز شد، دیگه هیچ‌وقت چک نمی‌کنیم که ببندیمش

        bool bothPressed = plateA.isPressed && plateB.isPressed;

        if (bothPressed && !isOpen)
        {
            OpenDoor();
        }
        else if (!bothPressed && isOpen)
        {
            CloseDoor();
        }
    }

    void OpenDoor()
    {
        isOpen = true;
        if (doorCollider != null) doorCollider.enabled = false; // دیگه مانع عبور نیست
        SwapSprite(openSprite);
        SpawnKey();
    }

    void CloseDoor()
    {
        isOpen = false;
        if (doorCollider != null) doorCollider.enabled = true;
        SwapSprite(closedSprite);
    }

    // فقط یک‌بار (اولین باری که در باز می‌شه) یه کلید از توی در می‌سازه
    // و با یه حرکت نرم بیرونش می‌کشه
    void SpawnKey()
    {
        if (keySpawned || keyPrefab == null) return;
        keySpawned = true;

        Vector3 startPos = transform.position + keySpawnLocalOffset;
        Vector3 endPos = transform.position + keyExitLocalOffset;

        GameObject keyObj = Instantiate(keyPrefab, startPos, Quaternion.identity);

        // مطمئن می‌شیم کلید همیشه جلوتر از خود در دیده بشه، نه پشتش
        SpriteRenderer keySr = keyObj.GetComponent<SpriteRenderer>();
        if (keySr != null && spriteRenderer != null)
        {
            keySr.sortingLayerID = spriteRenderer.sortingLayerID;
            keySr.sortingOrder = spriteRenderer.sortingOrder + 1;
        }

        StartCoroutine(PopKeyOut(keyObj.transform, startPos, endPos));
    }

    IEnumerator PopKeyOut(Transform keyTransform, Vector3 startPos, Vector3 endPos)
    {
        float t = 0f;
        while (t < keyPopDuration)
        {
            // اگه بازیکن قبل از تموم شدن انیمیشن، کلید رو برداشت (Destroy شد)، ادامه نده
            if (keyTransform == null) yield break;

            t += Time.deltaTime;
            float progress = t / keyPopDuration;
            float eased = Mathf.SmoothStep(0f, 1f, progress); // شروع و پایان نرم، نه یکنواخت
            keyTransform.position = Vector3.Lerp(startPos, endPos, eased);
            yield return null;
        }

        if (keyTransform != null) keyTransform.position = endPos;
    }

    void SwapSprite(Sprite target)
    {
        if (spriteRenderer == null || target == null) return;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeSwap(target));
    }

    // یه فِید کوتاه به شفافیت صفر، عوض کردن اسپرایت، بعد فِید برگشت به حالت کامل
    // این‌جوری تعویض عکس ناگهانی و چشم‌آزار نمی‌شه
    IEnumerator FadeSwap(Sprite target)
    {
        float half = fadeDuration / 2f;
        Color c = spriteRenderer.color;
        float t = 0f;

        while (t < half)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / half);
            spriteRenderer.color = c;
            yield return null;
        }

        spriteRenderer.sprite = target;

        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / half);
            spriteRenderer.color = c;
            yield return null;
        }

        c.a = 1f;
        spriteRenderer.color = c;
    }
}