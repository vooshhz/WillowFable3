using Mirror;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;

public class FirebaseCharacterSync : NetworkBehaviour
{
    private DatabaseReference dbRef;
    private string userId;
    private string characterId;

    public string LastSceneName { get; private set; }
    public Vector3 LastSpawnPosition { get; private set; }

    public void InitializeFirebase()
    {
        dbRef = FirebaseDatabase.GetInstance("https://willowfable3-default-rtdb.firebaseio.com/").RootReference;
    }

    public void SetupUserData()
{
    Debug.Log("[FirebaseCharacterSync] SetupUserData() called.");

    if (!isLocalPlayer)
    {
        Debug.LogWarning("[FirebaseCharacterSync] Not local player. Skipping CmdSetUserData.");
        return;
    }

    userId = FirebaseAuth.DefaultInstance.CurrentUser?.UserId;
    characterId = PlayerPrefs.GetString("SelectedCharacterId", null);

    Debug.Log($"[FirebaseCharacterSync] userId = {userId}, characterId = {characterId}");

    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(characterId))
    {
        Debug.LogWarning("[FirebaseCharacterSync] Missing userId or characterId. Aborting.");
        return;
    }

    CmdSetUserData(userId, characterId);
}


    [Command]
    private void CmdSetUserData(string newUserId, string newCharacterId)
    {
        Debug.Log($"[FirebaseCharacterSync] CmdSetUserData called with userId={newUserId}, characterId={newCharacterId}");
        if (!isServer) return;

        userId = newUserId;
        characterId = newCharacterId;

        LoadCharacterDataFromFirebase();
    }

    [Server]
    private void LoadCharacterDataFromFirebase()
    {
        dbRef.Child("users").Child(userId).Child("characters").Child(characterId).GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || !task.Result.Exists) return;

                DataSnapshot snapshot = task.Result;
                LastSceneName = snapshot.Child("location/sceneName").Value.ToString();
                float x = float.Parse(snapshot.Child("location/x").Value.ToString());
                float y = float.Parse(snapshot.Child("location/y").Value.ToString());
                float z = float.Parse(snapshot.Child("location/z").Value.ToString());
                LastSpawnPosition = new Vector3(x, y, z);

                // At the end of successful load
                EventManager.Instance.TriggerEvent(EventType.FirebaseCharacterSynced);
                Debug.Log("[FirebaseCharacterSync] Character data loaded and event triggered.");
            });
    }

    public void CmdSaveLocation(string sceneName, Vector3 position)
    {
        if (!isServer) return;

        var updates = new System.Collections.Generic.Dictionary<string, object>
        {
            [$"users/{userId}/characters/{characterId}/location/sceneName"] = sceneName,
            [$"users/{userId}/characters/{characterId}/location/x"] = position.x,
            [$"users/{userId}/characters/{characterId}/location/y"] = position.y,
            [$"users/{userId}/characters/{characterId}/location/z"] = position.z
        };

        dbRef.UpdateChildrenAsync(updates);
    }
}