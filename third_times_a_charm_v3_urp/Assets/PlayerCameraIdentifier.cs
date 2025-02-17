using UnityEngine;
using Cinemachine;
using Unity.Netcode;

public class PlayerCameraIdentifier : MonoBehaviour
{
    private CinemachineFreeLook freeLookCamera;
    private Camera mainCamera;
    private CinemachineBrain cinemachineBrain;

    private void Awake()
    {
        // Try to find the CinemachineFreeLook and Main Camera components.
        freeLookCamera = GetComponent<CinemachineFreeLook>();
        mainCamera = Camera.main; // MainCamera is the camera in the scene
        cinemachineBrain = mainCamera.GetComponent<CinemachineBrain>();
    }

    private void Start()
    {
        if (freeLookCamera != null && cinemachineBrain != null)
        {
            if (IsOwner())
            {
                // Set high priority for the local player's camera
                freeLookCamera.m_Priority = 10;
            }
            else
            {
                // Set low priority for non-owner cameras
                freeLookCamera.m_Priority = 0;
            }
        }
        else
        {
            Debug.LogError("CinemachineFreeLook or CinemachineBrain not found!");
        }
    }

    // Determine if this is the local player's camera
    private bool IsOwner()
    {
        return NetworkManager.Singleton.LocalClientId == GetComponentInParent<NetworkObject>().OwnerClientId;
    }
}