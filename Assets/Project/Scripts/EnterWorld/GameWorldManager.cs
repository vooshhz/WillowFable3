using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;

public class GameWorldManager : MonoBehaviour
{
    public GameObject playerPrefab; // Assign PlayerPrefab in Inspector
    public Transform spawnPoint; // Assign a spawn point in the scene

    private DatabaseReference dbReference;

    private void Start()
    {
        dbReference = FirebaseDatabase.GetInstance("https://willowfable3-default-rtdb.firebaseio.com/").RootReference;

        // Get the stored Character ID
        string characterId = PlayerPrefs.GetString("SelectedCharacterId", null);

        if (!string.IsNullOrEmpty(characterId))
        {
            LoadCharacterData(characterId);
        }
        else
        {
            Debug.LogError("No character selected. Returning to Character Selection.");
            // Optionally, send the player back to Character Selection if no ID is found.
        }
    }

    private void LoadCharacterData(string characterId)
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser?.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("User not logged in.");
            return;
        }

        dbReference.Child("users").Child(userId).Child("characters").Child(characterId).GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Error retrieving character data from Firebase.");
                    return;
                }

                DataSnapshot snapshot = task.Result;
                if (!snapshot.Exists)
                {
                    Debug.LogError("Character data not found.");
                    return;
                }

                // Retrieve character data from Firebase
                string charName = snapshot.Child("characterName").Value.ToString();
                string charClass = snapshot.Child("characterClass").Value.ToString();
                int charLevel = int.Parse(snapshot.Child("level").Value.ToString());
                int headItem = int.Parse(snapshot.Child("headItemNumber").Value.ToString());
                int bodyItem = int.Parse(snapshot.Child("bodyItemNumber").Value.ToString());
                int hairItem = int.Parse(snapshot.Child("hairItemNumber").Value.ToString());
                int torsoItem = int.Parse(snapshot.Child("torsoItemNumber").Value.ToString());
                int legsItem = int.Parse(snapshot.Child("legsItemNumber").Value.ToString());

                // Instantiate the player prefab at the spawn point
                GameObject playerInstance = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);

                // Find the CharacterAnimator component inside the child object
                CharacterAnimator animator = playerInstance.GetComponent<CharacterAnimator>();
                if (animator == null)
                {
                    animator = playerInstance.GetComponentInChildren<CharacterAnimator>(); // Look inside child objects
                }

                if (animator != null)
                {
                    Debug.Log("Found CharacterAnimator inside player prefab!");
                    // Apply the retrieved equipment data
                    ApplyEquipmentToCharacter(animator, headItem, bodyItem, hairItem, torsoItem, legsItem);
                }
                else
                {
                    Debug.LogError("CharacterAnimator component is missing on the instantiated Player Prefab.");
                }
            });
    }

    private void ApplyEquipmentToCharacter(CharacterAnimator animator, int head, int body, int hair, int torso, int legs)
    {
        if (animator == null)
        {
            Debug.LogError("CharacterAnimator is missing on player character.");
            return;
        }

        // Assign retrieved item numbers to CharacterAnimator
        animator.headItemNumber = head;
        animator.bodyItemNumber = body;
        animator.hairItemNumber = hair;
        animator.torsoItemNumber = torso;
        animator.legsItemNumber = legs;

        // Refresh the character visuals
        animator.RefreshCurrentFrame();

        Debug.Log("Equipment applied: Head " + head + ", Body " + body + ", Hair " + hair + ", Torso " + torso + ", Legs " + legs);
    }
}
