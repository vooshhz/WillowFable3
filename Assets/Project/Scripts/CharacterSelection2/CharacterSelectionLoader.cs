using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using TMPro;
using UnityEngine.UI;
using Firebase.Extensions;

public class CharacterSelectionLoader : MonoBehaviour
{
    [Header("Character Panel 1")]
    public GameObject characterPanel1;
    public TMP_Text nameText1;
    public TMP_Text classText1;
    public TMP_Text levelText1;
    public CharacterAnimator characterAnimator1;

    [Header("Character Panel 2")]
    public GameObject characterPanel2;
    public TMP_Text nameText2;
    public TMP_Text classText2;
    public TMP_Text levelText2;
    public CharacterAnimator characterAnimator2;

    [Header("Character Panel 3")]
    public GameObject characterPanel3;
    public TMP_Text nameText3;
    public TMP_Text classText3;
    public TMP_Text levelText3;
    public CharacterAnimator characterAnimator3;

    private DatabaseReference dbReference;

    private void Start()
    {
        dbReference = FirebaseDatabase.GetInstance("https://willowfable3-default-rtdb.firebaseio.com/").RootReference;
        LoadCharacterData();
    }

    private void LoadCharacterData()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser?.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("User not logged in.");
            return;
        }

        // Retrieve up to 3 characters
        dbReference.Child("users").Child(userId).Child("characters").LimitToFirst(3).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error retrieving character data from Firebase.");
                return;
            }

            DataSnapshot snapshot = task.Result;

            // Hide all panels initially
            ResetCharacterPanels();

            int index = 0;
            foreach (DataSnapshot characterSnapshot in snapshot.Children)
            {
                if (index >= 3) break; // Maximum of 3 characters

                // Retrieve character details
                string charName = characterSnapshot.Child("characterName").Value.ToString();
                string charClass = characterSnapshot.Child("characterClass").Value.ToString();
                int charLevel = int.Parse(characterSnapshot.Child("level").Value.ToString());

                // Retrieve equipment item numbers
                int headItem = int.Parse(characterSnapshot.Child("headItemNumber").Value.ToString());
                int bodyItem = int.Parse(characterSnapshot.Child("bodyItemNumber").Value.ToString());
                int hairItem = int.Parse(characterSnapshot.Child("hairItemNumber").Value.ToString());
                int torsoItem = int.Parse(characterSnapshot.Child("torsoItemNumber").Value.ToString());
                int legsItem = int.Parse(characterSnapshot.Child("legsItemNumber").Value.ToString());

                // Assign data to the correct panel
                if (index == 0)
                {
                    EnablePanel(characterPanel1, nameText1, classText1, levelText1, charName, charClass, charLevel);
                    ApplyEquipmentToCharacter(characterAnimator1, headItem, bodyItem, hairItem, torsoItem, legsItem);
                }
                else if (index == 1)
                {
                    EnablePanel(characterPanel2, nameText2, classText2, levelText2, charName, charClass, charLevel);
                    ApplyEquipmentToCharacter(characterAnimator2, headItem, bodyItem, hairItem, torsoItem, legsItem);
                }
                else if (index == 2)
                {
                    EnablePanel(characterPanel3, nameText3, classText3, levelText3, charName, charClass, charLevel);
                    ApplyEquipmentToCharacter(characterAnimator3, headItem, bodyItem, hairItem, torsoItem, legsItem);
                }

                index++; // Move to next panel
            }
        });
    }

    private void EnablePanel(GameObject panel, TMP_Text nameText, TMP_Text classText, TMP_Text levelText, string charName, string charClass, int charLevel)
    {
        panel.SetActive(true);
        nameText.text = charName;
        classText.text = charClass;
        levelText.text = "Level " + charLevel;
    }

    private void ApplyEquipmentToCharacter(CharacterAnimator animator, int head, int body, int hair, int torso, int legs)
    {
        if (animator == null)
        {
            Debug.LogError("CharacterAnimator is missing on one of the CharacterSelection objects.");
            return;
        }

        animator.headItemNumber = head;
        animator.bodyItemNumber = body;
        animator.hairItemNumber = hair;
        animator.torsoItemNumber = torso;
        animator.legsItemNumber = legs;

        animator.RefreshCurrentFrame(); // Apply changes immediately
    }

    private void ResetCharacterPanels()
    {
        // Hide all panels and reset UI
        characterPanel1.SetActive(false);
        nameText1.text = "";
        classText1.text = "";
        levelText1.text = "";

        characterPanel2.SetActive(false);
        nameText2.text = "";
        classText2.text = "";
        levelText2.text = "";

        characterPanel3.SetActive(false);
        nameText3.text = "";
        classText3.text = "";
        levelText3.text = "";
    }
}
