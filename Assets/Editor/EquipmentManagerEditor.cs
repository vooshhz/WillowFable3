using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EquipmentManager))]
public class EquipmentManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw default inspector
        DrawDefaultInspector();

        EquipmentManager manager = (EquipmentManager)target;

        if (manager.equipmentData != null && manager.equipmentData.equipmentItems.Length > 0)
        {
            string[] itemNames = new string[manager.equipmentData.equipmentItems.Length];
            for (int i = 0; i < itemNames.Length; i++)
            {
                itemNames[i] = manager.equipmentData.equipmentItems[i].itemName;
            }

            // Dropdown to select an item
            int newIndex = EditorGUILayout.Popup("Select Item", manager.selectedIndex, itemNames);

            if (newIndex != manager.selectedIndex)
            {
                manager.selectedIndex = newIndex;
            }

            if (GUILayout.Button("Update All Animations"))
            {
                manager.RefreshAllAnimations(); // Trigger updates for all animation clips
            }
        }
        else
        {
            EditorGUILayout.HelpBox("No Equipment Data assigned or no items available.", MessageType.Warning);
        }

        if (manager.animator == null)
        {
            EditorGUILayout.HelpBox("Please assign an Animator.", MessageType.Warning);
        }

        if (manager.animationClips == null || manager.animationClips.Length == 0)
        {
            EditorGUILayout.HelpBox("Please assign at least one Animation Clip.", MessageType.Warning);
        }
    }
}
