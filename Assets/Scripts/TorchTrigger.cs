using UnityEngine;
using UnityEngine.Events;
using System.Collections;

// این اسکریپت رو روی آبجکت مشعل بچسبون
// نیاز به Box Collider2D با تیک "Is Trigger" فعال، کمی بزرگ‌تر از خودِ مشعل
public class TorchTrigger : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("فقط این لایه می‌تونه مشعل رو روشن کنه (معمولا Player/Aria)")]
    public LayerMask activatorLayer;

    [Header("Sprites")]
    [Tooltip("عکس مشعل خاموش (torchOff)")]
    public Sprite offSprite;
    [Tooltip("فریم‌های لرزش شعله وقتی روشنه (torch1, torch2)")]
    public Sprite[] flameFrames;

    [Header("Flicker Timing")]
    public float minFrameDuration = 0.08f;
    public float maxFrameDuration = 0.18f;

    [Header("Events")]
    [Tooltip("وقتی مشعل روشن می‌شه این رویداد صدا زده می‌شه - باس رو به این وصل کن")]
    public UnityEvent onLit;

    [Header("Message")]
    [Tooltip("متنی که همون لحظه‌ی روشن شدن، روی صفحه نشون داده می‌شه")]
    public string bossFightMessage = "Boss Fight!";

    private SpriteRenderer spriteRenderer;
    private bool isLit = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && offSprite != null)
            spriteRenderer.sprite = offSprite;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isLit) return;
        if (((1 << other.gameObject.layer) & activatorLayer) == 0) return;

        isLit = true;
        StartCoroutine(FlickerLoop());

        if (MessageBanner.Instance != null) MessageBanner.Instance.Show(bossFightMessage);
        onLit?.Invoke();
    }

    IEnumerator FlickerLoop()
    {
        if (spriteRenderer == null || flameFrames == null || flameFrames.Length == 0) yield break;

        int index = 0;
        while (true)
        {
            spriteRenderer.sprite = flameFrames[index];
            index = (index + 1) % flameFrames.Length;
            yield return new WaitForSeconds(Random.Range(minFrameDuration, maxFrameDuration));
        }
    }
}