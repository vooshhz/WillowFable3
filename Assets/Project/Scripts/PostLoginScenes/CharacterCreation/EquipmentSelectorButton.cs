using UnityEngine;

public class EquipmentSelector : MonoBehaviour
{
    public CharacterAnimator characterAnimator; // Reference to the character animator that updates visuals
    public SO_EquipmentData bodyData;  // ScriptableObject containing body equipment options
    public SO_EquipmentData headData;  // ScriptableObject containing head equipment options
    public SO_EquipmentData hairData;  // ScriptableObject containing hair equipment options
    public SO_EquipmentData torsoData; // ScriptableObject containing torso equipment options
    public SO_EquipmentData legsData;  // ScriptableObject containing leg equipment options

    public void NextBody() => ChangeEquipment("body", false); // go to next body item
    public void PreviousBody() => ChangeEquipment("body", true); // go to previous body item

    public void NextHead() => ChangeEquipment("head", false); // go to next head item
    public void PreviousHead() => ChangeEquipment("head", true); // go to previous head item

    public void NextHair() => ChangeEquipment("hair", false); // go to next hair item
    public void PreviousHair() => ChangeEquipment("hair", true); // go to previous hair item

    public void NextTorso() => ChangeEquipment("torso", false); // go to next torso item
    public void PreviousTorso() => ChangeEquipment("torso", true); // go to previous torso item

    public void NextLegs() => ChangeEquipment("legs", false); // go to next legs item
    public void PreviousLegs() => ChangeEquipment("legs", true);  // go to previous legs item

    private void ChangeEquipment(string part, bool isLeft) // Handles changing equipment based on part and direction
    {
        switch (part) // Decide which part is being changed 
        {
            case "body":
                characterAnimator.bodyItemNumber = isLeft // changes body item number left or right depending on button
                    ? GetPreviousItemNumber(bodyData, characterAnimator.bodyItemNumber)
                    : GetNextItemNumber(bodyData, characterAnimator.bodyItemNumber);
                break;
            case "head":
                characterAnimator.headItemNumber = isLeft
                    ? GetPreviousItemNumber(headData, characterAnimator.headItemNumber)// changes head item number left or right depending on button
                    : GetNextItemNumber(headData, characterAnimator.headItemNumber);
                break;
            case "hair":
                characterAnimator.hairItemNumber = isLeft
                    ? GetPreviousItemNumber(hairData, characterAnimator.hairItemNumber) // changes hair item number left or right depending on button
                    : GetNextItemNumber(hairData, characterAnimator.hairItemNumber);
                break;
            case "torso":
                characterAnimator.torsoItemNumber = isLeft
                    ? GetPreviousItemNumber(torsoData, characterAnimator.torsoItemNumber) // changes torso item number left or right depending on button
                    : GetNextItemNumber(torsoData, characterAnimator.torsoItemNumber);
                break;
            case "legs":
                characterAnimator.legsItemNumber = isLeft
                    ? GetPreviousItemNumber(legsData, characterAnimator.legsItemNumber) // changes legs item number left or right depending on button
                    : GetNextItemNumber(legsData, characterAnimator.legsItemNumber);
                break;
        }

        characterAnimator.RefreshCurrentFrame(); ; // Refresh display
    }

    // Returns the itemNumber of the next equipment item in the list
    private int GetNextItemNumber(SO_EquipmentData data, int currentItemNumber)
    {
        // Loop through all equipment items in the given equipment data
        for (int i = 0; i < data.equipmentItems.Length; i++)
        {
            // If the current item in the array matches the current equipped item number
            if (data.equipmentItems[i].itemNumber == currentItemNumber)
            {
                // Calculate the next index, wrapping around to 0 if we're at the end
                int nextIndex = (i + 1) % data.equipmentItems.Length;
                
                // Return the itemNumber of the next equipment item
                return data.equipmentItems[nextIndex].itemNumber;
            }
        }
        // If the current item number wasn't found in the list (edge case), just return the current one
        return currentItemNumber;
    }

    // Returns the itemNumber of the previous equipment item in the list
    private int GetPreviousItemNumber(SO_EquipmentData data, int currentItemNumber)
    {
        // Loop through all equipment items in the given equipment data
        for (int i = 0; i < data.equipmentItems.Length; i++)
        {
            // If the current item in the array matches the current equipped item number
            if (data.equipmentItems[i].itemNumber == currentItemNumber)
            {
                // Calculate the previous index, wrapping around to the end if we're at the start
                int prevIndex = (i - 1 + data.equipmentItems.Length) % data.equipmentItems.Length;
                
                // Return the itemNumber of the previous equipment item
                return data.equipmentItems[prevIndex].itemNumber;
            }
        }
        
        // If the current item number wasn't found in the list (edge case), just return the current one
        return currentItemNumber;
    }
}
