using UnityEngine;

[CreateAssetMenu(fileName = "NewEquipmentData", menuName = "Equipment/Equipment Data")]
public class EquipmentData : ScriptableObject
{
    [System.Serializable]
    public class EquipmentItem
    {
        public enum ItemType
        {
            body,
            torso,
            pants,
            head,
            hair,
            weapon
        }

        public ItemType itemType;           // The type of the item (body, torso, pants)
        public int itemNumber;              // Unique identifier for the item
        public string itemName;             // The name of the item
        public Sprite[] slicedSpritesArray; // Array of sprites for this item
    }

    [Header("Equipment Items")]
    public EquipmentItem[] equipmentItems; // Array of all equipment items
}