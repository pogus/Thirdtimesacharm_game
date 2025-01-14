using UnityEngine;
using UnityEngine.UI;

public class BoatController : MonoBehaviour
{
    public GameObject player; // Reference to the player object
    public GameObject interactionIcon; // The icon that says "Press K to enter"
    public Transform wheel; // Reference to the boat's wheel
    public GameObject[] boatParts; // Array of boat parts to check
    public float rotationSpeed = 10f; // Speed at which the wheel rotates
    public float boatSpeed = 5f; // Base speed of the boat
    public float reverseSpeed = 2f; // Speed at which the boat reverses
    public float decelerationRate = 2f; // Rate at which the boat slows down

    private bool isDriving = false; // Is the player currently driving the boat
    private CharacterController playerController; // Reference to the player's character controller
    private Rigidbody playerRigidbody; // Rigidbody for the player
    private Vector3 initialPlayerLocalPosition; // Stores player's local position relative to the boat
    private Vector3 initialIconLocalPosition; // Stores interaction icon's local position relative to the boat
    private float currentSpeed = 0f; // Current forward or reverse speed of the boat

    void Start()
    {
        // Ensure the interaction icon is initially hidden
        if (interactionIcon != null)
        {
            interactionIcon.SetActive(false);
            initialIconLocalPosition = interactionIcon.transform.localPosition; // Store initial icon position
        }

        // Get the player's character controller and add a Rigidbody if needed
        if (player != null)
        {
            playerController = player.GetComponent<CharacterController>();

            // Add a Rigidbody for collision if none exists
            playerRigidbody = player.GetComponent<Rigidbody>();
            if (playerRigidbody == null)
            {
                playerRigidbody = player.AddComponent<Rigidbody>();
                playerRigidbody.isKinematic = true; // Keep it kinematic to prevent physics interference
            }
        }

        // Ensure the wheel's static rotation values
        if (wheel != null)
        {
            wheel.localRotation = Quaternion.Euler(-90f, -90f, 90f);
        }
    }

    void Update()
    {
        // Check if all boat parts are active
        bool allPartsActive = true;
        foreach (GameObject part in boatParts)
        {
            if (part != null && !part.activeSelf)
            {
                allPartsActive = false;
                break;
            }
        }

        // Check player distance to the wheel
        if (player != null && wheel != null && allPartsActive)
        {
            float distanceToWheel = Vector3.Distance(player.transform.position, wheel.position);

            // Show interaction icon if close enough to the wheel and not driving
            if (distanceToWheel <= 3f && !isDriving)
            {
                interactionIcon.SetActive(true);

                // Check for "K" key press to start driving
                if (Input.GetKeyDown(KeyCode.K))
                {
                    EnterDrivingMode();
                }
            }
            else
            {
                interactionIcon.SetActive(false);
            }
        }

        // Handle driving mode
        if (isDriving)
        {
            HandleBoatControls();

            // Exit driving mode
            if (Input.GetKeyDown(KeyCode.P))
            {
                ExitDrivingMode();
            }
        }

        // Ensure interaction icon follows boat
        if (interactionIcon != null)
        {
            interactionIcon.transform.localPosition = initialIconLocalPosition;
        }
    }

    void HandleBoatControls()
    {
        // Accelerate the boat forward when W is pressed
        if (Input.GetKey(KeyCode.W))
        {
            currentSpeed += boatSpeed * Time.deltaTime;
        }
        // Decelerate or reverse the boat when S is pressed
        else if (Input.GetKey(KeyCode.S))
        {
            currentSpeed -= reverseSpeed * Time.deltaTime;
        }
        else
        {
            // Gradually decelerate the boat when no key is pressed
            if (currentSpeed > 0)
            {
                currentSpeed = Mathf.Max(0, currentSpeed - decelerationRate * Time.deltaTime);
            }
            else if (currentSpeed < 0)
            {
                currentSpeed = Mathf.Min(0, currentSpeed + decelerationRate * Time.deltaTime);
            }
        }

        // Apply forward or reverse movement
        if (currentSpeed != 0f)
        {
            transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
        }

        // Rotate the boat and wheel when A or D is pressed
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
            RotateWheel(rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            RotateWheel(-rotationSpeed * Time.deltaTime);
        }

        // Keep the player following the boat
        if (player != null)
        {
            player.transform.position = transform.TransformPoint(initialPlayerLocalPosition);
        }
    }

    void RotateWheel(float rotationAmount)
    {
        if (wheel != null)
        {
            Vector3 currentRotation = wheel.localEulerAngles;
            wheel.localEulerAngles = new Vector3(currentRotation.x + rotationAmount, -90f, 90f);
        }
    }

    void EnterDrivingMode()
    {
        isDriving = true;

        // Disable player controller
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Enable Rigidbody for collision handling
        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = true; // Keep it kinematic to prevent physics interference
        }

        // Lock the player's position relative to the boat
        if (player != null)
        {
            initialPlayerLocalPosition = transform.InverseTransformPoint(player.transform.position);
        }

        // Hide interaction icon
        interactionIcon.SetActive(false);
    }

    void ExitDrivingMode()
    {
        isDriving = false;

        // Enable player controller
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        // Stop the boat
        currentSpeed = 0f;
    }
}
