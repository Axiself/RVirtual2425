using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    private Transform droneTransform;
    private int toggleView;

    void Awake()
    {
        toggleView = 0;
        droneTransform = GameObject.FindGameObjectWithTag("Drone").transform;
        angle = 20;
    }

    private Vector3 velocityCamSmooth = Vector3.zero;
    public Vector3 behindPosition = new Vector3(0, 2.5f, -5);
    public float angle;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.V))
        {
            toggleView = toggleView == 0 ? 1 : 0;
        }
    }
    void FixedUpdate()
    {
        if (toggleView == 0)
        {
            //first person view
            transform.position = droneTransform.TransformPoint(new Vector3(0, 1.5f, 0));
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, droneTransform.TransformPoint(behindPosition) + Vector3.up * Input.GetAxis("Vertical"), ref velocityCamSmooth, 0.05f);
        }
        transform.rotation = Quaternion.Euler(new Vector3(angle, droneTransform.GetComponent<DroneMovementScript>().currentYRotation, 0));
        
    }
}
