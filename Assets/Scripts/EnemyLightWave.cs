using UnityEngine;
using System.Collections;

// این اسکریپت رو کنار EnemyPatrol روی خود آبجکت دشمن بچسبون
// هر چند ثانیه یه هاله‌ی نور از دشمن ساخته می‌شه که به سمت لبه‌ی نقشه (چپ یا راست) حرکت می‌کنه،
// اونجا یه مدت می‌مونه، بعد محو می‌شه و پاک می‌شه
// نکته: این هاله دقیقا مثل نورافکن دشمن، فقط با "سایه" برخورد می‌کنه (نه آریا) و فقط ریست می‌کنه، جون کم نمی‌کنه
public class EnemyLightWave : MonoBehaviour
{
    public enum WaveDirection { Left, Right, Alternate }

    [Header("Timing")]
    [Tooltip("هر چند ثانیه یه هاله‌ی جدید ساخته بشه")]
    public float interval = 10f;
    [Tooltip("چقدر طول بکشه تا هاله از دشمن به لبه‌ی نقشه برسه")]
    public float travelDuration = 1.5f;
    [Tooltip("چقدر بعد از رسیدن به لبه، همون‌جا بمونه قبل از محو شدن")]
    public float holdDuration = 5f;
    [Tooltip("چقدر طول بکشه تا کامل محو بشه")]
    public float fadeOutDuration = 0.6f;
    [Tooltip("چقدر طول بکشه تا هاله اول ظاهر بشه (فید ورود)")]
    public float fadeInDuration = 0.2f;

    [Header("Direction")]
    public WaveDirection direction = WaveDirection.Alternate;
    [Tooltip("یه GameObject خالی دقیقا روی لبه‌ی چپ نقشه بذار و اینجا وصل کن")]
    public Transform leftEdge;
    [Tooltip("یه GameObject خالی دقیقا روی لبه‌ی راست نقشه بذار و اینجا وصل کن")]
    public Transform rightEdge;

    [Header("Look")]
    public float haloRadius = 1.5f;
    public Color haloColor = new Color(1f, 0.95f, 0.6f, 0.65f);
    public int sortingOrder = 10;

    [Header("Shadow Collision")]
    [Tooltip("لایه‌ی سایه - اگه سایه توی این هاله بیفته، به چک‌پوینت برمی‌گرده")]
    public LayerMask shadowLayer;
    [Tooltip("اگه خالی بمونه، خودش توی صحنه دنبالش می‌گرده")]
    public CheckpointManager checkpointManager;

    private bool goingRight = true;

    void Start()
    {
        if (checkpointManager == null) checkpointManager = FindObjectOfType<CheckpointManager>();
        StartCoroutine(WaveLoop());
    }

    IEnumerator WaveLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            yield return StartCoroutine(EmitWave());
        }
    }

    IEnumerator EmitWave()
    {
        Transform target = PickTargetEdge();
        if (target == null) yield break;

        GameObject halo = CreateHaloObject();
        Transform haloT = halo.transform;
        SpriteRenderer sr = halo.GetComponent<SpriteRenderer>();

        Vector3 startPos = transform.position;
        Vector3 endPos = new Vector3(target.position.x, startPos.y, startPos.z);

        yield return Fade(sr, 0f, haloColor.a, fadeInDuration);

        float t = 0f;
        while (t < travelDuration)
        {
            t += Time.deltaTime;
            haloT.position = Vector3.Lerp(startPos, endPos, t / travelDuration);
            yield return null;
        }
        haloT.position = endPos;

        yield return new WaitForSeconds(holdDuration);

        yield return Fade(sr, sr.color.a, 0f, fadeOutDuration);

        Destroy(halo);
    }

    Transform PickTargetEdge()
    {
        bool useRight;
        switch (direction)
        {
            case WaveDirection.Left:
                useRight = false;
                break;
            case WaveDirection.Right:
                useRight = true;
                break;
            default: // Alternate
                useRight = goingRight;
                goingRight = !goingRight;
                break;
        }

        Transform target = useRight ? rightEdge : leftEdge;
        if (target == null)
        {
            Debug.LogWarning("EnemyLightWave: لبه‌ی " + (useRight ? "راست" : "چپ") + " نقشه (Left/Right Edge) ست نشده!");
        }
        return target;
    }

    GameObject CreateHaloObject()
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

        Sprite haloSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size / (2f * haloRadius));

        GameObject obj = new GameObject("EnemyLightHalo");
        obj.transform.position = transform.position;

        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = haloSprite;
        sr.color = new Color(haloColor.r, haloColor.g, haloColor.b, 0f);
        sr.sortingOrder = sortingOrder;

        CircleCollider2D col = obj.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = haloRadius;

        HaloShadowDetector detector = obj.AddComponent<HaloShadowDetector>();
        detector.shadowLayer = shadowLayer;
        detector.checkpointManager = checkpointManager;

        return obj;
    }

    IEnumerator Fade(SpriteRenderer sr, float from, float to, float duration)
    {
        float t = 0f;
        Color c = sr.color;
        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / duration);
            sr.color = c;
            yield return null;
        }
        c.a = to;
        sr.color = c;
    }
}