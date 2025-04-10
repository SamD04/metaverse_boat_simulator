using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoatController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float maxForwardSpeed = 20f;
    public float maxReverseSpeed = 10f;
    public float acceleration = 5f;
    public float deceleration = 3f;
    public float reverseAcceleration = 2f;

    [Header("Steering Settings")]
    public float turnRate = 2.0f;
    public float speedTurnDampening = 0.5f;
    public float turnDrag = 0.5f;

    [Header("Water Physics")]
    public float waterDrag = 0.3f;
    public float waterAngularDrag = 1.0f;

    private Rigidbody rb;
    private float currentThrottle;
    private float currentSpeed;
    private float lastSteerInput = 0f;

    private Vector3 initialCenterOfMass;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        initialCenterOfMass = rb.centerOfMass;
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
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
        float steerInput = Input.GetAxis("Horizontal");

        if (throttleInput > 0)
        {
            currentThrottle = Mathf.MoveTowards(currentThrottle, throttleInput, acceleration * Time.deltaTime);
        }
        else if (throttleInput < 0)
        {
            if (currentSpeed > 0.1f)
            {
                currentThrottle = Mathf.MoveTowards(currentThrottle, 0, deceleration * Time.deltaTime);
            }
            else
            {
                currentThrottle = Mathf.MoveTowards(currentThrottle, throttleInput, reverseAcceleration * Time.deltaTime);
            }
        }
        else
        {
            currentThrottle = Mathf.MoveTowards(currentThrottle, 0, deceleration * Time.deltaTime);
        }

        if (Mathf.Abs(currentSpeed) > 0.5f)
        {
            lastSteerInput = Mathf.Lerp(lastSteerInput, steerInput, 0.1f);
        }
        else
        {
            lastSteerInput = steerInput;
        }
    }

    private void ApplyMovement()
    {
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

        Vector3 forwardForce = transform.forward * currentSpeed;
        rb.AddForce(forwardForce, ForceMode.Acceleration);
    }

    private void ApplySteering()
    {
        if (Mathf.Abs(currentSpeed) > 0.1f)
        {
            float turnFactor = 1f - (Mathf.Abs(currentSpeed) / maxForwardSpeed * speedTurnDampening);
            float turnForce = lastSteerInput * turnRate * turnFactor * Mathf.Sign(currentSpeed);

            rb.AddTorque(0, turnForce, 0, ForceMode.Acceleration);

            if (Mathf.Abs(lastSteerInput) > 0.1f)
            {
                rb.velocity *= (1f - turnDrag * Time.fixedDeltaTime);
            }
        }
    }

    private void ApplyWaterPhysics()
    {
        rb.AddForce(-rb.velocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
        rb.AddTorque(-rb.angularVelocity * waterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }
}
