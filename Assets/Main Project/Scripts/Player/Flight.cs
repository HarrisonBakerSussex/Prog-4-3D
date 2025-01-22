using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flight : MonoBehaviour
{
    // Physics
    [SerializeField] private Vector3 turnTorques;
    [SerializeField] private float thrust, minThrust, maxThrust;
    [SerializeField] private Vector2 minMaxThrottleThrust = new Vector2(0f, 0.1f);
    [SerializeField] private float drag, increaseDragThreshold = -0.1f, dragScalar = 1f;
    [SerializeField] private float bankRate;
    [SerializeField] private float forceMultiplier;
    [SerializeField] private float gravity, littleGravity;
    [SerializeField] private float minAngularVelocityForTrails;
    [SerializeField] private float enableGravityAfter;
    private Vector3 turnVectors;

    // Auto rotate
    [SerializeField] private float autoSensitivity;
    [SerializeField] private float autoTurnAngle;
    private bool autoRotate = false;
    private bool trailsAreEnabled = false;

    private float thrustFromSine, dragFromCos, thrustFromThrottle, gravityDelay;
    private float currentThrust, currentDrag;

    // Components
    [SerializeField] private Flight_Inputs playerInput;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private TrailRenderer lTrail, rTrail;

    private void Start(){
        rb.centerOfMass = Vector3.zero;
        Wingsuit currentWingsuit = GameController.instance.wingsuits[GameController.instance.currentWingsuit];
        maxThrust = currentWingsuit.maxThrust;
        gravityDelay = enableGravityAfter;
    }

    private void Update(){
        // Get the flight inputs
        turnVectors = playerInput.planeVectors;
        // Calculate see if we need to roll the player back to the original position
        autoRotate = !(Mathf.Abs(turnVectors.z) > 0.1f);
        // Set the Yaw, Pitch, Roll based on flight inputs and auto roll
        turnVectors.z = autoRotate ? AutoPilot() : turnVectors.z;
    }

    private void FixedUpdate(){
        CalculateGravity();
        CalculateThrust();
        CalculateThrust();
        EnableTrails();
    }

    private void OnCollisionEnter(Collision collision){
        playerInput.UIController.EndResult();
    }

    private void CalculateGravity(){
        bool useLittleGravity = false;
        if (gravityDelay > 0f){
            gravityDelay -= Time.fixedDeltaTime;
            useLittleGravity = true;;
        }
        // Create gravity vector and apply as global Force
        float finalGravity = useLittleGravity ? littleGravity : gravity;
        Vector3 g = Vector3.down * finalGravity * forceMultiplier;
        rb.AddForce(g, ForceMode.Force);
    }

    private void CalculateThrust(){
        float pitchAngle = transform.eulerAngles.x;
        float pitchAngleRadians = pitchAngle * Mathf.Deg2Rad;
        thrustFromSine = Mathf.Sin(pitchAngleRadians);

        dragFromCos = (1 - Mathf.Cos(pitchAngleRadians));
        if (thrustFromSine <= increaseDragThreshold){ // If angled up increase drag more
            dragFromCos*= dragScalar;
        }
        currentDrag = dragFromCos;
        
        Vector2 throttleTargets = playerInput.throttleTargets;
        thrustFromThrottle =  Mathf.Clamp(1-Mathf.InverseLerp(throttleTargets.x, throttleTargets.y, playerInput.throttle), minMaxThrottleThrust.x, minMaxThrottleThrust.y);
        Vector3 thrustVector = Vector3.forward * currentThrust * forceMultiplier;
        currentThrust += (thrustFromThrottle + thrustFromSine) * thrust * Time.deltaTime;
        currentThrust = Mathf.Clamp(currentThrust, 0, maxThrust);

        if(rb.velocity.magnitude >= minThrust || thrustFromSine > 0f){
            rb.AddRelativeForce(thrustVector, ForceMode.Force);
            rb.drag = drag + currentDrag;
        }
        else{
            currentThrust = 0;
        }
    }

    private void CalculateTorques(){
        // Create turn vectors and apply as relative torque
        float pitchVector = turnTorques.x * turnVectors.x;
        float yawVector = turnTorques.y * turnVectors.y;
        float rollVector = turnTorques.z * turnVectors.z;
        
        rb.AddRelativeTorque(new Vector3(pitchVector, yawVector, rollVector) * forceMultiplier, ForceMode.Force);
        // Apply bank forces
        Vector3 bankForce = Vector3.up * (-transform.right.y) * bankRate * forceMultiplier;
        rb.AddRelativeTorque(bankForce, ForceMode.Force);
    }

    private float AutoPilot()
    {
        var wingsLevelInfluence = Mathf.InverseLerp(0f, autoTurnAngle, autoSensitivity);
        return Mathf.Lerp(-transform.right.y, 0f, wingsLevelInfluence);
    }

    private void EnableTrails(){
        bool enableTrails = Mathf.Abs(rb.angularVelocity.magnitude) > minAngularVelocityForTrails;
        if (enableTrails && !trailsAreEnabled){
            // Clear trail renderers
            lTrail.Clear();
            rTrail.Clear();
        }
        trailsAreEnabled = enableTrails;
        lTrail.gameObject.SetActive(enableTrails);
        rTrail.gameObject.SetActive(enableTrails);
    }
}
