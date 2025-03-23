using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class EnterGameManager : MonoBehaviour
{
    [SerializeField] private Button enterGameButton; // Assign the Enter Game button in the Inspector
    [SerializeField] private string persistentScene;
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

            // Start loading scenes in sequence
            StartCoroutine(LoadScenesSequentially());
        }
        else
        {
            Debug.LogError("No character selected. Cannot enter the game.");
        }
    }

    private IEnumerator LoadScenesSequentially()
    {
        yield return LoadSceneIfNotLoaded(persistentScene);
    }

    private IEnumerator LoadSceneIfNotLoaded(string scene)
    {
        string sceneName = GetSceneName(scene);
        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            Debug.Log($"Loading {sceneName}...");

            // Wait until the scene is fully loaded
            while (!SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                yield return null;
            }
        }
    }

    private string GetSceneName(string scene)
    {
        return scene != null ? scene : "";
    }
}
