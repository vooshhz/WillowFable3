using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;

public class CreateCharacterButton : MonoBehaviour
{
    public Button createCharacterButton; // Assign this in the Inspector
    private DatabaseReference dbReference;

    private void Start()
    {
        dbReference = FirebaseDatabase.GetInstance("https://willowfable3-default-rtdb.firebaseio.com/").RootReference;
        CheckCharacterLimit();
        createCharacterButton.onClick.AddListener(OnCreateCharacterButtonClicked);
    }

    private void CheckCharacterLimit()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser?.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("User not logged in.");
            return;
        }

        dbReference.Child("users").Child(userId).Child("characters").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                int characterCount = (int)snapshot.ChildrenCount;

                Debug.Log($"Character count: {characterCount}");

                // Enable/Disable button based on character count
                createCharacterButton.interactable = characterCount < 3;
            }
            else
            {
                Debug.LogError("Failed to check character count: " + task.Exception);
            }
        });
    }

    public void OnCreateCharacterButtonClicked()
    {
        SceneManager.LoadScene("Scene_CreateCharacter");
    }
}
