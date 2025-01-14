using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryUI; // Assign the InventoryUI GameObject in the Inspector.

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) // Check for the "I" key press.
        {
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {
        if (inventoryUI != null)
        {
            // Toggle the active state of the inventory UI.
            inventoryUI.SetActive(!inventoryUI.activeSelf);
        }
    }
}
