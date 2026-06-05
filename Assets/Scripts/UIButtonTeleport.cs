using UnityEngine;

public class UIButtonTeleport : MonoBehaviour
{
    // Drag your Player Prefab/Instance here in the inspector
    public Transform playerTransform;

    // Drag your Portal Object here in the inspector
    public Transform portalTransform;

    // This function MUST be public so the UI button can find it
    public void TeleportToPortal()
    {
        if (playerTransform != null && portalTransform != null)
        {
            // Move the player to the portal's position
            playerTransform.position = portalTransform.position;

            Debug.Log("Player teleported to portal!");
        }
        else
        {
            Debug.LogWarning("Missing Player or Portal assignment on the script!");
        }
    }
}