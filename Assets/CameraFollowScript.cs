using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    private Transform droneTransform;

    void Awake()
    {
        droneTransform = GameObject.FindGameObjectWithTag("Drone").transform;
    }

    private Vector3 velocityCamSmooth = Vector3.zero;
    public Vector3 behindPosition = new Vector3(0, 2.5f, -5);
    public float angle = 16;
    void FixedUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, droneTransform.TransformPoint(behindPosition) + Vector3.up * Input.GetAxis("Vertical"), ref velocityCamSmooth, 0.05f);
        transform.rotation = Quaternion.Euler(new Vector3(angle, droneTransform.GetComponent<DroneMovementScript>().currentYRotation, 0));
    }
}
