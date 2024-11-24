using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public float mouseSensitivity = 100f; // Reduced sensitivity
    public float smoothTime = 0.1f; // Time to smooth the camera movement

    private float xRotation = 0f;
    private float yRotation = 0f;

    private Vector2 currentMouseDelta;
    private Vector2 currentMouseDeltaVelocity;

    void Start()
    {
        // Locking the cursor to the middle of the screen and making it invisible
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Smooth the mouse movement
        Vector2 targetMouseDelta = new Vector2(mouseX, mouseY);
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, smoothTime);

        // Control rotation around x axis (Look up and down)
        xRotation -= currentMouseDelta.y;

        // Clamp the rotation so we can't over-rotate (like in real life)
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Control rotation around y axis (Look left and right)
        yRotation += currentMouseDelta.x;

        // Apply both rotations
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}