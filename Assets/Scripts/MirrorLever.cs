using UnityEngine;

// این اسکریپت رو روی یک Trigger (اهرم/دکمه) بچسبون
// نیاز به Box Collider 2D با تیک "Is Trigger" فعال
public class MirrorLever : MonoBehaviour
{
    [Tooltip("فقط این لایه بتونه اهرم رو فعال کنه (معمولا Player)")]
    public LayerMask activatorLayer;

    private WorldMirrorManager mirrorManager;
    private SpriteRenderer spriteRenderer;
    public Color activeColor = Color.cyan;
    public Color inactiveColor = Color.gray;

    void Start()
    {
        mirrorManager = FindObjectOfType<WorldMirrorManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & activatorLayer) != 0)
        {
            if (mirrorManager != null)
            {
                mirrorManager.ToggleMirror();
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = mirrorManager.isMirrored ? activeColor : inactiveColor;
                }
            }
        }
    }
}