using UnityEngine;

public class PlayerMovement : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        originalStepOffset = characterController.stepOffset;

        // Automatically find the main camera
        if (cameraTransform == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
            }
            else
            {
                Debug.LogError("Main Camera not found! Make sure your scene has a camera tagged as 'MainCamera'.");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Movement direction based on input
        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        float inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);

        // By default, use the walking speed
        float currentSpeed = moveSpeed;

        // If Shift is pressed, allow running
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentSpeed = runSpeed;

            // Allow inputMagnitude to go over 0.66 (running animation trigger)
            animator.SetFloat("InputMagnitude", inputMagnitude, 0.05f, Time.deltaTime);
        }
        else
        {
            // Cap inputMagnitude at 0.66 for walking animation
            inputMagnitude = Mathf.Clamp(inputMagnitude, 0, 0.66f);
            animator.SetFloat("InputMagnitude", inputMagnitude, 0.05f, Time.deltaTime);
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
            animator.SetBool("IsGrounded", true);
            isGrounded = true;
            animator.SetBool("IsJumping", false);
            isJumping = false;
            animator.SetBool("IsFalling", false);

            if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod)
            {
                ySpeed = Mathf.Sqrt(jumpHeight * -3 * gravity);
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
            Vector3 velocity = movementDirection * inputMagnitude * jumpHorizontalSpeed * currentSpeed * 50;
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