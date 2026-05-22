using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The object to follow.


    // Update is called once per frame
    void Update()
    {
        transform.position = target.position - 10f*Vector3.forward;
       
    }
}
