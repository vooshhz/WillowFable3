using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using TMPro;
using UnityEngine.UI;
using Firebase.Extensions;

// Manages loading and displaying up to 3 characters for selection
public class CharacterSelectionLoader : MonoBehaviour
{
    private DatabaseReference dbReference; // Firebase Realtime Database reference

    [Header("Character Panel 1")] // UI references for character panel 1
    public GameObject characterPanel1;
    public TMP_Text nameText1, classText1, levelText1;
    public CharacterAnimator characterAnimator1;
    public Image panelImage1;

    [Header("Character Panel 2")] // UI references for character panel 2
    public GameObject characterPanel2;
    public TMP_Text nameText2, classText2, levelText2;
    public CharacterAnimator characterAnimator2;
    public Image panelImage2;

    [Header("Character Panel 3")] // UI references for character panel 3
    public GameObject characterPanel3;
    public TMP_Text nameText3, classText3, levelText3;
    public CharacterAnimator characterAnimator3;
    public Image panelImage3;

    [Header("Enter Game Button")]
    // Stores the selected character's ID

    
    // Reference to the currently selected panel's image (for color highlighting)
    private Image selectedPanelImage = null;
    
    // Called before Start, used to hide all character panels initially
    private void Awake()
    {
        // Ensure all panels start hidden
        characterPanel1.SetActive(false);
        characterPanel2.SetActive(false);
        characterPanel3.SetActive(false);
    }

    private void Start()
    {
        // Initialize Firebase database reference
        dbReference = FirebaseDatabase.GetInstance("https://willowfable3-default-rtdb.firebaseio.com/").RootReference;
        // Start loading character data
        LoadCharacterData();
    }

    // Loads character data from Firebase for the current user
    private void LoadCharacterData()
    {
        // Get the current user's Firebase UID
        string userId = FirebaseAuth.DefaultInstance.CurrentUser?.UserId;
        
        // If user isn't logged in, return early
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("User not logged in.");
            return;
        }
        
        // Fetch up to the first 3 characters for this user
        dbReference.Child("users").Child(userId).Child("characters").LimitToFirst(3).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error retrieving character data from Firebase.");
                return;
            }

            DataSnapshot snapshot = task.Result;
            int index = 0;

            // Loop through each character snapshot (up to 3)
            foreach (DataSnapshot characterSnapshot in snapshot.Children)
            {
                if (index >= 3) break;

                string charId = characterSnapshot.Key;
                
                // Read "info" and "equipment" sub-nodes
                DataSnapshot infoData = characterSnapshot.Child("info");
                DataSnapshot equipmentData = characterSnapshot.Child("equipment");

                // Extract character info values
                string charName = infoData.Child("characterName").Value.ToString();
                string charClass = infoData.Child("characterClass").Value.ToString();
                int charLevel = int.Parse(infoData.Child("level").Value.ToString());
                
                // Extract equipment item numbers
                int headItem = int.Parse(equipmentData.Child("head").Value.ToString());
                int bodyItem = int.Parse(equipmentData.Child("body").Value.ToString());
                int hairItem = int.Parse(equipmentData.Child("hair").Value.ToString());
                int torsoItem = int.Parse(equipmentData.Child("torso").Value.ToString());
                int legsItem = int.Parse(equipmentData.Child("legs").Value.ToString());

                // Assign data to the corresponding panel based on index
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

                index++; // Move to next panel
            }
        });
    }
    
    // Sets up a single character panel's UI and click behavior
    private void SetupCharacterPanel(GameObject panel, TMP_Text nameText, TMP_Text classText, TMP_Text levelText, Image panelImage, string charId, string charName, string charClass, int charLevel)
    {
        panel.SetActive(true); // Make panel visible
        nameText.text = charName;
        classText.text = charClass;
        levelText.text = "Level " + charLevel;

        // Add click listener to the panel
        Button panelButton = panel.GetComponent<Button>();
        if (panelButton != null)
        {
            panelButton.onClick.RemoveAllListeners(); // Clear previous listeners
            panelButton.onClick.AddListener(() => SelectCharacter(charId, panelImage)); // Add new listener
        }
    }
    
    // Called when a character panel is clicked
    private void SelectCharacter(string charId, Image panelImage)
    {
        //  Pass selected character to EnterGameManager
        FindObjectOfType<EnterGameManager>().SetSelectedCharacter(charId);

        // Reset previous panel color if any
        if (selectedPanelImage != null)
        {
            selectedPanelImage.color = Color.white;
        }
        
        // Highlight the selected panel
        panelImage.color = Color.green;
        selectedPanelImage = panelImage;
    }

    // Applies equipment values to the CharacterAnimator instance
    private void ApplyEquipmentToCharacter(CharacterAnimator animator, int head, int body, int hair, int torso, int legs)
    {
        // Null check
        if (animator == null)
        {
            Debug.LogError("CharacterAnimator is missing.");
            return;
        }

        // Assign item numbers
        animator.headItemNumber = head;
        animator.bodyItemNumber = body;
        animator.hairItemNumber = hair;
        animator.torsoItemNumber = torso;
        animator.legsItemNumber = legs;
        
        // Refresh character sprite/animation frame
        animator.RefreshCurrentFrame();
    }

}
