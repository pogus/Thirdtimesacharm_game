using System.Linq;
using UnityEngine;

public class PlaceItem : MonoBehaviour
{
    public Transform inventoryUI; // The parent object containing all inventory spaces
    public string requiredItemName; // The name/type of the required item
    public GameObject greyedOutObject; // Greyed-out placeholder object
    public GameObject placedObject; // Fully visible placed object

    private GameObject[] inventorySlots;
    private bool isPlayerNearby = false;
    void Start()
    {
        // If inventoryUI is not assigned, try to find it dynamically
        if (inventoryUI == null)
        {
            GameObject inventoryPanel = GameObject.Find("InventoryUI"); // Change "InventoryUI" to match your actual object name
            if (inventoryPanel != null)
            {
                inventoryUI = inventoryPanel.transform;
            }
            else
            {
                Debug.LogError("InventoryUI not found in the scene! Assign it manually in the inspector.");
                return;
            }
        }

        // Fetch all inventory spaces under the inventoryUI parent dynamically
        inventorySlots = GetInventorySlots();
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.P))
        {
            Place();
        }
    }

    void Place()
    {
        // Check if inventory contains the required item
        foreach (GameObject slot in inventorySlots)
        {
            if (slot.activeSelf && slot.name == requiredItemName) // Slot is active and matches the item
            {
                // Remove the item from the inventory
                slot.SetActive(false);
                slot.name = ""; // Clear the slot's name

                // Replace the greyed-out object with the placed object
                if (greyedOutObject != null)
                    greyedOutObject.SetActive(false);

                if (placedObject != null)
                    placedObject.SetActive(true);

                Debug.Log($"{requiredItemName} placed!");
                return;
            }
        }

        Debug.Log("Required item not in inventory!");
    }

    private GameObject[] GetInventorySlots()
    {
        // Fetch all child objects under the inventoryUI Transform
        Transform[] slotTransforms = inventoryUI.GetComponentsInChildren<Transform>(true);
        // Filter only the inventory slots and convert them to GameObjects
        return System.Array.FindAll(slotTransforms, t => t != inventoryUI).Select(t => t.gameObject).ToArray();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }
}
