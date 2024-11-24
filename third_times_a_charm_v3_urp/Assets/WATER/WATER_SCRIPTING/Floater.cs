using Unity.Mathematics;
using UnityEngine;

public class Floater : MonoBehaviour
{
    public Rigidbody rigidbody;
    public float depthBeforeSubmerged = 1f;
    public float displacementAmount = 3f;
    public int floaterCount = 1;
    public float waterDrag = 0.99f;
    public float waterAngularDrag = 0.5f;

    private void FixedUpdate()
    {
        // Apply gravity
        rigidbody.AddForceAtPosition(Physics.gravity / floaterCount, transform.position, ForceMode.Acceleration);

        // Get the wave height vector at the current position, including X, Y, Z adjustments
        Vector3 waveHeightVector = WaveManager.instance.GetWaveHeightVector(transform.position);
        float waveHeight = waveHeightVector.y;
        float waterHeight = WaveManager.instance.transform.position.y;

        // Check if the object is below the water surface
        if (transform.position.y < (waveHeight + waterHeight))
        {
            // Calculate displacement multiplier based on how submerged the object is
            float displacementMultiplier = Mathf.Clamp01((waterHeight + waveHeight - transform.position.y) / depthBeforeSubmerged) * displacementAmount;

            // Apply upward buoyancy force with displacement multiplier, adjusting for the Y-axis wave height
            Vector3 buoyancyForce = new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0f);
            rigidbody.AddForceAtPosition(buoyancyForce, transform.position, ForceMode.Acceleration);

            // Apply horizontal adjustments based on the wave offset on the X and Z axes
            Vector3 horizontalWaveForce = new Vector3(waveHeightVector.x, 0f, waveHeightVector.z) * displacementMultiplier*10;
            rigidbody.AddForce(horizontalWaveForce, ForceMode.Acceleration);

            // Apply water drag to reduce velocity and angular velocity over time
            rigidbody.AddForce(displacementMultiplier * -rigidbody.velocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
            rigidbody.AddTorque(displacementMultiplier * -rigidbody.angularVelocity * waterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }
}