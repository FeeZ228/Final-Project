using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed; // Recommended value: 0.1 to 0.3 in the Inspector

    void Update()
    {
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, -10f);
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
    }
}