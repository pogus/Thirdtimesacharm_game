using UnityEditor;
using UnityEngine;
using System.IO;

public class ItemDataEditor
{
    [MenuItem("Assets/Create/Inventory/Item (Custom Name)")]
    public static void CreateCustomItem()
    {
        // Open the custom input window
        ItemCreationWindow.ShowWindow();
    }
}

public class ItemCreationWindow : EditorWindow
{
    private string customName = "NewItem";
    private Texture2D itemImage;

    public static void ShowWindow()
    {
        var window = GetWindow<ItemCreationWindow>("Create Item");
        window.minSize = new Vector2(300, 200);
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Create a New Item", EditorStyles.boldLabel);

        // Input field for the item name
        customName = EditorGUILayout.TextField("Item Name:", customName);

        // Field for assigning an image
        itemImage = (Texture2D)EditorGUILayout.ObjectField("Item Image:", itemImage, typeof(Texture2D), false);

        GUILayout.Space(10);

        if (GUILayout.Button("Create"))
        {
            CreateItem();
            Close();
        }

        if (GUILayout.Button("Cancel"))
        {
            Close();
        }
    }

    private void CreateItem()
    {
        string defaultPath = "Assets/Data/Items";

        // Ensure the folder exists
        if (!AssetDatabase.IsValidFolder(defaultPath))
        {
            string[] folders = defaultPath.Split('/');
            string currentPath = "Assets";
            foreach (var folder in folders)
            {
                if (!AssetDatabase.IsValidFolder(currentPath + "/" + folder))
                {
                    AssetDatabase.CreateFolder(currentPath, folder);
                }
                currentPath += "/" + folder;
            }
        }

        // Validate the name
        if (string.IsNullOrWhiteSpace(customName))
        {
            Debug.LogWarning("Item creation failed: Invalid or empty name.");
            return;
        }

        // Create the new ItemData asset
        ItemData newItem = ScriptableObject.CreateInstance<ItemData>();
        newItem.itemName = customName;
        newItem.itemImage = itemImage;

        string finalPath = Path.Combine(defaultPath, $"{customName}.asset").Replace("\\", "/");
        AssetDatabase.CreateAsset(newItem, finalPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Created new ItemData asset at: {finalPath}");
    }
}

