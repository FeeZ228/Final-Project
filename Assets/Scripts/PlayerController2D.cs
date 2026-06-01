using UnityEngine; 
using UnityEngine.InputSystem; 

public class PlayerController2D : MonoBehaviour 
{ 
    private InputSystem_Actions playerControls; 
    private Rigidbody2D rb; 
    private SpriteRenderer spriteRenderer; 
    private Animator animator; 

    [Header("Movement Configuration")] 
    [SerializeField] private float moveSpeed = 8f; 
    [SerializeField] private float jumpForce = 12f; 
    [SerializeField] private float fallMultiplier = 2.5f; 

    [Header("Ground Check Settings")] 
    [SerializeField] private Transform groundCheckPoint; 
    [SerializeField] private float groundCheckRadius = 0.3f; 
    [SerializeField] private LayerMask groundLayer; 

    [Header("Wall Interaction Settings")] 
    [Tooltip("The side check point used to detect walls on the RIGHT.")] 
    [SerializeField] private Transform rightWallCheckPoint; 
    [Tooltip("The side check point used to detect walls on the LEFT.")] 
    [SerializeField] private Transform leftWallCheckPoint; 
    [Tooltip("Radius of the wall detection check circle.")] 
    [SerializeField] private float wallCheckRadius = 0.25f; 
    [Tooltip("The layer assigned to your level walls (often the same as your ground layer).")] 
    [SerializeField] private LayerMask wallLayer; 
    [Tooltip("The slow, constant downward speed applied when sliding down a wall.")] 
    [SerializeField] private float wallSlideSpeed = 2f; 
    [Tooltip("Horizontal force applied when wall jumping away from the surface.")] 
    [SerializeField] private float wallJumpHorizontalForce = 10f; 
    [Tooltip("Vertical force applied during a wall jump.")] 
    [SerializeField] private float wallJumpVerticalForce = 12f; 
    [Tooltip("How long (in seconds) player input is locked immediately after executing a wall jump.")] 
    [SerializeField] private float wallJumpControlLockTime = 0.15f; 

    [Header("Trap Interaction Settings")] 
    [SerializeField] private LayerMask trapLayer; 

    [Header("Damage & Knockback Tuning")] 
    [SerializeField] private float knockbackHorizontalForce = 6f; 
    [SerializeField] private float knockbackDuration = 0.25f; 

    [Header("Animation Tuning")] 
    [SerializeField] private float runSpeedAnimationMultiplier = 0.15f; 

    private Vector2 moveInput; 
    private bool isGrounded; 
    
    // Wall mechanics states 
    private bool isTouchingWall; 
    private bool isTouchingRightWall;
    private bool isTouchingLeftWall;
    private bool isWallSliding; 
    private bool isWallJumping; 
    private float wallJumpTimer; 
    private float wallJumpDirection; 

    // Damage state tracking 
    private bool isKnockedBack; 
    private float knockbackTimer; 

    private void Awake() 
    { 
        playerControls = new InputSystem_Actions(); 
        rb = GetComponent<Rigidbody2D>(); 
        spriteRenderer = GetComponent<SpriteRenderer>(); 
        animator = GetComponent<Animator>(); 
    } 

    private void OnEnable() 
    { 
        playerControls.Player.Enable(); 
        playerControls.Player.Jump.performed += OnJumpTriggered; 
    } 

    private void OnDisable() 
    { 
        playerControls.Player.Jump.performed -= OnJumpTriggered; 
        playerControls.Player.Disable(); 
    } 

    private void Update() 
    { 
        // 1. Handle Knockback Timer 
        if (isKnockedBack) 
        { 
            knockbackTimer -= Time.deltaTime; 
            if (knockbackTimer <= 0) isKnockedBack = false; 
        } 

        // 2. Handle Wall Jump Input Lock Timer 
        if (isWallJumping) 
        { 
            wallJumpTimer -= Time.deltaTime; 
            if (wallJumpTimer <= 0) isWallJumping = false; 
        } 

        // 3. Gather Input (If not locked by states) 
        if (!isKnockedBack && !isWallJumping) 
        { 
            moveInput = playerControls.Player.Move.ReadValue<Vector2>(); 
        } 
        else if (isKnockedBack) 
        { 
            moveInput = Vector2.zero; 
        } 

        // 4. Perform Environmental Checks 
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer); 

        // Check right and left wall colliders independently
        isTouchingRightWall = rightWallCheckPoint != null && Physics2D.OverlapCircle(rightWallCheckPoint.position, wallCheckRadius, wallLayer); 
        isTouchingLeftWall = leftWallCheckPoint != null && Physics2D.OverlapCircle(leftWallCheckPoint.position, wallCheckRadius, wallLayer); 
        isTouchingWall = isTouchingRightWall || isTouchingLeftWall;

