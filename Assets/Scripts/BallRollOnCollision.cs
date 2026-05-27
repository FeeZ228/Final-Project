using UnityEngine;

// The ball will start rolling after a collision.
// Attach this script to the ball asset
public class BallRoll : MonoBehaviour
{
    private Rigidbody2D rb;  // Rigidbody2D component for the ball
    private bool hasHitGround; // A flag to ensure this happens only once


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        hasHitGround = false;  
        // Get the Rigidbody2D component attached to this object
        rb = GetComponent<Rigidbody2D>();   
    }
    // This method is called automatically by Unity when the ball collides with something.
    void OnCollisionEnter2D(Collision2D collision) {
        // Check to see if the ball has collided yet.
        if (!hasHitGround)
        {
            // Collided! Apply horizontal force to start the rolling. ForceMode2D.Impulse applies it immediately.
            rb.AddForce(new Vector2(2f, 0f), ForceMode2D.Impulse);
            
            // Set the flag to true so this only happens on the first collision and not as the ball collides when it bounces.
            hasHitGround = true;
        }
    }
}
