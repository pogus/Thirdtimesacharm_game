using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    // Public variables to allow editing from the inspector
    [SerializeField] private Vector3 originalPosition;    // Editable via Inspector
    [SerializeField] private Quaternion originalRotation; // Editable via Inspector

    private void Awake()
    {
        // If not manually set via inspector, store original position and rotation when the prefab is placed in the scene
        if (originalPosition == Vector3.zero && originalRotation == Quaternion.identity)
        {
            originalPosition = transform.position;
            originalRotation = transform.rotation;
        }
    }

    public override void OnNetworkSpawn()
    {
      
            // Move the player to the original position when spawned
            transform.position = originalPosition;
            transform.rotation = originalRotation;
        
    }
}
