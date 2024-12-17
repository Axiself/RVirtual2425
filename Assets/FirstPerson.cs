using UnityEngine;

public class FirstPerson : MonoBehaviour
{
    [Header("Camera Offset Settings")]
    private Transform camera;
    private Vector3 height = new Vector3(0.0f, -0.5f, 0.0f);

    void Awake()
    {
        camera = GameObject.FindGameObjectWithTag("Drone").transform;
    }

    void FixedUpdate()
    {
        if (camera != null)
        {
            // Update the position of the Camera Offset relative to XR Origin
            transform.position = camera.position + height;
        }
    }
}
