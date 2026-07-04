using UnityEngine;

// این اسکریپت رو روی Main Camera بچسبون
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;              // آبجکتی که دوربین دنبالش می‌کنه (Aria)

    [Header("Follow Settings")]
    public float smoothSpeed = 5f;        // هر چه بیشتر، دوربین سریع‌تر می‌رسه به هدف
    public Vector3 offset = new Vector3(0f, 1f, -10f); // فاصله‌ی دوربین از هدف (Z باید منفی بمونه چون دوربین باید عقب‌تر از صحنه باشه)

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}