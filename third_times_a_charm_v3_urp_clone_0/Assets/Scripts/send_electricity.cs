using UnityEngine;
using UnityEngine.VFX;
using System.Collections; // Required for IEnumerator

public class VFXHoldActivator : MonoBehaviour
{
    public VisualEffect vfxComponent; // The Visual Effect component to control.
    private bool isDelaying = false; // Prevents multiple delays running simultaneously

    void Start()
    {
        if (vfxComponent == null)
        {
           // Debug.LogError("VFX Component not assigned! Assign it in the Inspector.");
        }
    }

    void Update()
    {

        // Check if the "E" key is held down
        if (Input.GetKey(KeyCode.E))
        {
            if (!vfxComponent.enabled && !isDelaying)
            {
                StartCoroutine(ActivateVFXWithDelay(1.2f)); // Add a small delay (e.g., 0.2 seconds)
            }
        }
        else
        {
            if (vfxComponent.enabled)
            {
                vfxComponent.enabled = false;
                //Debug.Log("VFX Deactivated.");
            }
        }
    }

    private IEnumerator ActivateVFXWithDelay(float delay)
    {
        isDelaying = true; // Set delay flag to true
        yield return new WaitForSeconds(delay); // Wait for the delay

        if (Input.GetKey(KeyCode.E)) // Ensure "E" is still pressed
        {
            vfxComponent.enabled = true;
             //Debug.Log("VFX Activated.");
        }

        isDelaying = false; // Reset delay flag
    }
}