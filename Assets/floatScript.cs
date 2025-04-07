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
        if (rigidBody.angularVelocity.magnitude > 5f)
        {
            rigidBody.angularVelocity = rigidBody.angularVelocity.normalized * 5f;
        }

        rigidBody.AddForceAtPosition(Physics.gravity / floaterCount, transform.position, ForceMode.Acceleration);
        float waveHeight = waveScript.instance.GetWaveHeight(transform.position.x);

        if (transform.position.y < waveHeight)
        {
            float displacementMultiplier = Mathf.Clamp01((waveHeight - transform.position.y) / depthBeforeSubmerged) * displacementAmount;
            float floatForce = Mathf.Abs(Physics.gravity.y) * displacementMultiplier * floatForceMultiplier;
            rigidBody.AddForceAtPosition(new Vector3(0f, floatForce, 0f), transform.position, ForceMode.Acceleration);
            rigidBody.AddForce(-rigidBody.velocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
            rigidBody.AddTorque(-rigidBody.angularVelocity * waterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }
}
