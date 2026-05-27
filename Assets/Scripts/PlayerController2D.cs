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
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Animation Tuning")]
    [Tooltip("Adjusts how fast the running animation loops relative to player movement.")]
    [SerializeField] private float runSpeedAnimationMultiplier = 0.15f;

    private Vector2 moveInput;
    private bool isGrounded;

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
        moveInput = playerControls.Player.Move.ReadValue<Vector2>();
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);

        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        // Snap down faster for snappier arcade fall feel
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    private void OnJumpTriggered(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void UpdateAnimations()
    {
        // 1. Pass the raw Y velocity into the Animator (+ value = going up, - value = falling)
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        
        // 2. Pass horizontal speed check to switch between Idle and Run states
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isRunning", moveInput.x != 0 && isGrounded);

        // 3. Dynamically set the playback speed modifier parameter for the Running loop
        // The faster the player runs across the floor, the higher this float climbs.
        float calculatedAnimSpeed = Mathf.Abs(rb.linearVelocity.x) * runSpeedAnimationMultiplier;
        
        // Avoid setting animation speed to a flat 0 so the character doesn't totally freeze on calculation frames
        if (moveInput.x != 0)
        {
            animator.SetFloat("animSpeed", Mathf.Max(calculatedAnimSpeed, 0.5f));
        }
        else
        {
            animator.SetFloat("animSpeed", 1f); // Default baseline fallback for other states
        }

        // Flip character sprite asset left/right
        if (moveInput.x < 0) spriteRenderer.flipX = true;
        else if (moveInput.x > 0) spriteRenderer.flipX = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
    }
}
