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

    void Awake()
    {
        droneBody = GetComponent<Rigidbody>();
        droneTransform = GameObject.FindGameObjectWithTag("Drone").transform;

        var inputDevices = new List<UnityEngine.XR.InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right, inputDevices);
        if (inputDevices.Count > 0)
        {
            rightController = inputDevices[0];
        }
        else
        {
            Debug.LogError("Right controller not found!");
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

    private float wantedYRotation;
    [HideInInspector] public float currentYRotation;
    private float rotationSpeed = 2.5f;
    private float rotationYVelocity;

    private Quaternion initialControllerRotation;
    private bool isInitialRotationSet = false;
    void Rotation()
    {
        if (rightController.isValid)
        {
            // Get the controller's rotation
            if (rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceRotation, out Quaternion currentRotation))
            {
                // Set the initial rotation once
                if (!isInitialRotationSet)
                {
                    initialControllerRotation = currentRotation;
                    isInitialRotationSet = true;
                }

                // Calculate the rotation difference
                Quaternion deltaRotation = Quaternion.Inverse(initialControllerRotation) * currentRotation;

                // Extract the yaw (left/right tilt) from the deltaRotation
                Vector3 deltaEuler = deltaRotation.eulerAngles;
                float yawChange = deltaEuler.y > 180 ? deltaEuler.y - 360 : deltaEuler.y;

                // Apply the yaw change to the camera
                droneTransform.Rotate(0, yawChange * rotationSpeed * Time.deltaTime, 0, Space.World);

                // Optionally reset the controller's initial rotation (to prevent drift)
                initialControllerRotation = currentRotation;
            }
        }
        currentYRotation = Mathf.SmoothDamp(currentYRotation, wantedYRotation, ref rotationYVelocity, 0.25f);
    }

    Vector3 movementForwardDirection;

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
}
