using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DynamicCameraZoom : MonoBehaviour
{
    [Header("Grid Settings")]
    [Tooltip("The Pixels Per Unit (PPU) of your tiles (e.g., 16, 32)")]
    public int pixelsPerUnit = 16;
    
    [Tooltip("The vertical resolution you are designing for (e.g., 1080)")]
    public int targetVerticalResolution = 1080;

    [Header("Zoom Constraints")]
    public float minSize = 2f;
    public float maxSize = 20f;
    
    [Header("Controls")]
    public bool useMouseScroll = true;
    public bool useKeyboardKeys = true;
    public float scrollSensitivity = 1f;

    private Camera cam;
    private float targetSize;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;
        
        // Calculate the ideal starting size matching your grid
        targetSize = targetVerticalResolution / (2f * pixelsPerUnit);
        cam.orthographicSize = targetSize;
    }

    void Update()
    {
        float zoomDelta = 0f;

        // Handle Mouse Input
        if (useMouseScroll)
        {
            float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(mouseScroll) > 0.01f)
            {
                zoomDelta = -Mathf.Sign(mouseScroll) * scrollSensitivity;
            }
        }

        // Handle Keyboard Input (Z to Zoom In, X to Zoom Out)
        if (useKeyboardKeys)
        {
            if (Input.GetKey(KeyCode.Z)) zoomDelta = -scrollSensitivity * Time.deltaTime * 5f;
            if (Input.GetKey(KeyCode.X)) zoomDelta = scrollSensitivity * Time.deltaTime * 5f;
        }

        if (Mathf.Abs(zoomDelta) > 0.001f)
        {
            ChangeCameraSize(zoomDelta);
        }
    }

    void ChangeCameraSize(float delta)
    {
        // Adjust size target
        targetSize += delta;
        targetSize = Mathf.Clamp(targetSize, minSize, maxSize);

        // Snap size strictly to unit increments so pixels remain perfectly clean
        float pixelSize = 1f / pixelsPerUnit;
        float snappedSize = Mathf.Round(targetSize / pixelSize) * pixelSize;

        cam.orthographicSize = snappedSize;
    }
}
