using UnityEngine;

public class EquipmentSelector : MonoBehaviour
{
    public CharacterAnimator characterAnimator;

    public EquipmentData bodyData;
    public EquipmentData headData;
    public EquipmentData hairData;
    public EquipmentData torsoData;
    public EquipmentData legsData;

    public void NextBody() => ChangeEquipment("body", false);
    public void PreviousBody() => ChangeEquipment("body", true);

    public void NextHead() => ChangeEquipment("head", false);
    public void PreviousHead() => ChangeEquipment("head", true);

    public void NextHair() => ChangeEquipment("hair", false);
    public void PreviousHair() => ChangeEquipment("hair", true);

    public void NextTorso() => ChangeEquipment("torso", false);
    public void PreviousTorso() => ChangeEquipment("torso", true);

    public void NextLegs() => ChangeEquipment("legs", false);
    public void PreviousLegs() => ChangeEquipment("legs", true);

    private void ChangeEquipment(string part, bool isLeft)
    {
        switch (part)
        {
            case "body":
                characterAnimator.bodyItemNumber = isLeft
                    ? GetPreviousItemNumber(bodyData, characterAnimator.bodyItemNumber)
                    : GetNextItemNumber(bodyData, characterAnimator.bodyItemNumber);
                break;
            case "head":
                characterAnimator.headItemNumber = isLeft
                    ? GetPreviousItemNumber(headData, characterAnimator.headItemNumber)
                    : GetNextItemNumber(headData, characterAnimator.headItemNumber);
                break;
            case "hair":
                characterAnimator.hairItemNumber = isLeft
                    ? GetPreviousItemNumber(hairData, characterAnimator.hairItemNumber)
                    : GetNextItemNumber(hairData, characterAnimator.hairItemNumber);
                break;
            case "torso":
                characterAnimator.torsoItemNumber = isLeft
                    ? GetPreviousItemNumber(torsoData, characterAnimator.torsoItemNumber)
                    : GetNextItemNumber(torsoData, characterAnimator.torsoItemNumber);
                break;
            case "legs":
                characterAnimator.legsItemNumber = isLeft
                    ? GetPreviousItemNumber(legsData, characterAnimator.legsItemNumber)
                    : GetNextItemNumber(legsData, characterAnimator.legsItemNumber);
                break;
        }

        characterAnimator.SetFrame(312); // Refresh display
    }

    private int GetNextItemNumber(EquipmentData data, int currentItemNumber)
    {
        for (int i = 0; i < data.equipmentItems.Length; i++)
        {
            if (data.equipmentItems[i].itemNumber == currentItemNumber)
            {
                int nextIndex = (i + 1) % data.equipmentItems.Length;
                return data.equipmentItems[nextIndex].itemNumber;
            }
        }
        return currentItemNumber;
    }

    private int GetPreviousItemNumber(EquipmentData data, int currentItemNumber)
    {
        for (int i = 0; i < data.equipmentItems.Length; i++)
        {
            if (data.equipmentItems[i].itemNumber == currentItemNumber)
            {
                int prevIndex = (i - 1 + data.equipmentItems.Length) % data.equipmentItems.Length;
                return data.equipmentItems[prevIndex].itemNumber;
            }
        }
        return currentItemNumber;
    }
}
