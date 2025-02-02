using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private void Awake()
    {
        // Store original position and rotation when the prefab is placed in the scene
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            // Move the player to the original position when spawned
            transform.position = originalPosition;
            transform.rotation = originalRotation;
        }
    }
}