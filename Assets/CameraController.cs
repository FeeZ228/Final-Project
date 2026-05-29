using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target Tracking")]
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("Camera Zoom / Size")]
    public float targetSize = 5f;
    public float zoomSpeed = 5f;

    [Header("Environment Bounds")]
    public bool useBounds = false;
    public Vector2 minBounds;
    public Vector2 maxBounds;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 1. Handle dynamic camera size (zoom)
        if (cam.orthographic)
        {
            cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, targetSize, zoomSpeed * Time.deltaTime);
        }

        // 2. Smoothly track the target player sprite
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 3. Constrain camera position inside tilemap limits if enabled
        if (useBounds)
        {
            // Calculate half heights and widths to prevent camera edges from clipping past boundaries
            float camHeight = cam.orthographicSize;
            float camWidth = camHeight * cam.aspect;

            float clampedX = Mathf.Clamp(smoothedPosition.x, minBounds.x + camWidth, maxBounds.x - camWidth);
            float clampedY = Mathf.Clamp(smoothedPosition.y, minBounds.y + camHeight, maxBounds.y - camHeight);

            smoothedPosition = new Vector3(clampedX, clampedY, smoothedPosition.z);
        }

        transform.position = smoothedPosition;
    }
}
