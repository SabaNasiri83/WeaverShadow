using UnityEngine;

// این اسکریپت رو روی یک Trigger (کلید فشاری) بچسبون
// نیاز به Box Collider 2D با تیک "Is Trigger" فعال
public class PressurePlate : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("این کلید فقط با کدوم لایه فعال بشه؟ مثلا فقط Player یا فقط Shadow")]
    public LayerMask activatorLayer;

    [Header("Status (Read Only)")]
    public bool isPressed = false;

    private SpriteRenderer spriteRenderer;
    public Color pressedColor = Color.green;
    public Color unpressedColor = Color.red;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisual();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // چک می‌کنه که آبجکتی که وارد شده، توی لایه‌ی مجاز هست یا نه
        if (((1 << other.gameObject.layer) & activatorLayer) != 0)
        {
            isPressed = true;
            UpdateVisual();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & activatorLayer) != 0)
        {
            isPressed = false;
            UpdateVisual();
        }
    }

    void UpdateVisual()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = isPressed ? pressedColor : unpressedColor;
        }
    }
}