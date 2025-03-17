using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using Mirror;

public class SavedEnterGameManager : MonoBehaviour
{
    [SerializeField] private Button enterGameButton; // Assign the Enter Game button in the Inspector
    [SerializeField] private string persistentScene;
    [SerializeField] private string playerUIScene;
    [SerializeField] private string introScene;

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

        if (string.IsNullOrEmpty(selectedCharacterId))
        {
            Debug.LogError("No character selected. Cannot enter the game.");
            return;
        }

        PlayerPrefs.SetString("SelectedCharacterId", selectedCharacterId);
        PlayerPrefs.Save();

        // Ensure PersistentScene is loaded before connecting
        if (!SceneManager.GetSceneByName("PersistentScene").isLoaded)
        {
            Debug.Log("PersistentScene is not loaded. Loading it now...");
            StartCoroutine(LoadPersistentSceneBeforeConnecting());
            return; // Stop execution here, let the coroutine handle the connection
        }

        // Now connect to the server
        ConnectToServer();
    }

    // Coroutine to wait until PersistentScene is loaded
    private IEnumerator LoadPersistentSceneBeforeConnecting()
    {
        // Check if PersistentScene is already loaded
        if (SceneManager.GetSceneByName("PersistentScene").isLoaded)
        {
            Debug.Log("PersistentScene is already loaded. Skipping load...");
            ConnectToServer(); // Move to the next step
            yield break;
        }

        Debug.Log("Loading PersistentScene...");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("PersistentScene", LoadSceneMode.Additive);

        // Ensure the scene starts loading
        if (asyncLoad == null)
        {
            Debug.LogError("Failed to start loading PersistentScene!");
            yield break;
        }

        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            Debug.Log($"Waiting for PersistentScene to load... Progress: {asyncLoad.progress}");
            yield return null;
        }

        Debug.Log("PersistentScene loaded successfully!");

        // Ensure NetworkManager exists before proceeding
        while (NetworkManager.singleton == null)
        {
            Debug.Log("Waiting for NetworkManager to initialize...");
            yield return null;
        }

        Debug.Log("NetworkManager found! Connecting to server...");
        ConnectToServer();
    }



    // Handles connecting the client to the server
    private void ConnectToServer()
    {
        if (NetworkManager.singleton == null)
        {
            Debug.LogError("NetworkManager singleton is null! Make sure NetworkManager exists in the scene.");
            return;
        }

        // Ensure the client connects to the correct EC2 IP address
        NetworkManager.singleton.networkAddress = "44.202.86.167"; // EC2 Server IP

        if (!NetworkClient.isConnected)
        {
            Debug.Log("Attempting to connect to the dedicated server at " + NetworkManager.singleton.networkAddress);
            NetworkManager.singleton.StartClient();
        }
        else
        {
            Debug.Log("Already connected to the server.");
        }
    }





    private IEnumerator LoadScenesSequentially()
    {
        yield return LoadSceneIfNotLoaded(persistentScene);
        yield return LoadSceneIfNotLoaded(playerUIScene);
        yield return LoadSceneIfNotLoaded(introScene);
    }

    private IEnumerator LoadSceneIfNotLoaded(string scene)
    {
        string sceneName = GetSceneName(scene);

        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            Debug.Log($"Loading {sceneName}...");
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);

            while (!SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                Debug.Log($"Waiting for {sceneName} to load... Progress: {SceneManager.GetSceneByName(sceneName).isLoaded}");
                yield return null;
            }

            Debug.Log($" {sceneName} successfully loaded!");
        }
    }


    private string GetSceneName(string scene)
    {
        return scene != null ? scene : "";
    }
}
