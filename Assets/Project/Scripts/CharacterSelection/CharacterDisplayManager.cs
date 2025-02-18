using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;

public class CharacterDisplayManager : MonoBehaviour
{
    public GameObject characterPanelPrefab;
    public Transform panelParent;

    public RenderTexture renderTexture1;
    public RenderTexture renderTexture2;
    public RenderTexture renderTexture3;

    private DatabaseReference dbReference;

    private const string USERS_NODE = "users";
    private const string CHARACTERS_NODE = "characters";

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                dbReference = FirebaseDatabase.DefaultInstance.RootReference;
                string userId = Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId;
                LoadCharacterData(userId);
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies.");
            }
        });
    }

    void LoadCharacterData(string userId)
    {
        dbReference.Child(USERS_NODE).Child(userId).Child(CHARACTERS_NODE).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error retrieving character data from Firebase.");
                return;
            }

            DataSnapshot snapshot = task.Result;
            if (!snapshot.Exists)
            {
                Debug.Log("No character data found for user " + userId);
                return;
            }

            int count = 0;

            foreach (DataSnapshot characterSnapshot in snapshot.Children)
            {
                string charName = characterSnapshot.Child("characterName").Value.ToString();
                string charClass = characterSnapshot.Child("characterClass").Value.ToString();
                int level = int.Parse(characterSnapshot.Child("level").Value.ToString());

                // Instantiate CharacterPanel1 in Canvas
                GameObject panelInstance = Instantiate(characterPanelPrefab, panelParent);

                // Assign Text fields in CharacterPanel1
                panelInstance.transform.Find("NameText").GetComponent<TMP_Text>().text = charName;
                panelInstance.transform.Find("ClassText").GetComponent<TMP_Text>().text = charClass;
                panelInstance.transform.Find("LevelText").GetComponent<TMP_Text>().text = "Level " + level;

                // Assign the appropriate RenderTexture to RawImage
                CharacterPanelUI panelUI = panelInstance.GetComponent<CharacterPanelUI>();
                if (panelUI != null && panelUI.rawImage != null)
                {
                    switch (count)
                    {
                        case 0:
                            panelUI.rawImage.texture = renderTexture1;
                            break;
                        case 1:
                            panelUI.rawImage.texture = renderTexture2;
                            break;
                        case 2:
                            panelUI.rawImage.texture = renderTexture3;
                            break;
                    }
                }
                else
                {
                    Debug.LogWarning("CharacterPanelUI or RawImage is missing on panel instance.");
                }

                count++;
                if (count >= 3) break;
            }
        });
    }
}
