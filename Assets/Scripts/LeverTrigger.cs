using UnityEngine;
using System.Collections;

// این اسکریپت رو روی آبجکت اهرم بچسبون
// نیاز به Box Collider 2D با تیک "Is Trigger" فعال
public class LeverTrigger : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("فقط این لایه می‌تونه اهرم رو فعال کنه - اینجا فقط لایه‌ی Shadow رو انتخاب کن")]
    public LayerMask activatorLayer;

    [Header("Target")]
    public GateWall gateToLower;

    [Header("Pull Animation (اختیاری)")]
    [Tooltip("فریم‌ها به‌ترتیب از حالت اولیه (سمت دیوار) تا حالت کشیده‌شده (برعکس دیوار)")]
    public Sprite[] pullFrames;
    [Tooltip("هر فریم چقدر طول بکشه (ثانیه)")]
    public float frameDuration = 0.06f;

    private SpriteRenderer spriteRenderer;
    private Coroutine animCoroutine;
    private bool isActive = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // فریم اول (سمت دیوار / حالت استراحت) رو از ابتدا نشون بده
        if (spriteRenderer != null && pullFrames != null && pullFrames.Length > 0)
        {
            spriteRenderer.sprite = pullFrames[0];
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isActive) return;
        if (((1 << other.gameObject.layer) & activatorLayer) == 0) return;

        isActive = true;
        if (gateToLower != null) gateToLower.Lower();
        PlayAnimation(forward: true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!isActive) return;
        if (((1 << other.gameObject.layer) & activatorLayer) == 0) return;

        isActive = false;
        if (gateToLower != null) gateToLower.Raise();
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
            // از فریم دوم تا آخر (سمت دیوار -> برعکس دیوار)
            for (int i = 1; i < pullFrames.Length; i++)
            {
                spriteRenderer.sprite = pullFrames[i];
                yield return new WaitForSeconds(frameDuration);
            }
        }
        else
        {
            // از فریم ماقبل‌آخر تا اول (برعکس دیوار -> سمت دیوار)
            for (int i = pullFrames.Length - 2; i >= 0; i--)
            {
                spriteRenderer.sprite = pullFrames[i];
                yield return new WaitForSeconds(frameDuration);
            }
        }
    }
}