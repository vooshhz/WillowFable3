using Mirror;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using Cinemachine;
using System.Collections;

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

        // Load PlayerUIScene additively (only for this player)
        if (!SceneManager.GetSceneByName("PlayerUIScene").isLoaded)
        {
            SceneManager.LoadScene("PlayerUIScene", LoadSceneMode.Additive);
        }

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
                int newHead = int.Parse(snapshot.Child("headItemNumber").Value.ToString());
                int newBody = int.Parse(snapshot.Child("bodyItemNumber").Value.ToString());
                int newHair = int.Parse(snapshot.Child("hairItemNumber").Value.ToString());
                int newTorso = int.Parse(snapshot.Child("torsoItemNumber").Value.ToString());
                int newLegs = int.Parse(snapshot.Child("legsItemNumber").Value.ToString());

                // Set SyncVars on the server so they sync to all clients
                headItem = newHead;
                bodyItem = newBody;
                hairItem = newHair;
                torsoItem = newTorso;
                legsItem = newLegs;

                Debug.Log($"Server updated SyncVars: Head:{headItem}, Body:{bodyItem}, Hair:{hairItem}, Torso:{torsoItem}, Legs:{legsItem}");
            });
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
        dbRef.Child("users").Child(userId).Child("characters").Child(characterId).Child("headItemNumber").SetValueAsync(newHead);
        dbRef.Child("users").Child(userId).Child("characters").Child(characterId).Child("bodyItemNumber").SetValueAsync(newBody);
        dbRef.Child("users").Child(userId).Child("characters").Child(characterId).Child("hairItemNumber").SetValueAsync(newHair);
        dbRef.Child("users").Child(userId).Child("characters").Child(characterId).Child("torsoItemNumber").SetValueAsync(newTorso);
        dbRef.Child("users").Child(userId).Child("characters").Child(characterId).Child("legsItemNumber").SetValueAsync(newLegs);
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

    private void ApplyCharacterState(CharacterState state, PlayerFacing direction)
    {
        if (characterAnimator == null)
        {
            Debug.LogError("CharacterAnimator component missing!");
            return;
        }

        if (state == CharacterState.Running)
        {
            characterAnimator.PlayRun(direction);
        }
        else
        {
            characterAnimator.PlayIdle(direction);
        }
    }
}
