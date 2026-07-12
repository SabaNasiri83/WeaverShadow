using UnityEngine;

// این اسکریپت رو روی هر آبجکتی که صدا لازم داره بچسبون (اهرم، در، باس و...)
// خودش AudioSource رو اگه نبود اضافه می‌کنه
public class RandomSFX : MonoBehaviour
{
    [Header("Clips")]
    [Tooltip("چندتا نسخه‌ی مختلف از یه صدا (مثلا impactMetal_light_000 تا 004) - هر بار یکی‌شون رندوم پخش می‌شه")]
    public AudioClip[] clips;

    [Header("Settings")]
    [Range(0f, 1f)] public float volume = 1f;
    [Tooltip("برای اینکه هر بار دقیقاً عین هم به گوش نرسه، پیچ رو کمی رندوم می‌کنه")]
    public float minPitch = 0.92f;
    public float maxPitch = 1.08f;

    private AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        if (source == null) source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
    }

    // این متد رو از هر اسکریپت دیگه‌ای صدا بزن: mySfx.Play();
    public void Play()
    {
        if (clips == null || clips.Length == 0) return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];
        source.pitch = Random.Range(minPitch, maxPitch);
        source.PlayOneShot(clip, volume);
    }
}
