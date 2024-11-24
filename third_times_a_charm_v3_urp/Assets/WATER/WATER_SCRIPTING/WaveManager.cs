using Unity.Mathematics;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;
    public float amplitude = 1f;
    public float length = 2f;
    public float speed = 1f;
    public float offset = 0f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object");
            Destroy(this);
        }
    }

    private void Update()
    {
        // Increment offset for wave motion over time
        offset += Time.deltaTime * speed;
    }

    // This method provides the wave height only along the Y axis at a specific X position
    public float GetWaveHeight(float x)
    {
        return amplitude * Mathf.Sin(x / length + offset);
    }

    // New method to provide a 3D wave height vector based on both X and Z positions
    public Vector3 GetWaveHeightVector(Vector3 position)
    {
        // Calculate Y (vertical wave height) using a sine wave
        float y = amplitude * Mathf.Sin(position.x / length + offset);

        // Calculate slight X and Z movement using a cosine wave for more natural wave patterns
        float x = amplitude * Mathf.Cos(position.z / length + offset) * 0.5f;
        float z = amplitude * Mathf.Sin(position.x / length + offset) * 0.5f;

        return new Vector3(x, y, z);
    }
}