using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    // Input values
    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;

    // Settings
    [SerializeField] private float engineForce = 1500f;
    [SerializeField] private float maxSteerAngle = 30f;
    [SerializeField] private float decelerationForce = 1000f; // Deceleration force when no input
    [SerializeField] private float brakeForce = 3000f; // Force applied when braking
    [SerializeField] private float maxSpeed = 325f; // Maximum speed in km/h

    // Wheel Colliders
    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    // Wheels
    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    // Speed variables
    private float currentSpeedKmph;

    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        CalculateSpeed();
        PrintSpeed();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        
        // Check for spacebar input
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyBrakes(brakeForce);
        }
        else
        {
            if (verticalInput == 0)
            {
                ApplyBrakes(decelerationForce);
            }
            else
            {
                ApplyBrakes(0f);
            }
        }
    }

    private void ApplyBrakes(float brakeTorque)
    {
        frontLeftWheelCollider.brakeTorque = brakeTorque;
        frontRightWheelCollider.brakeTorque = brakeTorque;
        rearLeftWheelCollider.brakeTorque = brakeTorque;
        rearRightWheelCollider.brakeTorque = brakeTorque;
    }

    private void HandleMotor()
    {
        // Calculate current speed
        currentSpeedKmph = CalculateSpeed();

        // Limit acceleration if max speed is reached
        if (currentSpeedKmph < maxSpeed)
        {
            float motorTorque = verticalInput * engineForce;
            frontLeftWheelCollider.motorTorque = motorTorque;
            frontRightWheelCollider.motorTorque = motorTorque;
        }
        else
        {
            frontLeftWheelCollider.motorTorque = 0f;
            frontRightWheelCollider.motorTorque = 0f;
        }
    }

    private float CalculateSpeed()
    {
        // Calculate speed in km/h
        float wheelRpm = (frontLeftWheelCollider.rpm + frontRightWheelCollider.rpm) / 2f;
        float wheelCircumference = frontLeftWheelCollider.radius * 2 * Mathf.PI;
        float speedMps = wheelRpm * wheelCircumference / 60f; // Speed in meters per second
        float speedKmph = speedMps * 3.6f; // Convert to km/h

        return speedKmph;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }

    private void PrintSpeed()
    {
        // Print the speed in the console
        Debug.Log("Speed: " + currentSpeedKmph.ToString("F2") + " km/h");
    }
}
