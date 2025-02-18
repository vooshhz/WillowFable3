using UnityEngine;

public class CharacterPreviewController : MonoBehaviour
{
    public CharacterAnimator characterAnimator;
    public CharacterEquipmentData equipmentData; // Just for holding the data

    public void ApplyEquipmentData()
    {
        if (equipmentData != null && characterAnimator != null)
        {
            equipmentData.ApplyToCharacterAnimator(characterAnimator);
        }
        else
        {
            Debug.LogError("CharacterAnimator or EquipmentData is missing in CharacterPreviewController!");
        }
    }

    public void SetEquipmentData(CharacterEquipmentData data)
    {
        equipmentData = data;
        ApplyEquipmentData(); // Apply immediately when setting data
    }
}
