using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PickupItem : MonoBehaviour
{
    private Transform inventoryUI; // Automatically assigned at runtime
    public ItemData itemData; // Reference to the ItemData ScriptableObject containing the Texture2D

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
            PickUp();
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

    void PickUp()
    {
        if (inventorySlots == null || inventorySlots.Length == 0)
        {
            Debug.LogError("No inventory slots found!");
            return;
        }

        foreach (GameObject slot in inventorySlots)
        {
            if (!slot.activeSelf)
            {
                slot.SetActive(true);
                slot.name = itemData.name;

                if (itemData.itemImage != null)
                {
                    Image slotImage = slot.GetComponent<Image>();
                    if (slotImage != null)
                    {
                        slotImage.sprite = Sprite.Create(
                            itemData.itemImage,
                            new Rect(0, 0, itemData.itemImage.width, itemData.itemImage.height),
                            new Vector2(0.5f, 0.5f)
                        );
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

                gameObject.SetActive(false);
                Debug.Log($"{itemData.name} added to inventory!");
                return;
            }
        }

        Debug.Log("Inventory is full!");
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