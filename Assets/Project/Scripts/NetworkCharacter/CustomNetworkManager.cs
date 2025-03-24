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

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        // Just spawn the player, don't move or load scenes here
        GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player);
        
        Debug.Log("[Server] Player prefab instantiated and assigned to connection.");

    }

}
