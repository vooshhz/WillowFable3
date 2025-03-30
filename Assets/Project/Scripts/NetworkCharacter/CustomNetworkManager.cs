using System;
using Mirror;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    public static bool IsNetworkManagerReady { get; private set; }
    public override void Start()
    {
        base.Start();
        EventManager.Instance.Subscribe(EventType.PersistentSceneLoaded, OnPersistentSceneLoaded_NetworkManager);
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        // Just spawn the player, don't move or load scenes here
        GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player);
        Debug.Log("[Server] Player prefab instantiated and assigned to connection.");
    }

        private void OnPersistentSceneLoaded_NetworkManager()
    {
        EventManager.Instance.Unsubscribe(EventType.PersistentSceneLoaded, OnPersistentSceneLoaded_NetworkManager);
        Debug.Log("Triggering NetworkManagerReady AFTER PersistentSceneLoaded...");
        IsNetworkManagerReady = true;
        EventManager.Instance.TriggerEvent(EventType.NetworkManagerReady);
    }
}
