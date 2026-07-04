using UnityEngine;

// این اسکریپت رو روی خود آبجکت "در" (Door) بچسبون
public class PuzzleDoor : MonoBehaviour
{
    [Header("Required Plates")]
    [Tooltip("هر دو کلید باید همزمان فعال باشن تا در باز شه")]
    public PressurePlate plateA; // مثلا کلیدی که فقط آریا فعالش می‌کنه
    public PressurePlate plateB; // مثلا کلیدی که فقط سایه فعالش می‌کنه

    private BoxCollider2D doorCollider;
    private SpriteRenderer spriteRenderer;

    [Header("Visual Feedback")]
    public Color closedColor = new Color(0.5f, 0.3f, 0.8f); // بنفش
    public Color openColor = new Color(0.5f, 0.3f, 0.8f, 0.3f); // نیمه‌شفاف وقتی بازه

    void Start()
    {
        doorCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        bool bothPressed = plateA.isPressed && plateB.isPressed;

        if (bothPressed)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    void OpenDoor()
    {
        if (doorCollider != null) doorCollider.enabled = false; // دیگه مانع عبور نیست
        if (spriteRenderer != null) spriteRenderer.color = openColor;
    }

    void CloseDoor()
    {
        if (doorCollider != null) doorCollider.enabled = true;
        if (spriteRenderer != null) spriteRenderer.color = closedColor;
    }
}