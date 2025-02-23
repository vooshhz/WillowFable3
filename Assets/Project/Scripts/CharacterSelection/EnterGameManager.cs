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
        if (enterGameButton == null)
        {
            Debug.LogError("Enter Game Button is not assigned!");
            return;
        }

        if (!string.IsNullOrEmpty(selectedCharacterId))
        {
            PlayerPrefs.SetString("SelectedCharacterId", selectedCharacterId);
            PlayerPrefs.Save(); // Ensure data is saved

            // Check if PersistentScene exists before trying to load it
            Scene persistentScene = SceneManager.GetSceneByName("PersistentScene");
            if (persistentScene == null)
            {
                Debug.LogError("PersistentScene is missing or not added in Build Settings.");
                return;
            }

            if (!persistentScene.isLoaded)
            {
                SceneManager.LoadScene("PersistentScene", LoadSceneMode.Single);
            }

            // Load the actual game scene (Scene_IntroScene) additively
            SceneManager.LoadScene("Scene_IntroScene", LoadSceneMode.Additive);
        }
        else
        {
            Debug.LogError("No character selected. Cannot enter the game.");
        }
    }


}
