using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneControllerManager : MonoBehaviour
{
    private static SceneControllerManager _instance;

    // Public property to access the instance
    public static SceneControllerManager Instance => _instance;
    private bool isFading;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private CanvasGroup faderCanvasGroup = null;
    [SerializeField] private Image faderImage = null;
    public SceneName startingSceneName;
    private void Awake()
    {
        // If an instance already exists and it's not this one
        if (_instance != null && _instance != this)
        {
            // Destroy this instance
            Destroy(gameObject);
            return;
        }
        
        // Set this as the current instance
        _instance = this;
    }
    
    private void OnDestroy()
{
    // Clear the reference if this is the instance being destroyed
    if (_instance == this)
    {
        _instance = null;
    }
}
    private IEnumerator Fade(float finalAlpha)
    {
        // Set the fading flag to true so the FadeAndSwitchScenes coroutine won't be called again.
        isFading = true;

        // Make sure the CanvasGroup blocks raycasts into the scene so no more input can be accepted.
        faderCanvasGroup.blocksRaycasts = true;

        // Calculate how fast the CanvasGroup should fade based on it's current alpha, it's final alpha and how long it has to change between the two.
        float fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - finalAlpha) / fadeDuration;

        // While the CanvasGroup hasn't reached the final alpha yet...
        while (!Mathf.Approximately(faderCanvasGroup.alpha, finalAlpha))
        {
            // ... move the alpha towards it's target alpha.
            faderCanvasGroup.alpha = Mathf.MoveTowards(faderCanvasGroup.alpha, finalAlpha,
                fadeSpeed * Time.deltaTime);

            // Wait for a frame then continue.
            yield return null;
        }

        // Set the flag to false since the fade has finished.
        isFading = false;

        // Stop the CanvasGroup from blocking raycasts so input is no longer ignored.
        faderCanvasGroup.blocksRaycasts = false;
    }

    // This is the coroutine where the 'building blocks' of the script are put together.
    private IEnumerator FadeAndSwitchScenes(string sceneName, Vector3 spawnPosition)
    {
        // Start fading to black and wait for it to finish before continuing.
        yield return StartCoroutine(Fade(1f));

        // Set player position - Note: You'll need to modify this for your network approach
        // Instead of using a singleton Player.Instance, you should pass a reference to the player
        // or use a different method to position the player in a networked context
        
        // We'll need to replace this line with your network player positioning approach
        // Player.Instance.gameObject.transform.position = spawnPosition;

        // Unload the current active scene.
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        // Start loading the given scene and wait for it to finish.
        yield return StartCoroutine(LoadSceneAndSetActive(sceneName));

        // Start fading back in and wait for it to finish before exiting the function.
        yield return StartCoroutine(Fade(0f));
    }

    private IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        // Allow the given scene to load over several frames and add it to the already loaded scenes (just the Persistent scene at this point).
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        // Find the scene that was most recently loaded (the one at the last index of the loaded scenes).
        Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

        // Set the newly loaded scene as the active scene (this marks it as the one to be unloaded next).
        SceneManager.SetActiveScene(newlyLoadedScene);
    }

    private IEnumerator Start()
    {
        // Set the initial alpha to start off with a black screen.
        faderImage.color = new Color(0f, 0f, 0f, 1f);
        faderCanvasGroup.alpha = 1f;

        // Start the first scene loading and wait for it to finish.
        yield return StartCoroutine(LoadSceneAndSetActive(startingSceneName.ToString()));

        // Once the scene is finished loading, start fading in.
        StartCoroutine(Fade(0f));
    }

    // This is the main external point of contact and influence from the rest of the project.
    // This will be called when the player wants to switch scenes.
    public void FadeAndLoadScene(string sceneName, Vector3 spawnPosition)
    {
        // If a fade isn't happening then start fading and switching scenes.
        if (!isFading)
        {
            StartCoroutine(FadeAndSwitchScenes(sceneName, spawnPosition));
        }
    }
}