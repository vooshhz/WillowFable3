using UnityEngine;
using UnityEngine.UI;

public class ClassSelectionManager : MonoBehaviour
{
    [Header("TMP Buttons")]
    public Button warriorButton; // UI button to select Warrior class
    public Button luminaryButton; // UI button to select Luminary class
    public Button magicianButton; // UI button to select Magician class
    public Button archerButton; // UI button to select Archer class

    [Header("Button Colors")]
    public Color normalColor = Color.white; // Default button color
    public Color selectedColor = Color.green; // Color when button is selected

    private string selectedClass; // Stores the currently selected class as a string

    private void Start()
    {
        // Assign click listeners to each button 
        warriorButton.onClick.AddListener(() => SelectClass("Warrior", warriorButton));
        luminaryButton.onClick.AddListener(() => SelectClass("Luminary", luminaryButton));
        magicianButton.onClick.AddListener(() => SelectClass("Magician", magicianButton));
        archerButton.onClick.AddListener(() => SelectClass("Archer", archerButton));

        // Reset all buttons to default (unselected) color at start
        ResetButtonColors();
    }

    private void SelectClass(string className, Button selectedButton)
    {
        selectedClass = className;
        ResetButtonColors();
        SetButtonColor(selectedButton, selectedColor);

        Debug.Log("Selected Class: " + selectedClass);
        // You can now use selectedClass later for Firebase storage
    }

    // Resets all buttons to the normal color (unselected state)
    private void ResetButtonColors()
    {
        SetButtonColor(warriorButton, normalColor);
        SetButtonColor(luminaryButton, normalColor);
        SetButtonColor(magicianButton, normalColor);
        SetButtonColor(archerButton, normalColor);
    }

    // Applies a color to all visual states of a button
    private void SetButtonColor(Button button, Color color)
    {
        var colors = button.colors;            // Get the button's color settings
        colors.normalColor = color;            // Color when not interacting
        colors.selectedColor = color;          // Color when selected
        colors.highlightedColor = color;       // Color when hovered
        colors.pressedColor = color;           // Color when clicked
        colors.disabledColor = color;          // Color when button is disabled
        button.colors = colors;                // Apply the updated color settings back to the button
    }


    // Public getter for accessing the selected class from other scripts
    public string GetSelectedClass()
    {
        return selectedClass;
    }
}
