using UnityEngine;

// این اسکریپت رو روی خود آبجکت Shadow بچسبون
[RequireComponent(typeof(Rigidbody2D))]
public class ShadowThrow : MonoBehaviour
{
    [Header("Throw Settings")]
    [Tooltip("حداکثر فاصله‌ای که موس رو می‌تونی از سایه دور کنی (برای محدود کردن قدرت)")]
    public float maxAimDistance = 4f;
    [Tooltip("هرچقدر بیشتر باشه، پرتاب قوی‌تر می‌شه")]
    public float powerMultiplier = 3f;

    private Rigidbody2D rb;
    private PlayerController playerController;
    private LineRenderer aimLine;
    private bool isAiming = false;
    private Vector2 aimDirection;
    private float aimPower;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();

        // اضافه کردن خودکار LineRenderer برای نمایش خط نشونه‌گیری
        aimLine = GetComponent<LineRenderer>();
        if (aimLine == null)
        {
            aimLine = gameObject.AddComponent<LineRenderer>();
            aimLine.startWidth = 0.08f;
            aimLine.endWidth = 0.02f;
            aimLine.material = new Material(Shader.Find("Sprites/Default"));
            aimLine.startColor = Color.white;
            aimLine.endColor = new Color(1f, 1f, 1f, 0.2f);
            aimLine.sortingOrder = 10;
        }
        aimLine.positionCount = 2;
        aimLine.enabled = false;
    }

    void Update()
    {
        // فقط وقتی این کاراکتر فعال و قابل‌کنترله، اجازه‌ی پرتاب بده
        bool isControllable = (playerController != null && playerController.enabled);
        if (!isControllable)
        {
            if (isAiming) StopAiming();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            isAiming = true;
            aimLine.enabled = true;
        }

        if (isAiming && Input.GetMouseButton(0))
        {
            UpdateAim();
        }

        if (isAiming && Input.GetMouseButtonUp(0))
        {
            Launch();
        }
    }

    void UpdateAim()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 rawDirection = mouseWorldPos - (Vector2)transform.position;

        float distance = Mathf.Min(rawDirection.magnitude, maxAimDistance);
        aimDirection = rawDirection.normalized;
        aimPower = distance * powerMultiplier;

        Vector2 lineEnd = (Vector2)transform.position + aimDirection * distance;
        aimLine.SetPosition(0, transform.position);
        aimLine.SetPosition(1, lineEnd);
    }

    void Launch()
    {
        rb.velocity = aimDirection * aimPower;
        StopAiming();
        StartCoroutine(DisableControlBriefly());
    }

    System.Collections.IEnumerator DisableControlBriefly()
    {
        if (playerController != null)
        {
            playerController.enabled = false;
            yield return new WaitForSeconds(0.4f);
            playerController.enabled = true;
        }
    }

    void StopAiming()
    {
        isAiming = false;
        aimLine.enabled = false;
    }
}