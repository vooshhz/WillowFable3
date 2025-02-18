using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Database;
using Firebase.Auth;

public class CharacterSelectionManager : MonoBehaviour
{
    public Button createCharacterButton;

    private DatabaseReference dbReference;

    private void Start()
    {
        dbReference = FirebaseDatabase.GetInstance("https://willowfable3-default-rtdb.firebaseio.com/").RootReference;
        CheckCharacterLimit();
        createCharacterButton.onClick.AddListener(OnCreateCharacterButtonClicked);
    }

    private void CheckCharacterLimit()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        dbReference.Child("users").Child(userId).Child("characters").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                int characterCount = (int)snapshot.ChildrenCount;

                Debug.Log($"Character count: {characterCount}");

                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    if (characterCount >= 3)
                    {
                        createCharacterButton.interactable = false;
                        createCharacterButton.gameObject.SetActive(false);
                    }
                    else
                    {
                        createCharacterButton.interactable = true;
                        createCharacterButton.gameObject.SetActive(true);
                    }
                });
            }
            else
            {
                Debug.LogError("Failed to check character count: " + task.Exception);
            }
        });
    }

    private void OnCreateCharacterButtonClicked()
    {
        SceneManager.LoadScene("Scene_CreateCharacter");
    }
}
