using System.Collections;
using UnityEngine;

public class BasicFallingPlatform : MonoBehaviour
{
    [SerializeField] private float fallDelay = 0.8f;
    [SerializeField] private float resetDelay = 3.0f;
    [SerializeField] private float shakeMagnitude = 0.05f;

    private Rigidbody2D rb;
    private BoxCollider2D platformCollider;
    private Vector3 initialPosition;
    private bool isFalling = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        platformCollider = GetComponent<BoxCollider2D>();
        initialPosition = transform.position;

        // Force the platform to stay frozen at start
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Simple verification: Did something hit us?
        if (!isFalling)
        {
            StartCoroutine(FallSequence());
        }
    }

    private IEnumerator FallSequence()
    {
        isFalling = true;

        // Shake Phase
        float elapsed = 0f;
        while (elapsed < fallDelay)
        {
            float randomX = Random.Range(-shakeMagnitude, shakeMagnitude);
            float randomY = Random.Range(-shakeMagnitude, shakeMagnitude);
            transform.position = initialPosition + new Vector3(randomX, randomY, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        transform.position = initialPosition;

        // Fall Phase: Turn on gravity
        rb.bodyType = RigidbodyType2D.Dynamic; 
        yield return new WaitForSeconds(0.2f);
        platformCollider.enabled = false; // Turn off collider so it drops through the world

        // Reset Phase
        yield return new WaitForSeconds(resetDelay);
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero; // Use rb.velocity in Unity 2022 or older
        transform.position = initialPosition;
        platformCollider.enabled = true;
        
        isFalling = false;
    }
}
