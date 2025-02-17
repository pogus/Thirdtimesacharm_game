using UnityEngine;

public class IceLanceThrower : MonoBehaviour
{
    public GameObject iceLancePrefab; // Reference to the lance prefab
    public ParticleSystem iceTrail;  // Reference to the ice trail particle system
    public Transform spawnPoint;    // Where the lance spawns
    public float throwSpeed = 20f;  // Speed of the throw

    private GameObject currentLance; // Stores the active lance
    private ParticleSystem currentTrail; // Stores the active ice trail

    void Update()
    {
        // Check if Q is pressed
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ThrowIceLance();
        }
    }

    void ThrowIceLance()
    {
        // If a lance already exists, destroy it before creating a new one
        if (currentLance != null)
        {
            Destroy(currentLance);
        }

        if (currentTrail != null)
        {
            Destroy(currentTrail.gameObject);
        }

        // Spawn the lance (hidden) at the spawn point
        currentLance = Instantiate(iceLancePrefab, spawnPoint.position, spawnPoint.rotation);
        currentLance.GetComponent<Renderer>().enabled = false; // Hide the lance

        // Play the ice trail
        currentTrail = Instantiate(iceTrail, spawnPoint.position, spawnPoint.rotation);
        currentTrail.Play();

        // Add force to simulate throwing
        Rigidbody rb = currentLance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = spawnPoint.forward * throwSpeed;
        }

        // Destroy the lance after a short delay (e.g., 2 seconds)
        Destroy(currentLance, 2f);

        // Destroy the ice trail after it finishes
        Destroy(currentTrail.gameObject, currentTrail.main.duration + currentTrail.main.startLifetime.constantMax);
    }
}