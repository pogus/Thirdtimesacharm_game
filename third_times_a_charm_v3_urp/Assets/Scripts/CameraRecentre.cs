using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraRecentre : MonoBehaviour
{
    private CinemachineFreeLook Camera;

    // Zoom variables
    [SerializeField] private float zoomSpeed = 1f;  // Speed of zoom adjustment
    [SerializeField] private float minZoom = 2f;   // Minimum zoom (closest)
    [SerializeField] private float maxZoom = 10f;  // Maximum zoom (farthest)

    // Start is called before the first frame update
    void Start()
    {
        Camera = GetComponent<CinemachineFreeLook>();
    }

    // Update is called once per frame
    void Update()
    {
        // Handle recentring
        if (Input.GetAxis("CameraRecentre") == 1)
        {
            Camera.m_RecenterToTargetHeading.m_enabled = true;
        }
        else
        {
            Camera.m_RecenterToTargetHeading.m_enabled = false;
        }

        // Handle zooming
        HandleZoom();
    }

    void HandleZoom()
    {
        // Get scroll wheel input
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0f)
        {
            // Adjust the middle rig radius (affects zoom)
            float newRadius = Camera.m_Orbits[1].m_Radius - scrollInput * zoomSpeed;

            // Clamp the radius to stay within min and max zoom
            newRadius = Mathf.Clamp(newRadius, minZoom, maxZoom);

            // Apply the new radius to all orbits (top, middle, bottom)
            for (int i = 0; i < Camera.m_Orbits.Length; i++)
            {
                Camera.m_Orbits[i].m_Radius = newRadius;
            }
        }
    }
}