using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        // Ensure Scene_IntroScene is loaded before spawning the player
        Scene gameScene = SceneManager.GetSceneByName("Scene_IntroScene");

        if (!gameScene.isLoaded)
        {
            SceneManager.LoadScene("Scene_IntroScene", LoadSceneMode.Additive);
        }

        // Spawn the player at a default spawn position (adjust as needed)
        GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        // Move the player to the correct game scene
        SceneManager.MoveGameObjectToScene(player, gameScene);

        // Add the player to the network
        NetworkServer.AddPlayerForConnection(conn, player);

        Debug.Log($"Spawned player in scene: {gameScene.name}");
    }
}
