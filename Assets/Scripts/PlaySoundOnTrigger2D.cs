using UnityEngine;

public class PlaySoundOnTrigger2D : MonoBehaviour
{
    [Header("Audio Settings")]
    [Tooltip("Drag your .mp3 or audio clip here")]
    public AudioClip soundToPlay;

    [Tooltip("Volume of the sound effect (0.0 to 1.0)")]
    [Range(0f, 1f)]
    public float volume = 1f;

    [Header("Optional Settings")]
    [Tooltip("If checked, this object will destroy itself after playing the sound (great for collectibles).")]
    public bool destroyOnCollision = false;

    private AudioSource audioSource;

    private void Awake()
    {
        // Get or add an AudioSource component automatically
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Ensure spatial blend is set for standard 2D sound
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object colliding with this is the Player
        if (other.CompareTag("Player"))
        {
            if (soundToPlay != null)
            {
                // Play OneShot so it doesn't get cut off immediately
                audioSource.PlayOneShot(soundToPlay, volume);
            }

            if (destroyOnCollision)
            {
                // Disable renderer and collider so it disappears instantly,
                // then destroy the object after the sound finishes playing.
                GetComponent<Collider2D>().enabled = false;

                SpriteRenderer sprite = GetComponent<SpriteRenderer>();
                if (sprite != null) sprite.enabled = false;

                Destroy(gameObject, soundToPlay != null ? soundToPlay.length : 0f);
            }
        }
    }
}