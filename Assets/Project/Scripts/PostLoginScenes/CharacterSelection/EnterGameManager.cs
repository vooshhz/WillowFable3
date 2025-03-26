using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnterGameManager : MonoBehaviour
{
    [SerializeField] private Button enterGameButton; // Reference to the "Enter Game" button set in the Inspector
    [SerializeField] private string persistentScene; // Name of the scene to load (e.g., main world, persistent gameplay scene)

    private string selectedCharacterId = null; // Stores the ID of the character the player selected

    private void Start()
    {
        enterGameButton.gameObject.SetActive(false); // Hide the "Enter Game" button at start
        enterGameButton.onClick.AddListener(EnterGame); // Add the EnterGame function to the button's onClick event
    }

    // Called externally when a character is selected from the UI
    public void SetSelectedCharacter(string characterId)
    {
        if (!string.IsNullOrEmpty(characterId)) // Check if the provided ID is valid
        {
            selectedCharacterId = characterId; // Store selected character ID
            enterGameButton.gameObject.SetActive(true); // Enable the "Enter Game" button now that a character is selected
        }
    }

    // Called when the "Enter Game" button is clicked
    public void EnterGame()
    {
        if (enterGameButton == null) // Safety check in case the button wasn't assigned
        {
            Debug.LogError("Enter Game Button is not assigned!");
            return;
        }

        if (string.IsNullOrEmpty(selectedCharacterId)) // Make sure a character is selected
        {
            Debug.LogError("No character selected. Cannot enter the game.");
            return;
        }

        // Save the selected character ID using PlayerPrefs so it's available in the next scene
        PlayerPrefs.SetString("SelectedCharacterId", selectedCharacterId);
        PlayerPrefs.Save(); // Ensure data is written to disk

        // If the persistent scene is not already loaded, load it now
        if (!SceneManager.GetSceneByName(persistentScene).isLoaded)
        {
            Debug.Log("PersistentScene is not loaded. Loading it now...");
            SceneManager.LoadScene(persistentScene); // Load the main game scene
        }
    }
}
