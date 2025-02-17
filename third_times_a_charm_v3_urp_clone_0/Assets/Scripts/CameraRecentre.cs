using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;

public class CameraRecentre : NetworkBehaviour
{
    private CinemachineFreeLook Camera;
    private Transform playerTransform;
    private int playerID;

    // Zoom variables
    [SerializeField] private float zoomSpeed = 1f;
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 100f;

    void Start()
    {
        if (IsLocalPlayer) // Check if this is the local player's instance
        {
            // You can assign a unique player ID based on the networked instance.
            playerID = (int)NetworkObject.NetworkObjectId; // Use NetworkObjectId or assign a custom ID
        }

        Camera = GetComponent<CinemachineFreeLook>();

        if (Camera != null)
        {
            //Camera.Follow = transform;  // Set Follow to player transform (this will allow the camera to follow the player)
            //Camera.LookAt = transform;  // Set LookAt to player transform (this ensures the camera always looks at the player)
        }

        if (IsLocalPlayer) // Only local player should have control over the camera
        {
            // Ensure the camera only follows the local player and disables for others
            Camera.enabled = true;
            FindAndAssignPlayer();
        }
        else
        {
            Camera.enabled = false; // Disable the camera on non-local players
        }
    }

    void FindAndAssignPlayer()
    {
        if (IsOwner) // Check if this is the local player's instance
        {
            // Find the player dynamically (Ensure the player has the name "player_object_final")
            GameObject playerObj = transform.parent != null && transform.parent.name == "player_object_final" ? transform.parent.gameObject : null;
            //GameObject playerObj = transform.name == "Player_Object" ? transform.gameObject : null;
            if (playerObj != null)
            {
            playerTransform = playerObj.transform;
            AssignPlayerToCamera();
            }
        else
        {
            Debug.LogError("Player not found! Make sure the player prefab is spawned and has the 'Player_Object' name.");
        }
    }
    }

    void AssignPlayerToCamera()
    {
        if (IsLocalPlayer) // Check if this is the local player's instance
        {
            if (Camera != null && playerTransform != null)
            {
                //Camera.Follow = playerTransform;
                //Camera.LookAt = playerTransform;

                // Make sure each orbit (Top, Middle, Bottom Rig) is looking at the player
                Camera.m_Orbits[0].m_Height = Camera.m_Orbits[0].m_Height;
                Camera.m_Orbits[1].m_Height = Camera.m_Orbits[1].m_Height;
                Camera.m_Orbits[2].m_Height = Camera.m_Orbits[2].m_Height;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsLocalPlayer) return; // Only allow the local player to control the camera

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
        if (!IsLocalPlayer) return; // Only allow the local player to control the camera

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
