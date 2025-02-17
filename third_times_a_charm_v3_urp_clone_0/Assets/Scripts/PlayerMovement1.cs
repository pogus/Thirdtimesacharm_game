using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField]
    private float rotationSpeed;

    [SerializeField]
    private float jumpHeight;

    [SerializeField]
    private float gravityMultiplier;

    [SerializeField]
    private float jumpHorizontalSpeed;

    [SerializeField]
    private float jumpButtonGracePeriod;

    [SerializeField]
    private float moveSpeed; // Walking speed

    [SerializeField]
    private float runSpeed; // Running speed (when Shift is pressed)

    [SerializeField]
    private Transform cameraTransform;

    [SerializeField]
    private float forceMagnitude = 2.0f; // Push strength for pushing objects

    private Animator animator;
    private CharacterController characterController;
    private float ySpeed;
    private float originalStepOffset;
    private float? lastGroundedTime;
    private float? jumpButtonPressedTime;
    private bool isJumping;
    private bool isGrounded;
    GameObject playerCamera = null;

    // Networked variables to sync the player's position across all clients
    private NetworkVariable<Vector3> networkedPosition = new NetworkVariable<Vector3>(
        writePerm: NetworkVariableWritePermission.Owner
    );

    private NetworkVariable<Quaternion> networkedRotation = new NetworkVariable<Quaternion>(
        writePerm: NetworkVariableWritePermission.Owner
    );

    private NetworkVariable<float> inputMagnitude = new NetworkVariable<float>(
        writePerm: NetworkVariableWritePermission.Owner
    );


    // Start is called before the first frame update
    void Start()
    {
        if (IsLocalPlayer) { 

            animator = GetComponent<Animator>();
            characterController = GetComponent<CharacterController>();
            originalStepOffset = characterController.stepOffset;

            AssignCamera();

            // Initialize the position to sync with the server
            networkedPosition.Value = transform.position;
            networkedRotation.Value = transform.rotation;
        }


    }

    void AssignCamera()
    {
        if (IsLocalPlayer)
        {
            playerCamera = GameObject.Find("MainCamera");
            if (playerCamera != null)
            {
                playerCamera.SetActive(true);
                cameraTransform = playerCamera.transform;
                Debug.Log("PlayerCamera assigned successfully.");
            }
            else
            {
                Debug.LogError("PlayerCamera not found. Ensure it's a direct child of the player.");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        Debug.Log("Player movement script running on: " + IsOwner);
        if (!IsLocalPlayer) return;

        // Ensure camera is assigned before using it
        if (cameraTransform == null)
        {
            AssignCamera();
            if (cameraTransform == null) return; // Avoid errors if no camera found
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Movement direction based on input
        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        float localInputMagnitude = Mathf.Clamp01(movementDirection.magnitude);

        // By default, use the walking speed
        float currentSpeed = moveSpeed;

        // If Shift is pressed, allow running
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentSpeed = runSpeed;

            // Allow inputMagnitude to go over 0.66 (running animation trigger)
            animator.SetFloat("InputMagnitude", localInputMagnitude, 0.05f, Time.deltaTime);
        }
        else
        {
            // Cap inputMagnitude at 0.66 for walking animation
            localInputMagnitude = Mathf.Clamp(localInputMagnitude, 0, 0.66f);
            animator.SetFloat("InputMagnitude", localInputMagnitude, 0.05f, Time.deltaTime);
        }

        // Rotate movement direction to match camera orientation
        movementDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
        movementDirection.Normalize();

        float gravity = Physics.gravity.y * gravityMultiplier;

        // Double gravity when jump is released mid-air
        if (isJumping && ySpeed > 0 && !Input.GetButton("Jump"))
        {
            gravity *= 2;
        }

        ySpeed += gravity * Time.deltaTime;

        // Check if grounded
        if (characterController.isGrounded)
        {
            lastGroundedTime = Time.time;
        }

        // Handle jump input
        if (Input.GetButtonDown("Jump"))
        {
            jumpButtonPressedTime = Time.time;
        }

        // Grace period for jumping after landing
        if (Time.time - lastGroundedTime <= jumpButtonGracePeriod)
        {
            characterController.stepOffset = originalStepOffset;
            ySpeed = -0.5f;
            if (animator != null && animator.layerCount > 0)
            {
                animator.SetBool("IsGrounded", true);
                animator.SetBool("IsJumping", false);
                animator.SetBool("IsFalling", false);
            }
            isGrounded = true;
            isJumping = false;

            if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod)
            {
                ySpeed = Mathf.Sqrt(jumpHeight * -3 * gravity);
                if (animator != null && animator.layerCount > 0)
                    animator.SetBool("IsJumping", true);
                isJumping = true;
                jumpButtonPressedTime = null;
                lastGroundedTime = null;
            }
        }
        else
        {
            characterController.stepOffset = 0;
            animator.SetBool("IsGrounded", false);
            isGrounded = false;

            if ((isJumping && ySpeed < 0) || ySpeed < -2)
            {
                animator.SetBool("IsFalling", true);
            }
        }

        // Rotate character based on movement direction
        if (movementDirection != Vector3.zero)
        {
            animator.SetBool("IsMoving", true);
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }

        // When not grounded, apply movement in the air with horizontal jump speed
        if (!isGrounded)
        {
            Vector3 velocity = movementDirection * localInputMagnitude * jumpHorizontalSpeed * currentSpeed * 50;
            velocity.y = ySpeed;

            characterController.Move(velocity * Time.deltaTime);
        }

        // Handle "2h_attack" animation
        if (Input.GetKey(KeyCode.E))
        {
            animator.SetBool("2h_attack", true); // Trigger animation
        }
        else
        {
            animator.SetBool("2h_attack", false); // Reset animation
        }


        // Sync position only if it has changed significantly (e.g., more than 0.1 units)
        if (Vector3.Distance(networkedPosition.Value, transform.position) > 0.1f)
        {
            networkedPosition.Value = transform.position;
        }

        // Sync rotation only if it has changed significantly (e.g., more than 1 degree)
        if (Quaternion.Angle(networkedRotation.Value, transform.rotation) > 1f)
        {
            networkedRotation.Value = transform.rotation;
        }

        // Sync input magnitude only if it has changed significantly (e.g., more than 0.05 difference)
        float currentMagnitude = animator.GetFloat("InputMagnitude");
        if (Mathf.Abs(inputMagnitude.Value - currentMagnitude) > 0.05f)
        {
            inputMagnitude.Value = currentMagnitude;
        }
    }

    // Synchronize position, rotation, and input magnitude for non-owners
    private void OnNetworkedPositionChanged(Vector3 oldValue, Vector3 newValue)
    {
        if (!IsOwner)
        {
            transform.position = newValue;
        }
    }

    private void OnNetworkedRotationChanged(Quaternion oldValue, Quaternion newValue)
    {
        if (!IsOwner)
        {
            transform.rotation = newValue;
        }
    }

    private void OnNetworkedInputMagnitudeChanged(float oldValue, float newValue)
    {
        if (!IsOwner && animator != null)
        {
            animator.SetFloat("InputMagnitude", newValue);
        }
    }

    // Start observing the networked variables
    private void OnEnable()
    {
        networkedPosition.OnValueChanged += OnNetworkedPositionChanged;
        networkedRotation.OnValueChanged += OnNetworkedRotationChanged;
        inputMagnitude.OnValueChanged += OnNetworkedInputMagnitudeChanged;
    }

    // Stop observing the networked variables
    private void OnDisable()
    {
        networkedPosition.OnValueChanged -= OnNetworkedPositionChanged;
        networkedRotation.OnValueChanged -= OnNetworkedRotationChanged;
        inputMagnitude.OnValueChanged -= OnNetworkedInputMagnitudeChanged;
    }

    // Move character when grounded, applying movement speed
    private void OnAnimatorMove()
    {
        if (isGrounded)
        {
            Vector3 velocity = animator.deltaPosition;

            // Apply the current speed based on whether walking or running
            velocity = velocity.normalized * moveSpeed * animator.GetFloat("InputMagnitude");

            velocity.y = ySpeed * Time.deltaTime;

            characterController.Move(velocity);
        }
    }

    // Lock cursor during gameplay
    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        var rigidBody = hit.collider.attachedRigidbody;

        if (rigidBody != null)
        {
            var forceDirection = hit.gameObject.transform.position - transform.position;
            forceDirection.y = 0;
            forceDirection.Normalize();

            rigidBody.AddForceAtPosition(forceDirection * forceMagnitude, transform.position, ForceMode.Impulse);


        }
    }
}