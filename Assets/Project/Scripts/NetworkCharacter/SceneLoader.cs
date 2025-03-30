using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader _instance;
    
    public static SceneLoader Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("SceneLoader");
                _instance = go.AddComponent<SceneLoader>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        EventManager.Instance.Subscribe(EventType.InventoryInitialized, LoadPlayerInitialScene); 
        EventManager.Instance.Subscribe(EventType.PlayerEquipmentApplied, LoadUIScene); 
    }

    void OnDestroy()
    {
        EventManager.Instance.Unsubscribe(EventType.InventoryInitialized, LoadPlayerInitialScene); 
        EventManager.Instance.Unsubscribe(EventType.PlayerEquipmentApplied, LoadUIScene); 
    }
    private void LoadPlayerInitialScene()
    {
        if (NetworkClient.localPlayer == null)
        {
            Debug.LogError("[SceneLoader] Local player not found.");
            return;
        }

        var networkChar = NetworkClient.localPlayer.GetComponent<NetworkCharacter>();
        var sync = networkChar.GetComponent<FirebaseCharacterSync>();

        string sceneName = sync.LastSceneName;
        Vector3 spawnPos = sync.LastSpawnPosition;

        StartCoroutine(LoadSceneAndPlacePlayer(sceneName, spawnPos));
    }

    private IEnumerator LoadSceneAndPlacePlayer(string sceneName, Vector3 spawnPos)
    {
        yield return LoadSceneAdditiveCoroutine(sceneName, EventType.PlayerSceneLoaded);

        // Wait one frame to ensure scene is fully active
        yield return null;

        GameObject player = NetworkClient.localPlayer.gameObject;
        player.transform.position = spawnPos;

        Scene scene = SceneManager.GetSceneByName(sceneName);
        SceneManager.MoveGameObjectToScene(player, scene);

        EventManager.Instance.TriggerEvent(EventType.PlayerInstantiated);
        Debug.Log("[SceneLoader] Player moved to scene and event triggered.");
    }

    // Load a scene additively and raise appropriate events
    public void LoadSceneAdditive(string sceneName, EventType loadedEventType)
    {
        StartCoroutine(LoadSceneAdditiveCoroutine(sceneName, loadedEventType));
    }  
    private IEnumerator LoadSceneAdditiveCoroutine(string sceneName, EventType loadedEventType)
    {
        // Trigger scene transition start event
        EventManager.Instance.TriggerEvent(EventType.BeginSceneTransition);
        
        // Load the scene additively
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        asyncOperation.allowSceneActivation = true;
        
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
        
        // Scene is now loaded, set it active
        Scene loadedScene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(loadedScene);
        
        // Trigger the specific event for this scene's loading completion
        EventManager.Instance.TriggerEvent(loadedEventType);
        
        // Trigger general scene transition complete event
        EventManager.Instance.TriggerEvent(EventType.SceneTransitionComplete);
        
        Debug.Log($"Scene loaded: {sceneName}");
    }

    private void LoadUIScene()
    {
            StartCoroutine(LoadSceneAdditiveCoroutineWithoutSetActive("PlayerUIScene", EventType.PlayerUISceneLoaded));
    }

    private IEnumerator LoadSceneAdditiveCoroutineWithoutSetActive(string sceneName, EventType loadedEventType)
    {
        // Trigger scene transition start event
        EventManager.Instance.TriggerEvent(EventType.BeginSceneTransition);
        
        // Load the scene additively
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        asyncOperation.allowSceneActivation = true;
        
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
        
        // Scene is now loaded, set it active
        Scene loadedScene = SceneManager.GetSceneByName(sceneName);
        
        // Trigger the specific event for this scene's loading completion
        EventManager.Instance.TriggerEvent(loadedEventType);
        
        // Trigger general scene transition complete event
        EventManager.Instance.TriggerEvent(EventType.SceneTransitionComplete);
        
        Debug.Log($"Scene loaded: {sceneName}");
    }
    public void LoadScene(string sceneName, EventType loadedEventType)
        {
            StartCoroutine(LoadSceneCoroutine(sceneName, loadedEventType));
        }

    private IEnumerator LoadSceneCoroutine(string sceneName, EventType loadedEventType)
    {
        // Trigger scene transition start event
        EventManager.Instance.TriggerEvent(EventType.BeginSceneTransition);
        
        // Load the scene 
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = true;
        
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
        
        // Trigger scene-specific and general transition complete events
        EventManager.Instance.TriggerEvent(loadedEventType);
        EventManager.Instance.TriggerEvent(EventType.SceneTransitionComplete);
    }

}