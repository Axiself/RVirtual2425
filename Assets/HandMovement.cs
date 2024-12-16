using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;


public class HandMovement : MonoBehaviour
{
    Rigidbody droneBody;
    private string positionDataFilePath;
    [Header("Controller")]
    [SerializeField] public GameObject Hand;
    public UnityEngine.XR.InputDevice rightController;
    private StreamWriter file;
    public InputActionReference upAction; // Grip button
    public InputActionReference downAction;
    public Transform droneTransform;
    public int count = 0;

    void Awake()
    {
        droneBody = GetComponent<Rigidbody>();
        droneTransform = GameObject.FindGameObjectWithTag("Drone").transform;
        droneBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        InitializeRightHand();
    }

    void InitializeRightHand()
    {
        var inputDevices = new List<UnityEngine.XR.InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right, inputDevices);
        if (inputDevices.Count > 0)
        {
            rightController = inputDevices[0];
        }
        else
        {
            //Debug.LogError("Right controller not found!");
        }
    }

    public void OnEnable()
    {
        upAction.action.started += moveUp;
        downAction.action.started += moveDown;
        upAction.action.canceled += stopMovingUp;
        downAction.action.canceled += stopMovingDown;
    }
    public void OnDisable()
    {
        upAction.action.performed -= moveUp;
        downAction.action.performed -= moveDown;
    }

    bool isMovingUp = false;
    bool isMovingDown = false;

    public void moveUp(InputAction.CallbackContext args)
    {
        isMovingUp = true;
    }

    public void moveDown(InputAction.CallbackContext args)
    {
        isMovingDown = true;
    }

    public void stopMovingUp(InputAction.CallbackContext args)
    {
        isMovingUp = false;
    }

    public void stopMovingDown(InputAction.CallbackContext args)
    {
        isMovingDown = false;
    }

    void FixedUpdate()
    {
        if (!rightController.isValid) InitializeRightHand();
        MovementUpDown();
        MovementForward();
        Rotation();
        ClampingSpeedValues();
        Swerve();
        AlignDroneUpright();

        droneBody.AddRelativeForce(Vector3.up * upForce);
       
    }

    public float upForce;
    void MovementUpDown()
    {
        if (Input.GetKey(KeyCode.Space) || isMovingUp)
        {
            upForce = 450f;
        }
        else if (Input.GetKey(KeyCode.LeftControl) || isMovingDown)
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
        }
    }

    private float rotationSpeed = 2.5f; // Maximum turning speed in degrees per second
    private float deadzone = 20f; // Deadzone angle in degrees
    private float smoothTime = 0.1f; // Smoothing factor for rotation
    private float currentRotationSpeed; // Current rotation speed for smoothing
    private float rotationVelocity; // Velocity tracker for SmoothDamp
    private bool isInitialRotationSet;
    private Quaternion initialControllerRotation;

    public float maxRotationAngle = 45f; // Maximum rotation angle in degrees

    private Vector3 maxRotationAngles = new Vector3(10f, 45f, 10f); // Max rotation for X (pitch), Y (yaw), Z (roll)

    // Update the Rotation method
    void Rotation()
    {
        if (rightController.isValid)
        {
            if (rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceRotation, out Quaternion currentRotation))
            {
                // Set the initial rotation once and align the drone to the controller's starting position
                if (!isInitialRotationSet)
                {
                    initialControllerRotation = currentRotation;
                    transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0); // Ensure upright alignment
                    isInitialRotationSet = true;
                    return; // Skip rotation calculations on the first frame
                }

                // Calculate the rotation difference
                Quaternion deltaRotation = Quaternion.Inverse(initialControllerRotation) * currentRotation;

                // Extract only the yaw (Y-axis rotation)
                Vector3 deltaEuler = deltaRotation.eulerAngles;
                float yawChange = deltaEuler.y > 180 ? deltaEuler.y - 360 : deltaEuler.y;

                // Apply a deadzone 
                if (Mathf.Abs(yawChange) < deadzone && Mathf.Abs(yawChange) > -deadzone)
                {
                    yawChange = 0; // Ignore small movements within the deadzone
                }

                // Prevent rotation at the start until meaningful input is detected
                if (yawChange == 0 && !isInitialRotationSet)
                {
                    currentRotationSpeed = 0; // Ensure no rotation
                    return;
                }

                // Clamp the yaw change to the max rotation angle
                yawChange = Mathf.Clamp(yawChange, -maxRotationAngles.y, maxRotationAngles.y);

                // Gradually apply the rotation
                float targetRotationSpeed = yawChange * rotationSpeed;
                currentRotationSpeed = Mathf.SmoothDamp(currentRotationSpeed, targetRotationSpeed, ref rotationVelocity, smoothTime);

                // Rotate only around the Y-axis
                Vector3 newEulerAngles = transform.eulerAngles;
                newEulerAngles.y += currentRotationSpeed * Time.deltaTime;
                transform.eulerAngles = newEulerAngles;

                // Ensure the drone stays upright by clamping pitch and roll
                ClampDroneRotation();
            }
        }
    }


    // Helper method to clamp the drone's rotation and correct orientation
    void ClampDroneRotation()
    {
        Vector3 clampedEuler = transform.eulerAngles;
        clampedEuler.x = 0; // Lock pitch
        clampedEuler.z = 0; // Lock roll
        transform.eulerAngles = clampedEuler;
    }


    public Vector3 movementForwardDirection;

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
        }
    }

    // Helper method to keep the drone upright
    void AlignDroneUpright()
    {
        Vector3 currentEulerAngles = transform.eulerAngles;

        // Reset pitch and roll to 0, maintaining the current yaw
        currentEulerAngles.x = 0; // No tilt forward/backward
        currentEulerAngles.z = 0; // No tilt sideways

        transform.rotation = Quaternion.Euler(currentEulerAngles);
    }
}
