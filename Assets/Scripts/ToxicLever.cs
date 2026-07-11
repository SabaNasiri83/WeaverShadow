using UnityEngine;
using System.Collections;

// این اسکریپت رو روی آبجکت اهرمِ کنار لوله‌ی مواد سمی بچسبون
// نیاز به Box Collider 2D با تیک "Is Trigger" فعال روی همین اهرم
public class ToxicLever : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("فقط این لایه می‌تونه اهرم رو فعال کنه - فقط لایه‌ی Shadow رو انتخاب کن")]
    public LayerMask activatorLayer;

    [Header("Toxic Flow")]
    [Tooltip("جریان(های) موادی که باید وقتی سایه کنار اهرمه، متوقف بشن")]
    public ToxicFlowAnimator[] flowsToStop;

    [Header("Hazard Zones")]
    [Tooltip("منطقه(های) خطرِ روی زمین که باید غیرفعال بشن تا آریا بدون آسیب رد شه")]
    public HazardZone[] hazardZonesToDisable;

    [Header("Bridge")]
    [Tooltip("آبجکت پل/لایه (bridgeB) که روی مواد سمی ظاهر می‌شه - باید از قبل توی صحنه باشه و از اول خاموش باشه")]
    public GameObject bridgeObject;

    [Header("Pull Animation (اختیاری)")]
    [Tooltip("فریم‌ها به‌ترتیب: leverLeft -> leverMid -> leverRight")]
    public Sprite[] pullFrames;
    [Tooltip("هر فریم چقدر طول بکشه (ثانیه)")]
    public float frameDuration = 0.06f;

    private SpriteRenderer spriteRenderer;
    private Coroutine animCoroutine;
    private bool isActive = false;
    private GameObject currentActivator = null; // فقط همین آبجکت می‌تونه بعداً غیرفعالش کنه

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null && pullFrames != null && pullFrames.Length > 0)
        {
            spriteRenderer.sprite = pullFrames[0];
        }

        // مطمئن می‌شیم پل از اول خاموشه (هر چی هم توی صحنه ست شده باشه)
        if (bridgeObject != null) bridgeObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // اگه از قبل یه نفر (آریا یا سایه) اهرم رو نگه داشته، اون یکی هیچ تاثیری روش نداره
        if (isActive) return;
        if (((1 << other.gameObject.layer) & activatorLayer) == 0) return;

        isActive = true;
        currentActivator = other.gameObject;
        Activate();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!isActive) return;
        // فقط همون کاراکتری که اهرم رو فعال کرده می‌تونه غیرفعالش کنه
        // اگه کاراکتر دیگه از کنارش رد بشه و بره بیرون، تاثیری نداره
        if (other.gameObject != currentActivator) return;

        isActive = false;
        currentActivator = null;
        Deactivate();
    }

    // سایه کنار اهرم ایستاده: مواد قطع، هزارد خاموش، پل روشن
    void Activate()
    {
        if (flowsToStop != null)
        {
            foreach (var flow in flowsToStop)
                if (flow != null) flow.StopFlow();
        }

        if (hazardZonesToDisable != null)
        {
            foreach (var zone in hazardZonesToDisable)
                if (zone != null) zone.enabled = false;
        }

        if (bridgeObject != null) bridgeObject.SetActive(true);

        PlayAnimation(forward: true);
    }

    // سایه از کنار اهرم دور شد: فوراً همه‌چیز برمی‌گرده به حالت اول
    void Deactivate()
    {
        if (flowsToStop != null)
        {
            foreach (var flow in flowsToStop)
                if (flow != null) flow.StartFlow();
        }

        if (hazardZonesToDisable != null)
        {
            foreach (var zone in hazardZonesToDisable)
                if (zone != null) zone.enabled = true;
        }

        if (bridgeObject != null) bridgeObject.SetActive(false);

        PlayAnimation(forward: false);
    }

    void PlayAnimation(bool forward)
    {
        if (spriteRenderer == null || pullFrames == null || pullFrames.Length < 2) return;

        if (animCoroutine != null) StopCoroutine(animCoroutine);
        animCoroutine = StartCoroutine(PlayFrames(forward));
    }

    IEnumerator PlayFrames(bool forward)
    {
        if (forward)
        {
            for (int i = 1; i < pullFrames.Length; i++)
            {
                spriteRenderer.sprite = pullFrames[i];
                yield return new WaitForSeconds(frameDuration);
            }
        }
        else
        {
            for (int i = pullFrames.Length - 2; i >= 0; i--)
            {
                spriteRenderer.sprite = pullFrames[i];
                yield return new WaitForSeconds(frameDuration);
            }
        }
    }
}