using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClassSelectionManager : MonoBehaviour
{
    [Header("TMP Buttons")]
    public Button warriorButton;
    public Button luminaryButton;
    public Button magicianButton;
    public Button archerButton;

    [Header("Button Colors")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.green;

    private string selectedClass;

    private void Start()
    {
        warriorButton.onClick.AddListener(() => SelectClass("Warrior", warriorButton));
        luminaryButton.onClick.AddListener(() => SelectClass("Luminary", luminaryButton));
        magicianButton.onClick.AddListener(() => SelectClass("Magician", magicianButton));
        archerButton.onClick.AddListener(() => SelectClass("Archer", archerButton));

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

    private void ResetButtonColors()
    {
        SetButtonColor(warriorButton, normalColor);
        SetButtonColor(luminaryButton, normalColor);
        SetButtonColor(magicianButton, normalColor);
        SetButtonColor(archerButton, normalColor);
    }

    private void SetButtonColor(Button button, Color color)
    {
        var colors = button.colors;
        colors.normalColor = color;
        colors.selectedColor = color;
        colors.highlightedColor = color;
        colors.pressedColor = color;
        colors.disabledColor = color;
        button.colors = colors;
    }

    public string GetSelectedClass()
    {
        return selectedClass;
    }
}
