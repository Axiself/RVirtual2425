using UnityEngine;

public class ThirdPerson : MonoBehaviour
{
    [Header("Camera Offset Settings")]
    private Transform camera;
    public Vector3 offset;
    private Vector3 height = new Vector3(0.0f, 1.36144f, 0.0f);

    void Awake()
    {
        camera = GameObject.FindGameObjectWithTag("Drone").transform;
    }

    void FixedUpdate()
    {
        if (camera != null)
        {
            offset = camera.transform.forward;
            offset.y = 0;
            offset.Normalize();
            // Update the position of the Camera Offset relative to XR Origin
            transform.position = camera.position - offset*4 + height;
        }
    }
}
