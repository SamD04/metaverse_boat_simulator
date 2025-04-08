using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoatController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float maxForwardSpeed = 20f;
    public float maxReverseSpeed = 10f;
    public float acceleration = 5f;
    public float deceleration = 8f;
    public float reverseAcceleration = 3f;

    [Header("Steering Settings")]
    public float turnRate = 1.5f;
    public float speedTurnDampening = 0.7f; // How much speed affects turning
    public float turnDrag = 0.5f; // Resistance when turning

    [Header("Water Physics")]
    public float waterDrag = 0.1f;
    public float waterAngularDrag = 0.5f;

    private Rigidbody rb;
    private float currentThrottle;
    private float currentSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0); // Lower center for stability
    }

    private void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        ApplySteering();
        ApplyWaterPhysics();
    }

    private void HandleInput()
    {
        float throttleInput = Input.GetAxis("Vertical");

        // Forward/Reverse input
        if (throttleInput > 0)
        {
            // Accelerating forward
            currentThrottle = Mathf.MoveTowards(currentThrottle, throttleInput, acceleration * Time.deltaTime);
        }
        else if (throttleInput < 0)
        {
            // Reversing or braking
            if (currentSpeed > 0.1f)
            {
                // Braking first
                currentThrottle = Mathf.MoveTowards(currentThrottle, 0, deceleration * Time.deltaTime);
            }
            else
            {
                // Reversing
                currentThrottle = Mathf.MoveTowards(currentThrottle, throttleInput, reverseAcceleration * Time.deltaTime);
            }
        }
        else
        {
            // No input - natural deceleration
            currentThrottle = Mathf.MoveTowards(currentThrottle, 0, deceleration * Time.deltaTime);
        }
    }

    private void ApplyMovement()
    {
        // Calculate current speed
        if (currentThrottle > 0)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, maxForwardSpeed * currentThrottle, acceleration * Time.fixedDeltaTime);
        }
        else if (currentThrottle < 0)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, maxReverseSpeed * currentThrottle, reverseAcceleration * Time.fixedDeltaTime);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, deceleration * Time.fixedDeltaTime);
        }

        // Apply forward force
        Vector3 forwardForce = transform.forward * currentSpeed;
        rb.AddForce(forwardForce, ForceMode.Acceleration);
    }

    private void ApplySteering()
    {
        float steerInput = Input.GetAxis("Horizontal");

        // Only allow steering when moving
        if (Mathf.Abs(currentSpeed) > 0.1f)
        {
            // Turning is more responsive at lower speeds
            float turnFactor = 1f - (Mathf.Abs(currentSpeed) / maxForwardSpeed * speedTurnDampening);
            float turnForce = steerInput * turnRate * turnFactor * Mathf.Sign(currentSpeed);

            // Apply rotation
            rb.AddTorque(0, turnForce, 0, ForceMode.Acceleration);

            // Add some drag when turning to make it feel more "water-like"
            if (Mathf.Abs(steerInput) > 0.1f)
            {
                rb.velocity *= (1f - turnDrag * Time.fixedDeltaTime);
            }
        }
    }

    private void ApplyWaterPhysics()
    {
        // Apply drag
        rb.AddForce(-rb.velocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
        rb.AddTorque(-rb.angularVelocity * waterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }
}
