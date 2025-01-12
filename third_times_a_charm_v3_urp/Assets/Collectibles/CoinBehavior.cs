using UnityEngine;
using UnityEngine.Audio;

public class CoinBehavior : MonoBehaviour
{
    public float floatAmplitude = 0.5f; // How high the coin floats
    public float floatSpeed = 2f;      // How fast the coin floats up and down
    public float rotationSpeed = 100f; // Speed of rotation
    public int coinValue = 5;          // Value of the coin (5, 10, 25, 50)

    private Vector3 startPosition;
    public AudioSource audioSource;

    void Start()
    {
        // Save the initial position
        startPosition = transform.position;
 
    }

    void Update()
    {
        // Floating effect
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Rotation effect
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(coinValue);

                // Play coin collection sound
                if (audioSource != null)
                {
                    audioSource.Play();
                    Debug.Log("played");

                }
            }
            Destroy(gameObject);
        }
    }
}
