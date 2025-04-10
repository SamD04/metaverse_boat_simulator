using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floatScript : MonoBehaviour
{
    public Rigidbody rigidBody;
    public float depthBeforeSubmerged = 1f;
    public float displacementAmount = 3f;
    public int floaterCount = 1;
    public float waterDrag = 0.5f;
    public float waterAngularDrag = 1f;
    public float floatForceMultiplier = 1f;

    private void FixedUpdate()
    {
        // 1. Limit excessive angular velocity to avoid jitter
        if (rigidBody.angularVelocity.magnitude > 5f)
        {
            rigidBody.angularVelocity = rigidBody.angularVelocity.normalized * 5f;
        }

        // 2. Apply gravity compensation (just a base downward force)
        rigidBody.AddForceAtPosition(Physics.gravity / floaterCount, transform.position, ForceMode.Acceleration);

        // 3. Get wave height to simulate the floating (based on position)
        float waveHeight = waveScript.instance.GetWaveHeight(transform.position.x);

        // Only apply forces when submerged (y < wave height)
        if (transform.position.y < waveHeight)
        {
            // 4. Calculate displacement force based on submerged depth
            float displacementMultiplier = Mathf.Clamp01((waveHeight - transform.position.y) / depthBeforeSubmerged) * displacementAmount;
            float floatForce = Mathf.Abs(Physics.gravity.y) * displacementMultiplier * floatForceMultiplier;

            // 5. Apply buoyancy force (upward force)
            rigidBody.AddForceAtPosition(new Vector3(0f, floatForce, 0f), transform.position, ForceMode.Acceleration);

            // 6. Apply water drag (resistance to motion)
            // Apply linear drag (resistance to movement)
            rigidBody.AddForce(-rigidBody.velocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
            // Apply angular drag (resistance to rotation)
            rigidBody.AddTorque(-rigidBody.angularVelocity * waterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }
}
