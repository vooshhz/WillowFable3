using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnterGameManager : MonoBehaviour
{
    [SerializeField] private Button enterGameButton; // Assign the Enter Game button in the Inspector
    [SerializeField] private string persistentScene;

    private string selectedCharacterId = null;

    private void Start()
    {
        enterGameButton.gameObject.SetActive(false); // Hide initially
        enterGameButton.onClick.AddListener(EnterGame);
    }

    public void SetSelectedCharacter(string characterId)
    {
        if (!string.IsNullOrEmpty(characterId))
        {
            selectedCharacterId = characterId;
            enterGameButton.gameObject.SetActive(true);
        }
    }

    public void EnterGame()
    {
        if (enterGameButton == null)
        {
            Debug.LogError("Enter Game Button is not assigned!");
            return;
        }

        if (string.IsNullOrEmpty(selectedCharacterId))
        {
            Debug.LogError("No character selected. Cannot enter the game.");
            return;
        }

        PlayerPrefs.SetString("SelectedCharacterId", selectedCharacterId);
        PlayerPrefs.Save();

        if (!SceneManager.GetSceneByName(persistentScene).isLoaded)
        {
            Debug.Log("PersistentScene is not loaded. Loading it now...");
            SceneManager.LoadScene(persistentScene);
        }
    }
}
