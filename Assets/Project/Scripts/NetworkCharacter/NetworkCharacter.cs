using Mirror;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;


public class NetworkCharacter : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnEquipmentChanged))] public int headItem;
    [SyncVar(hook = nameof(OnEquipmentChanged))] public int bodyItem;
    [SyncVar(hook = nameof(OnEquipmentChanged))] public int hairItem;
    [SyncVar(hook = nameof(OnEquipmentChanged))] public int torsoItem;
    [SyncVar(hook = nameof(OnEquipmentChanged))] public int legsItem;

    [SyncVar(hook = nameof(OnStateChanged))]
    public CharacterState currentState = CharacterState.Idle;

    [SyncVar(hook = nameof(OnDirectionChanged))]
    public PlayerFacing currentDirection = PlayerFacing.Down;

    public CharacterAnimator characterAnimator; // Assign in Inspector
    private DatabaseReference dbRef;
    private string userId;
    private string characterId;
    private SceneControllerManager sceneControllerManager;

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log($"Client started with netId {netId}");
        ApplyCharacterEquipment();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log($"Server started with netId {netId}");
        dbRef = FirebaseDatabase.GetInstance("https://willowfable3-default-rtdb.firebaseio.com/").RootReference;
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Debug.Log($"Local player started with netId {netId}");

        // Start coroutine to set up Cinemachine Camera
        StartCoroutine(SetCameraFollow());

        // Only the local player should retrieve authentication info
        userId = FirebaseAuth.DefaultInstance.CurrentUser?.UserId;
        characterId = PlayerPrefs.GetString("SelectedCharacterId", null);

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(characterId))
        {
            Debug.LogError("No character ID or user is not logged in.");
            return;
        }

        // Send user data to the server for character setup
        CmdSetUserData(userId, characterId);
    }

    [Command]
    public void CmdSaveLocation(string sceneName)
    {
        if (!isServer) return;
        
        Dictionary<string, object> updates = new Dictionary<string, object>();
        
        updates[$"users/{userId}/characters/{characterId}/location/sceneName"] = sceneName;
        updates[$"users/{userId}/characters/{characterId}/location/x"] = transform.position.x;
        updates[$"users/{userId}/characters/{characterId}/location/y"] = transform.position.y;
        updates[$"users/{userId}/characters/{characterId}/location/z"] = transform.position.z;
        
        dbRef.UpdateChildrenAsync(updates);
    }    
    private IEnumerator SetCameraFollow()
    {
        // Wait until PlayerUIScene is fully loaded
        while (!SceneManager.GetSceneByName("PlayerUIScene").isLoaded)
        {
            yield return null; // Wait for next frame
        }

        // Find Cinemachine Virtual Camera
        CinemachineVirtualCamera virtualCam = FindObjectOfType<CinemachineVirtualCamera>();

        if (virtualCam != null)
        {
            virtualCam.Follow = this.transform; // Make the camera follow the local player
            Debug.Log("Virtual Camera follow target set to local player.");
        }
        else
        {
            Debug.LogError("No Cinemachine Virtual Camera found in PlayerUIScene!");
        }
    }

    [Command]
    private void CmdSetUserData(string newUserId, string newCharacterId)
    {
        if (!isServer) return;

        userId = newUserId;
        characterId = newCharacterId;

        // Now fetch character equipment from Firebase
        LoadCharacterDataFromFirebaseServer();
    }

    [Server]
    private void LoadCharacterDataFromFirebaseServer()
    {
        dbRef.Child("users").Child(userId).Child("characters").Child(characterId).GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || !task.Result.Exists)
                {
                    Debug.LogError("Error retrieving character data from Firebase.");
                    return;
                }

                DataSnapshot snapshot = task.Result;
                DataSnapshot equipmentData = snapshot.Child("equipment");

                // Read equipment
                headItem = int.Parse(equipmentData.Child("head").Value.ToString());
                bodyItem = int.Parse(equipmentData.Child("body").Value.ToString());
                hairItem = int.Parse(equipmentData.Child("hair").Value.ToString());
                torsoItem = int.Parse(equipmentData.Child("torso").Value.ToString());
                legsItem = int.Parse(equipmentData.Child("legs").Value.ToString());

                // Read scene + position
                string sceneName = snapshot.Child("location").Child("sceneName").Value.ToString();
                float x = float.Parse(snapshot.Child("location").Child("x").Value.ToString());
                float y = float.Parse(snapshot.Child("location").Child("y").Value.ToString());
                float z = float.Parse(snapshot.Child("location").Child("z").Value.ToString());
                Vector3 spawnPos = new Vector3(x, y, z);

                StartCoroutine(LoadSceneAndSpawnPlayer(sceneName, spawnPos));                
            });
    }

    public void LoadPlayerUI()
    {
<<<<<<< Updated upstream
        if (isLocalPlayer)
        {
            // Just update position instead of loading scene
            transform.position = spawnPosition;
=======
        if (!SceneManager.GetSceneByName("PlayerUIScene").isLoaded)
        {
            SceneManager.LoadScene("PlayerUIScene", LoadSceneMode.Additive);
            Debug.Log("✅ Player UI Scene loaded.");
>>>>>>> Stashed changes
        }
    }
   
    [Command]
    public void CmdChangeEquipment(int newHead, int newBody, int newHair, int newTorso, int newLegs)
    {
        if (!isServer) return;

        // Update the equipment for all clients
        headItem = newHead;
        bodyItem = newBody;
        hairItem = newHair;
        torsoItem = newTorso;
        legsItem = newLegs;

        // Save the new data to Firebase
        SaveEquipmentToFirebase(newHead, newBody, newHair, newTorso, newLegs);
    }

    [Server]
    private void SaveEquipmentToFirebase(int newHead, int newBody, int newHair, int newTorso, int newLegs)
    {
        Dictionary<string, object> updates = new Dictionary<string, object>();
        
        // Update equipment values using the new path
        updates[$"users/{userId}/characters/{characterId}/equipment/head"] = newHead;
        updates[$"users/{userId}/characters/{characterId}/equipment/body"] = newBody;
        updates[$"users/{userId}/characters/{characterId}/equipment/hair"] = newHair;
        updates[$"users/{userId}/characters/{characterId}/equipment/torso"] = newTorso;
        updates[$"users/{userId}/characters/{characterId}/equipment/legs"] = newLegs;
        
        // Execute all updates atomically
        dbRef.UpdateChildrenAsync(updates);
    }

    [Server]
    public void SaveLocationToFirebase(string sceneName, Vector3 position)
    {
        Dictionary<string, object> updates = new Dictionary<string, object>();
        
        updates[$"users/{userId}/characters/{characterId}/location/sceneName"] = sceneName;
        updates[$"users/{userId}/characters/{characterId}/location/x"] = position.x;
        updates[$"users/{userId}/characters/{characterId}/location/y"] = position.y;
        updates[$"users/{userId}/characters/{characterId}/location/z"] = position.z;
        
        dbRef.UpdateChildrenAsync(updates);
    }

    private void OnEquipmentChanged(int oldValue, int newValue)
    {
        ApplyCharacterEquipment();
    }

    private void ApplyCharacterEquipment()
    {
        if (characterAnimator == null)
        {
            Debug.LogError("CharacterAnimator component missing.");
            return;
        }

        characterAnimator.headItemNumber = headItem;
        characterAnimator.bodyItemNumber = bodyItem;
        characterAnimator.hairItemNumber = hairItem;
        characterAnimator.torsoItemNumber = torsoItem;
        characterAnimator.legsItemNumber = legsItem;


        characterAnimator.RefreshCurrentFrame();
    }

    // ----- STATE MANAGEMENT -----

    [Command]
    public void CmdUpdateState(CharacterState newState, PlayerFacing newDirection)
    {
        if (!isServer) return;

        currentState = newState;
        currentDirection = newDirection;

        RpcUpdateState(newState, newDirection);
    }

    [ClientRpc]
    private void RpcUpdateState(CharacterState newState, PlayerFacing newDirection)
    {
        ApplyCharacterState(newState, newDirection);
    }

    private void OnStateChanged(CharacterState oldState, CharacterState newState)
    {
        ApplyCharacterState(newState, currentDirection);
    }

    private void OnDirectionChanged(PlayerFacing oldDirection, PlayerFacing newDirection)
    {
        ApplyCharacterState(currentState, newDirection);
    }

    public void ApplyCharacterState(CharacterState state, PlayerFacing direction)
    {
        if (characterAnimator == null)
        {
            Debug.LogError("CharacterAnimator component missing!");
            return;
        }

        switch (state)
        {
            case CharacterState.Idle:
                characterAnimator.PlayIdle(direction);
                break;
            case CharacterState.Running:
                characterAnimator.PlayRun(direction);
                break;
        }
    }

<<<<<<< Updated upstream
    
    // private IEnumerator LoadSceneWithRetry(string sceneName, Vector3 spawnPosition)
    // {
    //     // Wait a moment for everything to initialize
    //     yield return new WaitForSeconds(0.5f);
        
    //     // Try to find the SceneControllerManager multiple times
    //     int attempts = 0;
    //     SceneControllerManager sceneController = null;
        
    //     while (attempts < 5)
    //     {
    //         sceneController = SceneControllerManager.Instance;
    //         if (sceneController != null)
    //         {
    //             break;
    //         }
            
    //         Debug.Log($"Attempt {attempts+1}: Waiting for SceneControllerManager...");
    //         yield return new WaitForSeconds(0.5f);
    //         attempts++;
    //     }
        
    //     // Use the SceneControllerManager if found
    //     if (sceneController != null)
    //     {
    //         Debug.Log("Using SceneControllerManager to load scene");
    //         sceneController.FadeAndLoadScene(sceneName, spawnPosition);
    //     }
    //     else
    //     {
    //         Debug.LogError("SceneControllerManager still not found after multiple attempts");
    //         // Fallback direct loading
    //         SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    //     }
    // }
=======
    private IEnumerator LoadSceneAndSpawnPlayer(string sceneName, Vector3 spawnPos)
{
    if (!SceneManager.GetSceneByName(sceneName).isLoaded)
    {
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!loadOp.isDone) yield return null;
    }

    // Move this player to the correct scene
    GameObject player = gameObject; // This player object
    player.transform.position = spawnPos;

    Scene targetScene = SceneManager.GetSceneByName(sceneName);
    SceneManager.MoveGameObjectToScene(player, targetScene);

    Debug.Log($"Spawned player in {sceneName} at {spawnPos}");

    LoadPlayerUI();
}

>>>>>>> Stashed changes
}