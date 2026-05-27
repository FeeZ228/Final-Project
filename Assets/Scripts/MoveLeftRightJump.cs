using UnityEngine;

public class MoveLeftRightJump : MonoBehaviour
{
    public float moveHorizontalSpeed;
    public float jumpSpeed;
    public LayerMask groundLayer;
    public float groundRadius;

    // References
    private Rigidbody2D target;
    private SpriteRenderer spriteRenderer;
    private Animator animator; // Added for animation control

    private float horizontalInput;

    void Start()
    {
        target = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>(); // Cache the animator
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Sprite flipping
        if (horizontalInput < 0)
            spriteRenderer.flipX = true;
        else if (horizontalInput > 0)
            spriteRenderer.flipX = false;

        // Apply Movement
        target.linearVelocity = new Vector2(horizontalInput * moveHorizontalSpeed, target.linearVelocity.y);

        // Jump Handling
        bool grounded = IsGrounded();
        if (Input.GetButtonDown("Jump") && grounded)
        {
            target.linearVelocity = new Vector2(target.linearVelocity.x, jumpSpeed);
        }

        // --- ANIMATION CONTROLLER INTEGRATION ---
        
        // 1. Instantly update movement states
        bool isMoving = Mathf.Abs(horizontalInput) > 0.01f;
        animator.SetBool("isRunning", isMoving);
        animator.SetBool("isGrounded", grounded);
        animator.SetFloat("yVelocity", target.linearVelocity.y);

        // 2. Dynamic Speed Syncing
        if (isMoving)
        {
            // Calculate current physical speed relative to max speed
            float currentPhysicalSpeed = Mathf.Abs(target.linearVelocity.x);
            
            // Avoid division by zero if moveHorizontalSpeed is misconfigured
            if (moveHorizontalSpeed > 0)
            {
                // Multiplier scales linearly from 0 to 1 based on real velocity
                float speedMultiplier = currentPhysicalSpeed / moveHorizontalSpeed;
                
                // Set a floor value (e.g., 0.2f) so it doesn't freeze completely when starting
                animator.SetFloat("animSpeed", Mathf.Max(0.2f, speedMultiplier));
            }
        }
        else
        {
            animator.SetFloat("animSpeed", 1f); // Default speed multiplier when idle
        }
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(target.position, groundRadius, groundLayer);
    }
}
