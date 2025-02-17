using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FloatingObject : MonoBehaviour
{
    public Material waterMaterial; // Reference to the water material

    public float floatStrength = 1.0f; // Strength of buoyancy
    public float dampingFactor = 0.5f; // Dampening factor to reduce oscillation
    public float objectHeightOffset = 1.0f; // Offset from the water surface

    private Rigidbody rb;

    // Shader variables
    private float noiseScale;
    private float waterHeight;
    private float waveSpeed;
    private Vector2 waveDirection;
    private float waveStrength;
    private float waveDensity;
    private float waterScrollSpeed;
    private float waveheight;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        UpdateShaderVariables();
    }

    void FixedUpdate()
    {
        UpdateShaderVariables();

        // Calculate water height at the object's position
        float waterHeightAtPosition = CalculateWaveHeight(transform.position);

        // Determine submersion depth
        float submergedDepth = waterHeightAtPosition - transform.position.y + objectHeightOffset;

        if (submergedDepth > 0)
        {
            // Calculate buoyancy force proportional to submerged depth
            Vector3 buoyantForce = Vector3.up * (submergedDepth * floatStrength);

            // Apply damping to reduce vertical oscillation
            Vector3 dampingForce = -rb.velocity * dampingFactor;

            // Combine buoyant force and damping
            rb.AddForce(buoyantForce + dampingForce, ForceMode.Acceleration);
        }
    }

    private void UpdateShaderVariables()
    {
        if (waterMaterial != null)
        {
            noiseScale = waterMaterial.GetFloat("Noise_Scale");
            waterHeight = waterMaterial.GetFloat("Water_Height");
            waveSpeed = waterMaterial.GetFloat("wave_speed");
            waveDirection = waterMaterial.GetVector("wave_direction");
            waveStrength = waterMaterial.GetFloat("wave_strength");
            waterScrollSpeed = waterMaterial.GetFloat("Water_Scroll_Speed");
            waveheight = waterMaterial.GetFloat("wave_height");
            waveDensity = waterMaterial.GetFloat("wave_density");
        }
    }

    private float CalculateWaveHeight(Vector3 position)
    {
        // **1. Octave Noise Calculation**
        float noiseValue = 0.0f;
        float factor = 1.0f;

        for (int i = 0; i < 6; i++) // Octave factors: 1, 2, 4, 8, 16, 32
        {
            float octaveNoise = Mathf.PerlinNoise(
                position.x * noiseScale * factor + Time.time * waterScrollSpeed,
                position.z * noiseScale * factor + Time.time * waterScrollSpeed
            );
            noiseValue += octaveNoise;
            factor *= 2.0f; // Increase factor for next octave
        }

        noiseValue *= waterHeight;

        // **2. Wave Calculation**
        float waveTime = waveSpeed * Time.time;
        Vector2 waveUV = waveDirection * waveTime;

        float simpleNoise = Mathf.PerlinNoise(waveUV.x, waveUV.y) * waveStrength;
        float voronoiNoise = Mathf.Clamp01((Mathf.Sin(waveUV.x) + Mathf.Cos(waveUV.y)) * waveDensity * waveStrength);

        float waveValue = (simpleNoise + voronoiNoise) / waveheight;

        // Combine with Octave Noise
        float finalHeight = noiseValue + waveValue;

        return finalHeight;
    }
}