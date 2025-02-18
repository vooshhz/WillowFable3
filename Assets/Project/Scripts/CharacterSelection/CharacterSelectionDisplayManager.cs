using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;

public class CharacterSelectionSpawner : MonoBehaviour
{
    public GameObject characterSelection1Prefab;
    public GameObject characterSelection2Prefab;
    public GameObject characterSelection3Prefab;

    private DatabaseReference dbReference;

    void Start()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        LoadCharacterCount(userId);
    }

    void LoadCharacterCount(string userId)
    {
        dbReference.Child("users").Child(userId).Child("characters").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to retrieve character data: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;
            int characterCount = (int)snapshot.ChildrenCount;

            Debug.Log("Character count: " + characterCount);
            SpawnCharacterSelections(characterCount);
        });
    }

    void SpawnCharacterSelections(int count)
    {
        if (count >= 1)
        {
            Instantiate(characterSelection1Prefab, new Vector3(-100f, 0f, 0f), Quaternion.identity);
        }
        if (count >= 2)
        {
            Instantiate(characterSelection2Prefab, new Vector3(-80f, 0f, 0f), Quaternion.identity);
        }
        if (count >= 3)
        {
            Instantiate(characterSelection3Prefab, new Vector3(-60f, 0f, 0f), Quaternion.identity);
        }
    }
}
