using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class CharacterSaveManager : MonoBehaviour
{
    public CharacterAnimator characterAnimator; // reference the animator component 
    public ClassSelectionManager classSelectionManager; // reference the class selection manager 
    public TMP_InputField characterNameInput; // reference the character name input
    public TMP_Text errorMessageText; // reference the message

    private DatabaseReference dbReference; // reference to the firebase database 

    private void Start()
    {
        // Initialize the Firebase database reference
        dbReference = FirebaseDatabase.GetInstance("https://willowfable3-default-rtdb.firebaseio.com/").RootReference;
    }

    // Called when the player tries to save a new character
    public void SaveCharacterData()
    {
        // Get the currently logged-in Firebase user
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        
        // If the read operation failed, log the error and exit
        if (user == null)
            {
                Debug.LogError("User is not logged in.");
                return;
            }


    string userId = user.UserId; // Get the unique ID of the currently logged-in Firebase user
    string characterName = characterNameInput.text; // Get the character name entered by the user in the input field

    // Check if the user already has characters (or same name exists)
    dbReference.Child("users").Child(userId).Child("characters")
        .GetValueAsync().ContinueWithOnMainThread(task =>
        {
            // If the read operation failed, log the error and exit
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to check existing characters: " + task.Exception);
                return;
            }
            
            // If the read operation completed successfully
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result; // Snapshot of all user's characters
                
                // Count characters
                int characterCount = 0; // Track number of valid characters
                bool nameExists = false; // Track if this name is already taken

                // Only count valid character entries (must have core data sections)
                foreach (DataSnapshot characterSnapshot in snapshot.Children)
                {
                    // Check if this is a valid character node
                    if (characterSnapshot.HasChild("info") && 
                        characterSnapshot.HasChild("equipment") && 
                        characterSnapshot.HasChild("inventory"))
                    {
                        characterCount++; //Count this as one valid character

                        // Get the existing character's name from the snapshot
                        string existingName = characterSnapshot.Child("info").Child("characterName").Value?.ToString();

                        // Check if the name already exists (case-sensitive)
                        if (existingName == characterName)
                        {
                            nameExists = true;
                        }
                    }
                }

                // Enforce a maximum of 3 characters per user
                if (characterCount >= 3)
                {
                    Debug.LogWarning("Maximum character limit reached.");
                    errorMessageText.text = "Maximum of 3 characters allowed.";
                    return;
                }
                
                // Show an error if a character with the same name exists
                if (nameExists)
                {
                    Debug.LogWarning("Character name already exists. Choose a different name.");
                    errorMessageText.text = "Character name already exists. Choose another name.";
                }
                else
                {
                    // If all checks pass, proceed to create and save the new character
                    CreateNewCharacter(userId, characterName);
                }
            }
        });
    }
    private void CreateNewCharacter(string userId, string characterName)
    {
        // Generate a unique key for this new character under the user's "characters" node
        string characterKey = dbReference.Child("users").Child(userId).Child("characters").Push().Key;
        
        // Define the base character info (class, name, level, XP, and timestamp)
        Dictionary<string, object> characterInfo = new Dictionary<string, object>
        {
            { "characterName", characterName },
            { "characterClass", classSelectionManager.GetSelectedClass() },
            { "level", 1 },
            { "experience", 0 },
            { "createdAt", ServerValue.Timestamp }
        };

        // Define the starting scene and coordinates (used for spawning the character)
        Dictionary<string, object> location = new Dictionary<string, object>
        {
            { "sceneName", "Scene_Intro_Scene" },
            { "x", 0 },
            { "y", 0 },
            { "z", 0 }
        };
        
        // Define the equipped item numbers for each body part (from the animator)
        Dictionary<string, object> characterEquipment = new Dictionary<string, object>
        {
            { "head", characterAnimator.headItemNumber },
            { "body", characterAnimator.bodyItemNumber },
            { "hair", characterAnimator.hairItemNumber },
            { "torso", characterAnimator.torsoItemNumber },
            { "legs", characterAnimator.legsItemNumber }
        };
        
        // Define the inventory with initial capacity (no items yet)
        Dictionary<string, object> characterInventory = new Dictionary<string, object>
        {
            { "capacity", Settings.playerInitialInventoryCapacity }
        };

        
        // Batch together all updates to write to Firebase in one atomic operation
        Dictionary<string, object> updates = new Dictionary<string, object>();
        updates["users/" + userId + "/characters/" + characterKey + "/info"] = characterInfo;
        updates["users/" + userId + "/characters/" + characterKey + "/equipment"] = characterEquipment;
        updates["users/" + userId + "/characters/" + characterKey + "/inventory"] = characterInventory;
        updates["users/" + userId + "/characters/" + characterKey + "/location"] = location;


        
        // Push all updates to Firebase
        dbReference.UpdateChildrenAsync(updates)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    // Successfully saved all character data
                    Debug.Log("Character saved successfully!");
                    errorMessageText.text = "Character saved successfully!";
                    
                    // Wait a second and return to character selection screen
                    StartCoroutine(ReturnToCharacterSelectionScene());
                }
                else
                {
                    // Log the error that something went wrong while saving
                    Debug.LogError("Failed to save character: " + task.Exception);
                    errorMessageText.text = "Failed to save character.";
                }
            });
    }

    private IEnumerator ReturnToCharacterSelectionScene()
    {
        // Wait for 1 second before changing scenes (gives time for UI message or animation)
        yield return new WaitForSeconds(1f);
        // Load the character selection scene
        SceneManager.LoadScene("Scene_CharacterSelection");
    }
}
