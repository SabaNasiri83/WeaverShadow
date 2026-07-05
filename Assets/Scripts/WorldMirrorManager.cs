using UnityEngine;

// این اسکریپت رو روی همون آبجکت "GameManager" بچسبون
public class WorldMirrorManager : MonoBehaviour
{
    [Header("Light World Objects")]
    [Tooltip("اشیایی که فقط توی دنیای نور (حالت عادی) وجود دارن")]
    public GameObject[] lightWorldObjects;

    [Header("Dark World Objects")]
    [Tooltip("اشیایی که فقط توی دنیای معکوس (تاریکی) وجود دارن")]
    public GameObject[] darkWorldObjects;

    [Header("Status (Read Only)")]
    public bool isMirrored = false;

    void Start()
    {
        ApplyState();
    }

    public void ToggleMirror()
    {
        isMirrored = !isMirrored;
        ApplyState();
    }

    void ApplyState()
    {
        // اگه معکوس شده، دنیای تاریکی رو فعال کن و دنیای نور رو خاموش کن، و برعکس
        foreach (GameObject obj in lightWorldObjects)
        {
            if (obj != null) obj.SetActive(!isMirrored);
        }

        foreach (GameObject obj in darkWorldObjects)
        {
            if (obj != null) obj.SetActive(isMirrored);
        }
    }
}