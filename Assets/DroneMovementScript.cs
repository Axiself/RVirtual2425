using UnityEngine;

public class DroneMovementScript : MonoBehaviour
{
    Rigidbody droneBody;

    void Awake()
    {
        droneBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        MovementUpDown();
        MovementForward();
        Rotation();
        ClampingSpeedValues();
        Swerve();

        droneBody.AddRelativeForce(Vector3.up * upForce);
        droneBody.rotation = Quaternion.Euler(new Vector3(tiltAmountForward, currentYRotation, tiltAmountSideways));
    }

    public float upForce;
    void MovementUpDown()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            upForce = 450f;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            upForce = -200f;
        }
        else if (!Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.LeftControl))
        {
            upForce = 98.1f;
        }
    }

    public float movementForwardSpeed = 500.0f;
    public float tiltAmountForward = 0;
    public float tiltVelocityForward;
    void MovementForward()
    {
        if (Input.GetAxis("Vertical") != 0)
        {
            // Calculate the forward force and project it onto the horizontal plane
            Vector3 forwardForce = Vector3.ProjectOnPlane(transform.forward, Vector3.up) * Input.GetAxis("Vertical") * movementForwardSpeed;
            droneBody.AddForce(forwardForce, ForceMode.Force);

            tiltAmountForward = Mathf.SmoothDamp(tiltAmountForward, 20 * Input.GetAxis("Vertical"), ref tiltVelocityForward, 0.1f);
        }
        else
        {
            tiltAmountForward = Mathf.SmoothDamp(tiltAmountForward, 0, ref tiltVelocityForward, 0.1f);
        }
    }

    private float wantedYRotation;
    [HideInInspector]public float currentYRotation;
    private float rotateAmountByKeys = 2.5f;
    private float rotationYVelocity;
    void Rotation()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            wantedYRotation -= rotateAmountByKeys;
        }
        if (Input.GetKey(KeyCode.E))
        {
            wantedYRotation += rotateAmountByKeys;
        }

        currentYRotation = Mathf.SmoothDamp(currentYRotation, wantedYRotation, ref rotationYVelocity, 0.25f);
    }

    public Vector3 velocityToSmothDampToZero;
    void ClampingSpeedValues()
    {
        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.2f && Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2f)
        {
            droneBody.linearVelocity = Vector3.ClampMagnitude(droneBody.linearVelocity, Mathf.Lerp(droneBody.linearVelocity.magnitude, 10.0f, Time.deltaTime * 5f));
        }
        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.2f && Mathf.Abs(Input.GetAxis("Horizontal")) < 0.2f)
        {
            droneBody.linearVelocity = Vector3.ClampMagnitude(droneBody.linearVelocity, Mathf.Lerp(droneBody.linearVelocity.magnitude, 10.0f, Time.deltaTime * 5f));
        }
        if (Mathf.Abs(Input.GetAxis("Vertical")) < 0.2f && Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2f)
        {
            droneBody.linearVelocity = Vector3.ClampMagnitude(droneBody.linearVelocity, Mathf.Lerp(droneBody.linearVelocity.magnitude, 5.0f, Time.deltaTime * 5f));
        }
        if (Mathf.Abs(Input.GetAxis("Vertical")) < 0.2f && Mathf.Abs(Input.GetAxis("Horizontal")) < 0.2f)
        {
            droneBody.linearVelocity = Vector3.SmoothDamp(droneBody.linearVelocity, Vector3.zero, ref velocityToSmothDampToZero, 0.95f);
        }
    }

    public float sideMovementAmount = 300.0f;
    public float tiltAmountSideways;
    public float tiltAmountVelocity;
    void Swerve()
    {
        if (Input.GetAxis("Horizontal") != 0)
        {
            // Calculate the sideways force and project it onto the horizontal plane
            Vector3 sidewaysForce = Vector3.ProjectOnPlane(transform.right, Vector3.up) * Input.GetAxis("Horizontal") * sideMovementAmount;
            droneBody.AddForce(sidewaysForce, ForceMode.Force);

            tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, -20 * Input.GetAxis("Horizontal"), ref tiltAmountVelocity, 0.1f);
        }
        else
        {
            tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, 0, ref tiltAmountVelocity, 0.1f);
        }
    }
}
