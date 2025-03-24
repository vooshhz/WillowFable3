using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Firebase.Database;

// public class CustomNetworkManager : NetworkManager
// {
//     private DatabaseReference dbRef;
//     public override void Awake()
//     {
//         base.Awake();
//         dbRef = FirebaseDatabase.GetInstance("https://willowfable3-default-rtdb.firebaseio.com/").RootReference;
//     }

//     [SerializeField] private string playerSpawnScene = "Scene_IntroScene"; // Editable in Inspector

//     public override void OnServerAddPlayer(NetworkConnectionToClient conn)
//     {
//         StartCoroutine(EnsureSceneLoaded(conn));
//     }

//     private IEnumerator EnsureSceneLoaded(NetworkConnectionToClient conn)
//     {
//         string sceneName = playerSpawnScene; // Use the scene name from the Inspector

//         if (!SceneManager.GetSceneByName(sceneName).isLoaded)
//         {
//             SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
//             Debug.Log($"Loading {sceneName}...");

//             while (!SceneManager.GetSceneByName(sceneName).isLoaded)
//             {
//                 yield return null;
//             }
//         }

//         // Now that the scene is loaded, spawn the player
//         SpawnPlayer(conn);
//     }

//     private void SpawnPlayer(NetworkConnectionToClient conn)
//     {
//         GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

//         // Move player to the configured scene
//         SceneManager.MoveGameObjectToScene(player, SceneManager.GetSceneByName(playerSpawnScene));

//         NetworkServer.AddPlayerForConnection(conn, player);
//         Debug.Log($"Spawned player in scene: {playerSpawnScene}");
//     }

public class CustomNetworkManager : NetworkManager
{
    private DatabaseReference dbRef;
    
    public override void Awake()
    {
        base.Awake();
        dbRef = FirebaseDatabase.GetInstance("https://willowfable3-default-rtdb.firebaseio.com/").RootReference;
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        StartCoroutine(SpawnPlayerInCorrectScene(conn));
    }

    private IEnumerator SpawnPlayerInCorrectScene(NetworkConnectionToClient conn)
    {
        // Get the user ID from authentication data (you'll need to pass this when connecting)
        string userId = conn.authenticationData as string;
        if (string.IsNullOrEmpty(userId))
        {
            // Fallback to default scene if no authentication
            SpawnInDefaultScene(conn);
            yield break;
        }
        
        // Get character ID from connection data or from a connection dictionary
        string characterId = GetCharacterIdForConnection(conn);
        if (string.IsNullOrEmpty(characterId))
        {
            SpawnInDefaultScene(conn);
            yield break;
        }
        
        // Query Firebase for the character's last location
        bool dataRetrieved = false;
        string sceneToLoad = "Scene_IntroScene";
        Vector3 spawnPosition = Vector3.zero;
        
        dbRef.Child("users").Child(userId).Child("characters").Child(characterId)
            .Child("location").GetValueAsync().ContinueWith(task => {
                if (task.IsCompleted && !task.IsFaulted && task.Result.Exists)
                {
                    DataSnapshot snapshot = task.Result;
                    sceneToLoad = snapshot.Child("sceneName").Value?.ToString() ?? "Scene_IntroScene";
                    float x = float.Parse(snapshot.Child("x").Value?.ToString() ?? "0");
                    float y = float.Parse(snapshot.Child("y").Value?.ToString() ?? "0");
                    float z = float.Parse(snapshot.Child("z").Value?.ToString() ?? "0");
                    spawnPosition = new Vector3(x, y, z);
                }
                dataRetrieved = true;
            });
        
        // Wait for Firebase query to complete
        while (!dataRetrieved)
            yield return null;
            
        // Load scene if needed
        if (!SceneManager.GetSceneByName(sceneToLoad).isLoaded)
        {
            SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
            
            while (!SceneManager.GetSceneByName(sceneToLoad).isLoaded)
                yield return null;
        }
        
        // Spawn player in correct scene at correct position
        GameObject player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(player, SceneManager.GetSceneByName(sceneToLoad));
        NetworkServer.AddPlayerForConnection(conn, player);
    }
    
    private void SpawnInDefaultScene(NetworkConnectionToClient conn)
    {
        // Fallback spawning logic
        string defaultScene = "Scene_IntroScene";
        
        if (!SceneManager.GetSceneByName(defaultScene).isLoaded)
            SceneManager.LoadScene(defaultScene, LoadSceneMode.Additive);
            
        GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(player, SceneManager.GetSceneByName(defaultScene));
        NetworkServer.AddPlayerForConnection(conn, player);
    }
    
    private string GetCharacterIdForConnection(NetworkConnectionToClient conn)
    {
        // For now, we use the static PlayerPrefs approach
        return PlayerPrefs.GetString("SelectedCharacterId", null);
    }
}
