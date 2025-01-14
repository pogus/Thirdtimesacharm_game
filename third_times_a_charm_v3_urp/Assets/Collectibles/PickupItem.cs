using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PickupItem : MonoBehaviour
{
    public Transform inventoryUI; // The parent object containing all inventory spaces
    public ItemData itemData; // Reference to the ItemData ScriptableObject containing the Texture2D

    private GameObject[] inventorySlots;
    private bool isPlayerNearby = false;

    void Start()
    {
        // Fetch all inventory spaces under the inventoryUI parent dynamically
        inventorySlots = GetInventorySlots();
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.P))
        {
            PickUp();
        }
    }

    void PickUp()
    {
        // Find the first available inventory slot
        foreach (GameObject slot in inventorySlots)
        {
            if (!slot.activeSelf) // If slot is inactive, it's available
            {
                slot.SetActive(true); // Activate the inventory slot
                slot.name = itemData.name; // Assign the item's name to the slot

                // Assign the Texture2D from ItemData to the slot's Image component
                if (itemData.itemImage != null)
                {
                    Image slotImage = slot.GetComponent<Image>();
                    if (slotImage != null)
                    {
                        slotImage.sprite = Sprite.Create(itemData.itemImage, new Rect(0, 0, itemData.itemImage.width, itemData.itemImage.height), new Vector2(0.5f, 0.5f));
                        Debug.Log($"Successfully updated image for {itemData.name} in {slot.name}");
                    }
                    else
                    {
                        Debug.LogWarning($"No Image component found on slot {slot.name}");
                    }
                }
                else
                {
                    Debug.LogWarning($"No Texture2D found for item {itemData.name}");
                }

                gameObject.SetActive(false); // Hide the item in the scene
                Debug.Log($"{itemData.name} added to inventory!");
                return;
            }
        }

        Debug.Log("Inventory is full!");
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
