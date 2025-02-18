using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using UnityEngine.UI;

public class CharacterDisplayManager : MonoBehaviour
{
    public GameObject characterPanelPrefab;
    public Transform panelParent;

    public GameObject characterSelection1Prefab;
    public GameObject characterSelection2Prefab;
    public GameObject characterSelection3Prefab;

    public RenderTexture renderTexture1;
    public RenderTexture renderTexture2;
    public RenderTexture renderTexture3;

    public float startingXPosition = -100f;
    public float xOffset = 15f;

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
                string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
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

            int characterIndex = 0;
            float currentXPosition = startingXPosition;

            foreach (DataSnapshot characterSnapshot in snapshot.Children)
            {
                string charName = characterSnapshot.Child("characterName").Value.ToString();
                string charClass = characterSnapshot.Child("characterClass").Value.ToString();
                int level = int.Parse(characterSnapshot.Child("level").Value.ToString());

                int headItem = int.Parse(characterSnapshot.Child("headItemNumber").Value.ToString());
                int bodyItem = int.Parse(characterSnapshot.Child("bodyItemNumber").Value.ToString());
                int hairItem = int.Parse(characterSnapshot.Child("hairItemNumber").Value.ToString());
                int torsoItem = int.Parse(characterSnapshot.Child("torsoItemNumber").Value.ToString());
                int legsItem = int.Parse(characterSnapshot.Child("legsItemNumber").Value.ToString());

                // === Instantiate Panel in Canvas ===
                GameObject panelInstance = Instantiate(characterPanelPrefab, panelParent);
                panelInstance.transform.Find("NameText").GetComponent<TMP_Text>().text = charName;
                panelInstance.transform.Find("ClassText").GetComponent<TMP_Text>().text = charClass;
                panelInstance.transform.Find("LevelText").GetComponent<TMP_Text>().text = "Level " + level;

                // === Assign the appropriate RenderTexture to Panel ===
                CharacterPanelUI panelUI = panelInstance.GetComponent<CharacterPanelUI>();
                if (panelUI != null && panelUI.rawImage != null)
                {
                    switch (characterIndex)
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

                // === Instantiate Preview Prefab ===
                GameObject prefabToInstantiate = null;
                switch (characterIndex)
                {
                    case 0:
                        prefabToInstantiate = characterSelection1Prefab;
                        break;
                    case 1:
                        prefabToInstantiate = characterSelection2Prefab;
                        break;
                    case 2:
                        prefabToInstantiate = characterSelection3Prefab;
                        break;
                }

                if (prefabToInstantiate != null)
                {
                    GameObject previewInstance = Instantiate(prefabToInstantiate, new Vector3(currentXPosition, 0f, 0f), Quaternion.identity);

                    CharacterPreviewController previewController = previewInstance.GetComponent<CharacterPreviewController>();
                    if (previewController != null)
                    {
                        CharacterEquipmentData equipmentData = new CharacterEquipmentData
                        {
                            headItemNumber = headItem,
                            bodyItemNumber = bodyItem,
                            hairItemNumber = hairItem,
                            torsoItemNumber = torsoItem,
                            legsItemNumber = legsItem
                        };

                        previewController.SetEquipmentData(equipmentData);
                    }
                    else
                    {
                        Debug.LogError("CharacterPreviewController component is missing on the instantiated preview prefab!");
                    }
                }

                // Move X position for the next character
                currentXPosition += xOffset;

                characterIndex++;
                if (characterIndex >= 3)
                {
                    break;
                }
            }
        });
    }
}
