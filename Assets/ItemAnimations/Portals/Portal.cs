using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Teleport Destination")]
    [Tooltip("Drag the other portal or target Transform here")]
    public Transform destination;

    [Header("Cooldown Settings")]
    [Tooltip("How many seconds the player must wait between teleports")]
    public float teleportCooldown = 3.0f;

    // Static variable shared across ALL portals to track global player cooldown
    private static float nextAllowedTeleportTime = 0f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Check if the object inside the portal is the Player
        if (collision.CompareTag("Player"))
        {
            // Check if the current game time has passed the cooldown threshold
            if (Time.time >= nextAllowedTeleportTime)
            {
                if (destination != null)
                {
                    // Update the global cooldown timer before moving the player
                    nextAllowedTeleportTime = Time.time + teleportCooldown;

                    // Instantly move the player to the destination
                    collision.transform.position = destination.position;
                }
                else
                {
                    Debug.LogWarning("Portal is missing a destination target!");
                }
            }
        }
    }
}
