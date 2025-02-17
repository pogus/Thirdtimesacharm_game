using UnityEngine;
using UnityEngine.VFX;
using System.Collections; // Required for IEnumerator

public class VFXHoldActivator1 : MonoBehaviour
{
    public VisualEffect vfxComponent1; // The Visual Effect component to control.
    private bool isDelaying = false; // Prevents multiple delays running simultaneously

    void Start()
    {
        if (vfxComponent1 == null)
        {
            // Debug.LogError("VFX Component not assigned! Assign it in the Inspector.");
        }
    }

    void Update()
    {

        // Check if the "E" key is held down
        if (Input.GetKey(KeyCode.R))
        {
            if (!vfxComponent1.enabled && !isDelaying)
            {
                StartCoroutine(ActivateVFXWithDelay(0)); // Add a small delay (e.g., 0.2 seconds)
            }
        }
        else
        {
            if (vfxComponent1.enabled)
            {
                vfxComponent1.enabled = false;
                //Debug.Log("VFX Deactivated.");
            }
        }
    }

    private IEnumerator ActivateVFXWithDelay(float delay)
    {
        isDelaying = true; // Set delay flag to true
        yield return new WaitForSeconds(delay); // Wait for the delay

        if (Input.GetKey(KeyCode.R)) // Ensure "E" is still pressed
        {
            vfxComponent1.enabled = true;
            //Debug.Log("VFX Activated.");
        }

        isDelaying = false; // Reset delay flag
    }
}