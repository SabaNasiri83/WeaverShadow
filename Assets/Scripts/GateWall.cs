using UnityEngine;
using System.Collections;

// این اسکریپت رو روی خود آبجکت دیوار (LeverGate) بچسبون
// Layer این آبجکت باید همون "ShadowWall" باشه (که سایه ازش رد میشه ولی آریا نه)
public class GateWall : MonoBehaviour
{
    [Header("Lower Animation")]
    [Tooltip("دیوار چقدر باید پایین بره (معمولا برابر یا کمی بیشتر از ارتفاع خودش تا کامل زیر زمین قایم بشه)")]
    public float lowerDistance = 3f;
    public float moveDuration = 0.8f;

    private BoxCollider2D wallCollider;
    private Vector3 startPos;
    private Vector3 loweredPos;
    private bool isLowered = false;
    private Coroutine moveCoroutine;

    void Start()
    {
        wallCollider = GetComponent<BoxCollider2D>();
        startPos = transform.position;
        loweredPos = startPos + Vector3.down * lowerDistance;
    }

    // این متد رو LeverTrigger صدا می‌زنه وقتی سایه روی اهرم می‌ره
    public void Lower()
    {
        if (isLowered) return;
        isLowered = true;

        // بلافاصله کالایدر رو غیرفعال کن تا آریا هرلحظه بتونه رد بشه
        if (wallCollider != null) wallCollider.enabled = false;

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveTo(loweredPos, false));
    }

    // این متد رو LeverTrigger صدا می‌زنه وقتی سایه از روی اهرم کنار میره
    public void Raise()
    {
        if (!isLowered) return;
        isLowered = false;

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        // کالایدر رو فقط بعد از کامل بالا اومدن فعال می‌کنیم تا اگه کسی زیرش وایساده گیر نکنه
        moveCoroutine = StartCoroutine(MoveTo(startPos, true));
    }

    IEnumerator MoveTo(Vector3 target, bool enableColliderWhenDone)
    {
        Vector3 from = transform.position;
        float t = 0f;

        while (t < moveDuration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(from, target, t / moveDuration);
            yield return null;
        }

        transform.position = target;

        if (enableColliderWhenDone && wallCollider != null)
        {
            wallCollider.enabled = true;
        }
    }
}