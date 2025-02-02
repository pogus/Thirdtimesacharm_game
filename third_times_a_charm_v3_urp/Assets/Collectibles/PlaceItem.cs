using System.Linq;
using UnityEngine;

public class PlaceItem : MonoBehaviour
{
    private Transform inventoryUI; // Automatically assigned at runtime
    public string requiredItemName; // The name/type of the required item
    public GameObject greyedOutObject; // Greyed-out placeholder object
    public GameObject placedObject; // Fully visible placed object

    private GameObject[] inventorySlots;
    private bool isPlayerNearby = false;

    void Start()
    {
        AssignInventoryUI();
        inventorySlots = GetInventorySlots();
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.P))
        {
            Place();
        }
    }

    void AssignInventoryUI()
    {
        if (inventoryUI == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>(); // Look for an active Canvas
            if (canvas != null)
            {
                Transform foundInventory = canvas.transform.Find("InventoryUI");
                if (foundInventory != null)
                {
                    inventoryUI = foundInventory;
                    Debug.Log("InventoryUI automatically assigned.");
                }
                else
                {
                    Debug.LogError("InventoryUI object not found in the Canvas! Ensure it exists in the hierarchy.");
                }
            }
            else
            {
                Debug.LogError("No Canvas found in the scene! Inventory UI assignment failed.");
            }
        }
    }

    void Place()
    {
        if (inventorySlots == null || inventorySlots.Length == 0)
        {
            Debug.LogError("No inventory slots found!");
            return;
        }

        foreach (GameObject slot in inventorySlots)
        {
            if (slot.activeSelf && slot.name == requiredItemName)
            {
                slot.SetActive(false);
                slot.name = "";

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
        if (inventoryUI == null)
        {
            Debug.LogError("InventoryUI is not assigned! Cannot fetch inventory slots.");
            return new GameObject[0];
        }

        return inventoryUI.GetComponentsInChildren<Transform>(true)
            .Where(t => t != inventoryUI)
            .Select(t => t.gameObject)
            .ToArray();
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
