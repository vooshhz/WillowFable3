using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;

// Handles character creation button logic, including limiting characters per user
public class CreateCharacterButton : MonoBehaviour
{
    public Button createCharacterButton; // Button reference to trigger character creation (assign via Inspector)
    private DatabaseReference dbReference; // Reference to Firebase Realtime Database

    private void Start()
    {
        // Initialize database reference to the root of your Firebase DB
        dbReference = FirebaseDatabase.GetInstance("https://willowfable3-default-rtdb.firebaseio.com/").RootReference;

        // Check how many characters the current user has
        CheckCharacterLimit();

        // Add listener to the button click event
        createCharacterButton.onClick.AddListener(OnCreateCharacterButtonClicked);
    }

    // Checks if the user has fewer than 3 characters
    private void CheckCharacterLimit()
    {
        // Get the current user's UID
        string userId = FirebaseAuth.DefaultInstance.CurrentUser?.UserId;

        // If user is not logged in, log error and exit
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("User not logged in.");
            return;
        }

        // Access the user's "characters" node in the database
        dbReference.Child("users").Child(userId).Child("characters").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            // If the read is successful
            if (task.IsCompleted)
            {
                // Get snapshot of current data
                DataSnapshot snapshot = task.Result;

                // Count how many character entries exist
                int characterCount = (int)snapshot.ChildrenCount;
                
                // Log how many characters were found
                Debug.Log($"Character count: {characterCount}");

                // Enable the button only if the user has less than 3 characters
                createCharacterButton.interactable = characterCount < 3;
            }
            else
            {
                // Log if something went wrong with the database read
                Debug.LogError("Failed to check character count: " + task.Exception);
            }
        });
    }

    // Called when the create character button is clicked
    public void OnCreateCharacterButtonClicked()
    {
        // Load the character creation scene
        SceneManager.LoadScene("Scene_CreateCharacter");
    }
}
