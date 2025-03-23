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
    public CharacterAnimator characterAnimator;
    public ClassSelectionManager classSelectionManager;
    public TMP_InputField characterNameInput;
    public TMP_Text errorMessageText;

    private DatabaseReference dbReference;

    private void Start()
    {
        dbReference = FirebaseDatabase.GetInstance("https://willowfable3-default-rtdb.firebaseio.com/").RootReference;
    }

    public void SaveCharacterData()
    {
    FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;

    if (user == null)
    {
        Debug.LogError("User is not logged in.");
        return;
    }

    string userId = user.UserId;
    string characterName = characterNameInput.text;

    // Check if a character with the same name already exists
    dbReference.Child("users").Child(userId).Child("characters")
        .GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to check existing characters: " + task.Exception);
                return;
            }

            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                
                // Count characters
                int characterCount = 0;
                bool nameExists = false;

                foreach (DataSnapshot characterSnapshot in snapshot.Children)
                {
                    // Check if this is a valid character node
                    if (characterSnapshot.HasChild("info") && 
                        characterSnapshot.HasChild("equipment") && 
                        characterSnapshot.HasChild("inventory"))
                    {
                        characterCount++;

                        // Check for existing name in the info node
                        string existingName = characterSnapshot.Child("info").Child("characterName").Value?.ToString();

                        if (existingName == characterName)
                        {
                            nameExists = true;
                        }
                    }
                }

                // Check character limit
                if (characterCount >= 3)
                {
                    Debug.LogWarning("Maximum character limit reached.");
                    errorMessageText.text = "Maximum of 3 characters allowed.";
                    return;
                }

                if (nameExists)
                {
                    Debug.LogWarning("Character name already exists. Choose a different name.");
                    errorMessageText.text = "Character name already exists. Choose another name.";
                }
                else
                {
                    CreateNewCharacter(userId, characterName);
                }
            }
        });
    }
    private void CreateNewCharacter(string userId, string characterName)
    {
        // Create character key first
        string characterKey = dbReference.Child("users").Child(userId).Child("characters").Push().Key;
        
        // Create info data
        Dictionary<string, object> characterInfo = new Dictionary<string, object>
        {
            { "characterName", characterName },
            { "characterClass", classSelectionManager.GetSelectedClass() },
            { "level", 1 },
            { "experience", 0 },
            { "createdAt", ServerValue.Timestamp }
        };

        // Create location data
        Dictionary<string, object> location = new Dictionary<string, object>
        {
            { "sceneName", "Scene_IntroScene" },
            { "x", 0 },
            { "y", 0 },
            { "z", 0 }
        };
        
        // Create equipment data
        Dictionary<string, object> characterEquipment = new Dictionary<string, object>
        {
            { "head", characterAnimator.headItemNumber },
            { "body", characterAnimator.bodyItemNumber },
            { "hair", characterAnimator.hairItemNumber },
            { "torso", characterAnimator.torsoItemNumber },
            { "legs", characterAnimator.legsItemNumber }
        };
        
        // Create empty inventory structure
        Dictionary<string, object> characterInventory = new Dictionary<string, object>
        {
            { "capacity", Settings.playerInitialInventoryCapacity }
        };

        
        // Create updates for all paths
        Dictionary<string, object> updates = new Dictionary<string, object>();
        updates["users/" + userId + "/characters/" + characterKey + "/info"] = characterInfo;
        updates["users/" + userId + "/characters/" + characterKey + "/equipment"] = characterEquipment;
        updates["users/" + userId + "/characters/" + characterKey + "/inventory"] = characterInventory;
        updates["users/" + userId + "/characters/" + characterKey + "/location"] = location;


        
        // Execute all updates atomically
        dbReference.UpdateChildrenAsync(updates)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("Character saved successfully!");
                    errorMessageText.text = "Character saved successfully!";
                    StartCoroutine(ReturnToCharacterSelectionScene());
                }
                else
                {
                    Debug.LogError("Failed to save character: " + task.Exception);
                    errorMessageText.text = "Failed to save character.";
                }
            });
    }

    private IEnumerator ReturnToCharacterSelectionScene()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Scene_CharacterSelection");
    }
}
