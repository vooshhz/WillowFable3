using UnityEngine;
using UnityEngine.UI;  // Only needed if you're using UI.Button

public class EquipmentIndexButton : MonoBehaviour
{
    [Header("References")]
    // Array of EquipmentManagers that this button will update.
    // For example, you might assign the EquipmentManager on your "body" and "head" GameObjects.
    public EquipmentManager[] equipmentManagers;

    [Header("Button Settings")]
    // Set this to true for a button that decrements the index (goes left)
    // and false for a button that increments the index (goes right).
    public bool isLeft;

    // This method should be linked to the button's OnClick event.
    public void OnButtonClick()
    {
        if (equipmentManagers == null || equipmentManagers.Length == 0)
        {
            Debug.LogError("No EquipmentManagers assigned to this button.");
            return;
        }

        // Update each EquipmentManager in the array.
        foreach (EquipmentManager manager in equipmentManagers)
        {
            if (manager == null)
            {
                Debug.LogWarning("An EquipmentManager in the list is null. Skipping.");
                continue;
            }

            // Ensure that equipmentData and its items are assigned.
            if (manager.equipmentData == null ||
                manager.equipmentData.equipmentItems == null ||
                manager.equipmentData.equipmentItems.Length == 0)
            {
                Debug.LogError("EquipmentData or equipmentItems are not assigned for one of the managers.");
                continue;
            }

            int count = manager.equipmentData.equipmentItems.Length;
            int currentIndex = manager.selectedIndex;

            // If no index is selected yet, choose index 0.
            if (currentIndex == -1)
            {
                manager.selectedIndex = 0;
                continue;
            }

            // Calculate the new index using wrap-around arithmetic.
            int newIndex = isLeft
                ? (currentIndex - 1 + count) % count  // Wraps backward: if index is 0, it becomes count-1.
                : (currentIndex + 1) % count;         // Wraps forward: if index is the last one, it becomes 0.

            // Update the manager's selected index.
            manager.selectedIndex = newIndex;
        }
    }
}
