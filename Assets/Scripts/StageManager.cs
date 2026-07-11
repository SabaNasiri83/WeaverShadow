using UnityEngine;
using System.Collections;
using TMPro; // برای TextMeshPro

public class StageManager : MonoBehaviour
{
    public int currentStage = 1;
    public int keysNeededForStage2 = 5;
    public int keysCollected = 0;

    public TextMeshProUGUI messageText; // Drag & Drop از UI

    [Header(" timing settings")]
    public float stageTextDuration = 2f; // مدت نمایش متن استیج
    public float instructionDuration = 4f; // مدت نمایش دستورالعمل
    public float fadeDuration = 1f; // مدت محو شدن هر کدوم

    public GameObject doorForNextStage; // درِ مرحله بعد

    void Start()
    {
        // وقتی بازی شروع شد، مرحله ۱ فعاله
        messageText.gameObject.SetActive(false);
        if (doorForNextStage != null)
            doorForNextStage.SetActive(false);
    }

    // این تابع هر بار که کلید جمع می‌کنی صدا بزن
    public void CollectKey()
    {
        if (currentStage != 2) return; // فقط مرحله ۲ کلید جمع کن

        keysCollected++;
        Debug.Log("Key collected: " + keysCollected + " / " + keysNeededForStage2);

        if (keysCollected >= keysNeededForStage2)
        {
            OpenNextStage();
        }
    }

    // وقتی وارد مرحله ۲ میشی این رو صدا بزن
    public void EnterStage2()
    {
        currentStage = 2;
        keysCollected = 0;

        // شروع نمایش پیام‌های پشت سر هم
        StartCoroutine(ShowStageMessages());
    }

    IEnumerator ShowStageMessages()
    {
        // ========== پیام اول: استیج ۲ ==========
        ShowMessage("Stage 2");

        // صبر کن تا متن استیج نمایش داده بشه و محو بشه
        yield return StartCoroutine(DisplayAndFade(stageTextDuration));

        // ========== پیام دوم: دستورالعمل ==========
        ShowMessage("You need to find 5 keys to unlock the next door!");

        // صبر کن تا دستورالعمل نمایش داده بشه و محو بشه
        yield return StartCoroutine(DisplayAndFade(instructionDuration));

        // هر کاری که بعد از نمایش هر دو پیام میخوای انجام بدی
        Debug.Log("Both messages displayed!");
    }

    // این تابع متن رو نشون میده، مدت مشخصی نگهش میداره، بعد محو میکنه
    IEnumerator DisplayAndFade(float displayDuration)
    {
        // مطمئن بشیم متن فعال و قابل مشاهده هست
        messageText.gameObject.SetActive(true);

        // تنظیم شفافیت به ۱ (کاملاً قابل مشاهده)
        CanvasGroup canvasGroup = messageText.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = messageText.gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 1f;

        // به مدت displayDuration صبر کن
        yield return new WaitForSeconds(displayDuration);

        // حالا شروع به محو شدن کن
        float t = 0f;
        float startAlpha = canvasGroup.alpha;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t / fadeDuration);
            yield return null;
        }

        // بعد از محو شدن کامل، متن رو مخفی کن
        canvasGroup.alpha = 0f;
        messageText.gameObject.SetActive(false);
    }

    void ShowMessage(string msg)
    {
        messageText.text = msg;
        // فعال کردنش توی تابع DisplayAndFade انجام میشه
    }

    void OpenNextStage()
    {
        // نمایش پیام باز شدن در
        StartCoroutine(ShowFinalMessage());
    }

    IEnumerator ShowFinalMessage()
    {
        ShowMessage("Well done! The door is now open.");
        yield return StartCoroutine(DisplayAndFade(3f)); // ۳ ثانیه نمایش

        if (doorForNextStage != null)
            doorForNextStage.SetActive(true);
    }
}