using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The strength of the launch impulse.")]
    [SerializeField] private float launchForce = 15f;

    [Header("Orientation Alignment")]
    [Tooltip("Check this box if your trampoline's bouncy surface is the RED arrow (Right side). Uncheck if it is the GREEN arrow (Top side).")]
    [SerializeField] private bool lookDirectionIsRightAxis = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Determine which local axis represents where the trampoline is "looking"
            Vector2 launchDirection = lookDirectionIsRightAxis ? (Vector2)transform.right : (Vector2)transform.up;

            // 1. Wipe the current velocity vector along the launch path to prevent physics damping
            rb.linearVelocity = Vector2.zero; // Use rb.velocity in Unity 2022 or older

            // 2. Fire the player instantly in the precise direction of the axis
            rb.AddForce(launchDirection * launchForce, ForceMode2D.Impulse);
        }
    }
}
