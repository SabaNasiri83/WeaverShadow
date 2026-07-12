using UnityEngine;
using System.Collections;

// این اسکریپت رو روی آبجکت درِ خروجی بچسبون
public class ExitDoorController : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite closedSprite;
    public Sprite openSprite;

    [Header("Camera Pan")]
    [Tooltip("اسکریپت CameraFollow که روی دوربین اصلیه")]
    public CameraFollow cameraFollow;
    [Tooltip("چقدر دوربین روی در بمونه قبل از اینکه خودش برگرده سمت بازیکن")]
    public float panHoldDuration = 2f;

    [Header("Message")]
    public string openMessage = "The Door Is Open!";

    [Header("Sound (اختیاری)")]
    [Tooltip("صدای باز شدن در - پیشنهاد: impactMetal_heavy یا impactPlate_heavy")]
    public RandomSFX openSfx;

    private BoxCollider2D doorCollider;
    private SpriteRenderer spriteRenderer;
    private bool isOpen = false;

    void Awake()
    {
        doorCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null && closedSprite != null)
            spriteRenderer.sprite = closedSprite;
    }

    // این متد رو از KeyManager -> onAllKeysCollected صدا بزن
    public void OpenDoor()
    {
        if (isOpen) return;
        isOpen = true;

        if (doorCollider != null) doorCollider.enabled = false;
        if (spriteRenderer != null && openSprite != null) spriteRenderer.sprite = openSprite;
        if (openSfx != null) openSfx.Play();

        if (MessageBanner.Instance != null) MessageBanner.Instance.Show(openMessage);

        if (cameraFollow != null) StartCoroutine(PanToDoorAndBack());
    }

    IEnumerator PanToDoorAndBack()
    {
        Transform originalTarget = cameraFollow.target;
        cameraFollow.target = transform;

        yield return new WaitForSeconds(panHoldDuration);

        // دوربین خودش با همون Smooth Speed داخلی نرم برمی‌گرده سمت بازیکن
        cameraFollow.target = originalTarget;
    }
}