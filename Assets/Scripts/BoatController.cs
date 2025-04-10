using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(Rigidbody))]
public class BoatController : MonoBehaviour
{
    [Header("References")]
    public CinemachineVirtualCamera virtualCamera;
    public Transform visualModel; // assign your boat mesh/child here

    [Header("Movement")]
    public float maxForwardSpeed = 20f;
    public float maxReverseSpeed = 10f;
    public float acceleration = 5f;
    public float deceleration = 3f;
    public float reverseAcceleration = 2f;

    [Header("Steering")]
    public float turnRate = 2.0f;
    public float speedTurnDampening = 0.5f;
    public float turnDrag = 0.5f;

    [Header("Water Physics")]
    public float waterDrag = 0.3f;
    public float waterAngularDrag = 1.0f;
    public float lateralDrag = 2.5f;
    public float naturalDeceleration = 1.5f;

    [Header("Camera FOV")]
    public float minFOV = 60f;
    public float maxFOV = 90f;
    public float fovLerpSpeed = 2f;

    [Header("Visual Tilt")]
    public float modelYawIntensity = 15f;
    public float modelYawSmoothing = 5f;

    private Rigidbody rb;
    private float currentThrottle;
    private float currentSpeed;
    private float lastSteerInput = 0f;
    private Vector3 initialCOM;
    private Quaternion initialModelRotation;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        initialCOM = rb.centerOfMass;
        rb.centerOfMass = new Vector3(0, -0.5f, 0);

        if (visualModel != null)
            initialModelRotation = visualModel.localRotation;
    }

    void Update()
    {
        HandleInput();
        UpdateVisualTilt();
    }

    void FixedUpdate()
    {
        ApplyMovement();
        ApplySteering();
        ApplyWaterPhysics();
        ApplyDrift();
        ApplyCameraFOV();
    }

    void HandleInput()
    {
        float throttle = Input.GetAxis("Vertical");
        float steer = Input.GetAxis("Horizontal");

        // Throttle logic
        if (throttle > 0)
            currentThrottle = Mathf.MoveTowards(currentThrottle, throttle, acceleration * Time.deltaTime);
        else if (throttle < 0)
            currentThrottle = currentSpeed > 0.1f
                ? Mathf.MoveTowards(currentThrottle, 0, deceleration * Time.deltaTime)
                : Mathf.MoveTowards(currentThrottle, throttle, reverseAcceleration * Time.deltaTime);
        else
            currentThrottle = Mathf.MoveTowards(currentThrottle, 0, deceleration * Time.deltaTime);

        // Steering memory
        lastSteerInput = Mathf.Abs(currentSpeed) > 0.5f
            ? Mathf.Lerp(lastSteerInput, steer, 0.1f)
            : steer;
    }

    void ApplyMovement()
    {
        float targetSpeed = currentThrottle > 0
            ? maxForwardSpeed * currentThrottle
            : maxReverseSpeed * currentThrottle;

        float accel = currentThrottle > 0 ? acceleration : reverseAcceleration;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, accel * Time.fixedDeltaTime);

        rb.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration);
    }

    void ApplySteering()
    {
        if (Mathf.Abs(currentSpeed) > 0.1f)
        {
            float turnFactor = 1f - (Mathf.Abs(currentSpeed) / maxForwardSpeed * speedTurnDampening);
            float torque = lastSteerInput * turnRate * turnFactor * Mathf.Sign(currentSpeed);
            rb.AddTorque(0, torque, 0, ForceMode.Acceleration);

            if (Mathf.Abs(lastSteerInput) > 0.1f)
                rb.velocity *= (1f - turnDrag * Time.fixedDeltaTime);
        }
    }

    void ApplyWaterPhysics()
    {
        rb.AddForce(-rb.velocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
        rb.AddTorque(-rb.angularVelocity * waterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }

    void ApplyDrift()
    {
        Vector3 localVel = transform.InverseTransformDirection(rb.velocity);
        localVel.x = Mathf.Lerp(localVel.x, 0, lateralDrag * Time.fixedDeltaTime);

        if (Mathf.Abs(currentThrottle) < 0.01f)
            localVel.z = Mathf.MoveTowards(localVel.z, 0, naturalDeceleration * Time.fixedDeltaTime);

        rb.velocity = transform.TransformDirection(localVel);
    }

    void ApplyCameraFOV()
    {
        if (virtualCamera == null) return;

        float speedPercent = Mathf.Clamp01(rb.velocity.magnitude / maxForwardSpeed);
        float targetFOV = Mathf.Lerp(minFOV, maxFOV, speedPercent);

        var lens = virtualCamera.m_Lens;
        lens.FieldOfView = Mathf.Lerp(lens.FieldOfView, targetFOV, Time.deltaTime * fovLerpSpeed);
        virtualCamera.m_Lens = lens;
    }

    void UpdateVisualTilt()
    {
        if (visualModel == null) return;

        float yawOffset = lastSteerInput * modelYawIntensity;
        Quaternion targetRot = initialModelRotation * Quaternion.Euler(0, yawOffset, 0);

        visualModel.localRotation = Quaternion.Lerp(
            visualModel.localRotation,
            targetRot,
            Time.deltaTime * modelYawSmoothing
        );
    }
}