        // 5. Evaluate Wall Slide State (Sticky: Decoupled from moveInput.x)
        if (!isGrounded && isTouchingWall && rb.linearVelocity.y <= 0) 
        { 
            isWallSliding = true; 
        } 
        else 
        { 
            isWallSliding = false; 
        } 

        UpdateAnimations(); 
    } 

    private void FixedUpdate() 
    { 
        // Apply physics forces based on our movement states 
        if (!isKnockedBack && !isWallJumping) 
        { 
            if (isWallSliding) 
            { 
                // Constrain the downward slide to a nice steady speed
                // Allow the user to still drift outward if they try to move away
                rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed)); 
            } 
            else 
            { 
                // Standard running physics 
                rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y); 
            } 
        } 

        // Snap down faster for snappier arcade fall feel (skip while wall sliding or jumping up) 
        if (rb.linearVelocity.y < 0 && !isWallSliding) 
        { 
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime; 
        } 
    } 

    private void OnJumpTriggered(InputAction.CallbackContext context) 
    { 
        if (isKnockedBack) return; 

        if (isGrounded) 
        { 
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); 
        } 
        else if (isWallSliding) 
        { 
            // Set wall jump state configurations 
            isWallJumping = true; 
            isWallSliding = false; 
            wallJumpTimer = wallJumpControlLockTime; 

            // Explicitly dictate jump direction based on which wall we are touching
            if (isTouchingRightWall)
            {
                wallJumpDirection = -1f; // Jump Left
                spriteRenderer.flipX = true; // Face Left
            }
            else if (isTouchingLeftWall)
            {
                wallJumpDirection = 1f; // Jump Right
                spriteRenderer.flipX = false; // Face Right
            }

            // Apply the strong directional impulse away from the wall 
            rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpHorizontalForce, wallJumpVerticalForce); 
        } 
    } 

    private void OnTriggerEnter2D(Collider2D collision) 
    { 
        if (((1 << collision.gameObject.layer) & trapLayer) != 0) HandleTrapDamage(); 
    } 

    private void OnCollisionEnter2D(Collision2D collision) 
    { 
        if (((1 << collision.gameObject.layer) & trapLayer) != 0) HandleTrapDamage(); 
    } 

    private void HandleTrapDamage() 
    { 
        if (isKnockedBack) return; 
        isKnockedBack = true; 
        isWallSliding = false; 
        isWallJumping = false; 
        knockbackTimer = knockbackDuration; 

        float pushDirection = spriteRenderer.flipX ? 1f : -1f; 
        rb.linearVelocity = new Vector2(pushDirection * knockbackHorizontalForce, jumpForce * 0.5f); 
        animator.SetTrigger("playerHurt"); 
    } 

    private void UpdateAnimations() 
    { 
        animator.SetFloat("yVelocity", rb.linearVelocity.y); 
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x)); 
        animator.SetBool("isGrounded", isGrounded); 
        animator.SetBool("isRunning", moveInput.x != 0 && isGrounded); 

        // Pass the wall slide state explicitly into the Animator 
        animator.SetBool("isWallSliding", isWallSliding); 

        float calculatedAnimSpeed = Mathf.Abs(rb.linearVelocity.x) * runSpeedAnimationMultiplier; 
        if (moveInput.x != 0) animator.SetFloat("animSpeed", Mathf.Max(calculatedAnimSpeed, 0.5f)); 
        else animator.SetFloat("animSpeed", 1f); 

        // Only handle standard sprite flipping when completely unconstrained 
        if (!isKnockedBack && !isWallSliding && !isWallJumping) 
        { 
            if (moveInput.x < 0) spriteRenderer.flipX = true; 
            else if (moveInput.x > 0) spriteRenderer.flipX = false; 
        } 
    } 

    private void OnDrawGizmosSelected() 
    { 
        // Render Ground Check 
        if (groundCheckPoint != null) 
        { 
            Gizmos.color = Color.red; 
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius); 
        } 

        // Render Right Wall Check 
        if (rightWallCheckPoint != null) 
        { 
            Gizmos.color = Color.blue; 
            Gizmos.DrawWireSphere(rightWallCheckPoint.position, wallCheckRadius); 
        } 

        // Render Left Wall Check 
        if (leftWallCheckPoint != null) 
        { 
            Gizmos.color = Color.cyan; 
            Gizmos.DrawWireSphere(leftWallCheckPoint.position, wallCheckRadius); 
        } 
    } 
}
