using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

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
                    bool nameExists = false;

                    foreach (DataSnapshot characterSnapshot in snapshot.Children)
                    {
                        string existingName = characterSnapshot.Child("characterName").Value.ToString();

                        if (existingName == characterName)
                        {
                            nameExists = true;
                            break;
                        }
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
        CharacterData characterData = new CharacterData
        {
            characterName = characterName,
            characterClass = classSelectionManager.GetSelectedClass(),
            headItemNumber = characterAnimator.headItemNumber,
            bodyItemNumber = characterAnimator.bodyItemNumber,
            hairItemNumber = characterAnimator.hairItemNumber,
            torsoItemNumber = characterAnimator.torsoItemNumber,
            legsItemNumber = characterAnimator.legsItemNumber,
            level = 1,
            experience = 0
        };

        string characterKey = dbReference.Child("users").Child(userId).Child("characters").Push().Key;

        dbReference.Child("users").Child(userId).Child("characters").Child(characterKey)
            .SetRawJsonValueAsync(JsonUtility.ToJson(characterData))
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
        SceneManager.LoadScene("Scene_CharacterSelection2");
    }
}
