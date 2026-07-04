using UnityEngine;

// این اسکریپت رو روی یک GameObject خالی جدید به اسم "GameManager" بچسبون
// (نه روی Aria یا Shadow - یک آبجکت مستقل بساز)
public class ShadowSwitcher : MonoBehaviour
{
    [Header("Characters")]
    public GameObject aria;
    public GameObject shadow;

    [Header("Camera")]
    public CameraFollow cameraFollow; // اسکریپت دوربینی که قبلاً ساختیم

    private bool controllingShadow = false;

    void Start()
    {
        // در ابتدای بازی، آریا فعاله و سایه ثابت و غیرفعاله
        SetActiveCharacter(aria, shadow);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            controllingShadow = !controllingShadow;

            if (controllingShadow)
                SetActiveCharacter(shadow, aria);
            else
                SetActiveCharacter(aria, shadow);
        }
    }

    // characterToActivate: کاراکتری که از این به بعد کنترلش با بازیکنه
    // characterToFreeze: کاراکتری که باید ثابت و بی‌حرکت بمونه
    void SetActiveCharacter(GameObject characterToActivate, GameObject characterToFreeze)
    {
        // فعال‌سازی کنترل و فیزیک روی کاراکتر انتخاب‌شده
        PlayerController activeScript = characterToActivate.GetComponent<PlayerController>();
        Rigidbody2D activeRb = characterToActivate.GetComponent<Rigidbody2D>();
        if (activeScript != null) activeScript.enabled = true;
        if (activeRb != null) activeRb.bodyType = RigidbodyType2D.Dynamic;

        // منجمد کردن کامل کاراکتر دیگر (بدون فیزیک، بدون کنترل - دقیقا سر جاش می‌ماند)
        PlayerController frozenScript = characterToFreeze.GetComponent<PlayerController>();
        Rigidbody2D frozenRb = characterToFreeze.GetComponent<Rigidbody2D>();
        if (frozenScript != null) frozenScript.enabled = false;
        if (frozenRb != null)
        {
            frozenRb.velocity = Vector2.zero; // متوقف کردن کامل حرکت قبل از منجمد شدن
            frozenRb.bodyType = RigidbodyType2D.Kinematic; // یعنی دیگر تحت تاثیر گرانش و فیزیک نیست
        }

        // دوربین را به کاراکتر فعال متصل کن
        if (cameraFollow != null)
        {
            cameraFollow.target = characterToActivate.transform;
        }
    }
}