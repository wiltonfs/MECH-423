using System.Collections;
using UnityEngine;

public class EnemyViz : MonoBehaviour
{
    public AudioClip pingSound;
    public float minOpacity = 0.2f; // Minimum opacity
    public float opacityDecaySeconds = 1.0f; // Time to decay back to min opacity
    public float scaleJump = 1.2f;
    public float scaleDecaySeconds = 0.25f;
    
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private Vector3 originalScale;
    private float lastSoundTime = 0;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
        audioSource = GetComponent<AudioSource>();

        // Start invisible
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);
    }

    public void Pulse()
    {
        if (pingSound != null && audioSource != null && Time.time > (lastSoundTime + 1f))
        {
            lastSoundTime = Time.time;
            audioSource.PlayOneShot(pingSound);
        }
        StopAllCoroutines(); // Stop any ongoing pulse
        StartCoroutine(PulseEffect());
    }

    private IEnumerator PulseEffect()
    {
        // Set to full opacity and increase size
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
        transform.localScale = originalScale * 1.2f;

        // Gradually decay opacity and size
        float elapsed = 0f;
        while (elapsed < Mathf.Max(opacityDecaySeconds, scaleDecaySeconds))
        {
            float opacity = Mathf.Lerp(1f, minOpacity, elapsed / opacityDecaySeconds);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, opacity);
            transform.localScale = Vector3.Lerp(originalScale * scaleJump, originalScale, elapsed / scaleDecaySeconds);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset to minimum opacity and original size
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, minOpacity);
        transform.localScale = originalScale;
    }
}
