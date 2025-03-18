using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    // For local player only
    public static Player LocalPlayer { get; private set; }

    // For tracking all players
    private static Dictionary<string, Player> _allPlayers = new Dictionary<string, Player>();

    [SyncVar(hook = nameof(OnPlayerIdChanged))]
    [SerializeField] private string playerId; // Network ID 

    private Camera mainCamera;

    // Called when playerId SyncVar changes
    void OnPlayerIdChanged(string oldId, string newId)
    {
        // Remove old ID if it exists
        if (!string.IsNullOrEmpty(oldId) && _allPlayers.ContainsKey(oldId))
            _allPlayers.Remove(oldId);
            
        // Add with new ID
        if (!string.IsNullOrEmpty(newId) && !_allPlayers.ContainsKey(newId))
            _allPlayers.Add(newId, this);
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        
        // Set as local player
        LocalPlayer = this;
        
        // Start finding camera
        StartCoroutine(FindCamera());
        
        // Request player ID from server (only for local player)
        CmdSetupPlayer();
    }
    
    [Command]
    private void CmdSetupPlayer()
    {
        // Generate or assign player ID on the server
        playerId = connectionToClient.connectionId.ToString();
        // Alternative: Use a more unique ID system if needed
    }

    public override void OnStopClient()
    {
        if (isLocalPlayer && LocalPlayer == this)
            LocalPlayer = null;
            
        base.OnStopClient();
    }

   
    {
        // Unregister from dictionary
        if (!string.IsNullOrEmpty(playerId) && _allPlayers.ContainsKey(playerId))
            _allPlayers.Remove(playerId);
    }

   
    {
        if (_allPlayers.TryGetValue(id, out Player player))
            return player;
        return null;
    }

    private IEnumerator FindCamera()
    {
        float timeOut = 3f;
        float elapsed = 0f;
        
        while (mainCamera == null && elapsed < timeOut)
        {
            if (GameCamera.Instance != null)
                mainCamera = GameCamera.Instance.mainCamera;
            
            if (mainCamera == null)
            {
                elapsed += 0.1f;
                yield return new WaitForSeconds(0.1f);
            }
        }
        
        if (mainCamera == null)
            Debug.LogError("Could not find GameCamera singleton!");
    }
    
    public Vector3 GetPlayerViewportPosition()
    {
        if (mainCamera == null)
            return new Vector3(0.5f, 0.5f, 0f); // Default to center of screen

        return mainCamera.WorldToViewportPoint(transform.position);
    }
}