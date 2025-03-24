using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Extensions;


public class CharacterSelectionLoader : MonoBehaviour
{
    private DatabaseReference dbReference;

    [Header("Character Panel 1")]
    public GameObject characterPanel1;
    public TMP_Text nameText1, classText1, levelText1;
    public CharacterAnimator characterAnimator1;
    public Image panelImage1;

    [Header("Character Panel 2")]
    public GameObject characterPanel2;
    public TMP_Text nameText2, classText2, levelText2;
    public CharacterAnimator characterAnimator2;
    public Image panelImage2;

    [Header("Character Panel 3")]
    public GameObject characterPanel3;
    public TMP_Text nameText3, classText3, levelText3;
    public CharacterAnimator characterAnimator3;
    public Image panelImage3;

    [Header("Enter Game Button")]
    public Button enterGameButton; // Assign in Inspector

    private string selectedCharacterId = null;
    private Image selectedPanelImage = null;

    private void Awake()
    {
        // Ensure all panels start hidden
        characterPanel1.SetActive(false);
        characterPanel2.SetActive(false);
        characterPanel3.SetActive(false);

        // Ensure Enter Game button starts hidden
        enterGameButton.gameObject.SetActive(false);
    }

    private void Start()
    {
        dbReference = FirebaseDatabase.GetInstance("https://willowfable3-default-rtdb.firebaseio.com/").RootReference;
        LoadCharacterData();
        enterGameButton.onClick.AddListener(EnterGame);
    }
    private void LoadCharacterData()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser?.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("User not logged in.");
            return;
        }

        dbReference.Child("users").Child(userId).Child("characters").LimitToFirst(3).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error retrieving character data from Firebase.");
                return;
            }

            DataSnapshot snapshot = task.Result;
            int index = 0;

            foreach (DataSnapshot characterSnapshot in snapshot.Children)
            {
                if (index >= 3) break;

                string charId = characterSnapshot.Key;
                
                // Get info and equipment data
                DataSnapshot infoData = characterSnapshot.Child("info");
                DataSnapshot equipmentData = characterSnapshot.Child("equipment");

                // Read character info
                string charName = infoData.Child("characterName").Value.ToString();
                string charClass = infoData.Child("characterClass").Value.ToString();
                int charLevel = int.Parse(infoData.Child("level").Value.ToString());
                
                // Read equipment data
                int headItem = int.Parse(equipmentData.Child("head").Value.ToString());
                int bodyItem = int.Parse(equipmentData.Child("body").Value.ToString());
                int hairItem = int.Parse(equipmentData.Child("hair").Value.ToString());
                int torsoItem = int.Parse(equipmentData.Child("torso").Value.ToString());
                int legsItem = int.Parse(equipmentData.Child("legs").Value.ToString());

                if (index == 0)
                {
                    SetupCharacterPanel(characterPanel1, nameText1, classText1, levelText1, panelImage1, charId, charName, charClass, charLevel);
                    ApplyEquipmentToCharacter(characterAnimator1, headItem, bodyItem, hairItem, torsoItem, legsItem);
                }
                else if (index == 1)
                {
                    SetupCharacterPanel(characterPanel2, nameText2, classText2, levelText2, panelImage2, charId, charName, charClass, charLevel);
                    ApplyEquipmentToCharacter(characterAnimator2, headItem, bodyItem, hairItem, torsoItem, legsItem);
                }
                else if (index == 2)
                {
                    SetupCharacterPanel(characterPanel3, nameText3, classText3, levelText3, panelImage3, charId, charName, charClass, charLevel);
                    ApplyEquipmentToCharacter(characterAnimator3, headItem, bodyItem, hairItem, torsoItem, legsItem);
                }

                index++;
            }
        });
    }
    private void SetupCharacterPanel(GameObject panel, TMP_Text nameText, TMP_Text classText, TMP_Text levelText, Image panelImage, string charId, string charName, string charClass, int charLevel)
    {
        panel.SetActive(true);
        nameText.text = charName;
        classText.text = charClass;
        levelText.text = "Level " + charLevel;

        Button panelButton = panel.GetComponent<Button>();
        if (panelButton != null)
        {
            panelButton.onClick.RemoveAllListeners();
            panelButton.onClick.AddListener(() => SelectCharacter(charId, panelImage));
        }
    }

    private void SelectCharacter(string charId, Image panelImage)
    {
        selectedCharacterId = charId;

        //  Pass selected character to EnterGameManager
        FindObjectOfType<EnterGameManager>().SetSelectedCharacter(charId);

        if (selectedPanelImage != null)
        {
            selectedPanelImage.color = Color.white;
        }

        panelImage.color = Color.green;
        selectedPanelImage = panelImage;
    }


    private void ApplyEquipmentToCharacter(CharacterAnimator animator, int head, int body, int hair, int torso, int legs)
    {
        if (animator == null)
        {
            Debug.LogError("CharacterAnimator is missing.");
            return;
        }

        animator.headItemNumber = head;
        animator.bodyItemNumber = body;
        animator.hairItemNumber = hair;
        animator.torsoItemNumber = torso;
        animator.legsItemNumber = legs;

        animator.RefreshCurrentFrame();
    }

    private void EnterGame()
    {
        if (!string.IsNullOrEmpty(selectedCharacterId))
        {
            PlayerPrefs.SetString("SelectedCharacterId", selectedCharacterId);            
        }
    }
}
