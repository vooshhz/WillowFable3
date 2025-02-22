using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnterGameManager : MonoBehaviour
{
    public Button enterGameButton; // Assign the Enter Game button in the Inspector

    private string selectedCharacterId = null;

    private void Start()
    {
        // Ensure the button is initially hidden
        enterGameButton.gameObject.SetActive(false);

        // Attach click event listener
        enterGameButton.onClick.AddListener(EnterGame);
    }

    public void SetSelectedCharacter(string characterId)
    {
        if (!string.IsNullOrEmpty(characterId))
        {
            selectedCharacterId = characterId;
            enterGameButton.gameObject.SetActive(true); // Show button when character is selected
        }
    }

    private void EnterGame()
    {
        if (!string.IsNullOrEmpty(selectedCharacterId))
        {
            PlayerPrefs.SetString("SelectedCharacterId", selectedCharacterId);
            PlayerPrefs.Save(); // Ensure data is saved
            SceneManager.LoadScene("Scene_IntroScene"); // Replace with actual game scene
        }
        else
        {
            Debug.LogError("No character selected. Cannot enter the game.");
        }
    }
}
