using UnityEngine;

public class WallJumpController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    private float horizontalInput;

    [Header("Wall Jump Settings")]
    public float wallSlideSpeed = 2f;
    public Vector2 wallJumpForce = new Vector2(10f, 12f);
    public float wallJumpDuration = 0.4f;

    [Header("Collisions")]
    public Transform groundCheck;
    public Transform wallCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isWalled;
    private bool isSliding;
    private bool isWallJumping;
    private float wallJumpDirection;
    private float wallJumpTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Gather input only if not actively locked in a wall jump animation
        if (!isWallJumping)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        }

        // Check surrounding physics
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        isWalled = Physics2D.OverlapCircle(wallCheck.position, 0.2f, groundLayer);

        HandleWallSlide();
        HandleJump();
    }

    void HandleWallSlide()
    {
        // Slide if touching a wall, in the air, and pressing toward the wall
        if (isWalled && !isGrounded && horizontalInput != 0)
        {
            isSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlideSpeed, float.MaxValue));
        }
        else
        {
            isSliding = false;
        }
    }

    void HandleJump()
    {
        // Normal Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Wall Jump
        if (Input.GetButtonDown("Jump") && isSliding)
        {
            isWallJumping = true;
            wallJumpDirection = -transform.localScale.x; // Push away from wall
            wallJumpTimer = wallJumpDuration;

            rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpForce.x, wallJumpForce.y);
        }

        // Countdown the wall jump control lockout
        if (isWallJumping)
        {
            wallJumpTimer -= Time.deltaTime;
            if (wallJumpTimer <= 0)
            {
                isWallJumping = false;
            }
        }
    }

    // Optional: Flip sprite visual direction based on movement
    void FixedUpdate()
    {
        if (!isWallJumping && horizontalInput != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(horizontalInput), 1, 1);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draws check zones in the Unity Editor scene view
        if (groundCheck != null) Gizmos.DrawWireSphere(groundCheck.position, 0.2f);
        if (wallCheck != null) Gizmos.DrawWireSphere(wallCheck.position, 0.2f);
    }
}
