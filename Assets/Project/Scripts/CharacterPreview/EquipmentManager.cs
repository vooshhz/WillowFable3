using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class EquipmentManager : MonoBehaviour
{
    [Header("Equipment Data")]
    public EquipmentData equipmentData; // Reference to the ScriptableObject holding equipment data

    [Header("Animator Controller")]
    public Animator animator; // Drag and drop the Animator component here

    [Header("Animation Clips")]
    public AnimationClip[] animationClips; // Array of animation clips to modify

    [Header("Selected Item")]
    [SerializeField] private int _selectedIndex = -1; // Internal selected index for change detection

    public int selectedIndex
    {
        get => _selectedIndex;
        set
        {
            if (_selectedIndex != value) // Detect index change
            {
                _selectedIndex = value;
                OnSelectedIndexChanged(); // Handle the change
            }
        }
    }

    // Full mapping dictionary with the provided animation clip indices.
    // These keys are the general animation names (e.g., "idle_down")—your animation clips
    // should be named with a prefix (e.g., "body_idle_down" or "head_idle_down").
    private readonly Dictionary<string, (int startIndex, int endIndex)> animationMappings = new Dictionary<string, (int, int)>
    {
        { "spellcast_up", (0, 6) },
        { "spellcast_left", (13, 19) },
        { "spellcast_down", (26, 32) },
        { "spellcast_right", (39, 45) },
        { "thrust_up", (52, 59) },
        { "thrust_left", (65, 72) },
        { "thrust_down", (78, 85) },
        { "thrust_right", (91, 98) },
        { "walk_up", (104, 112) },
        { "walk_left", (117, 125) },
        { "walk_down", (130, 138) },
        { "walk_right", (143, 151) },
        { "slash_up", (156, 161) },
        { "slash_left", (169, 174) },
        { "slash_down", (182, 187) },
        { "slash_right", (195, 200) },
        { "shoot_up", (208, 220) },
        { "shoot_left", (221, 233) },
        { "shoot_down", (234, 246) },
        { "shoot_right", (247, 259) },
        { "hurt", (260, 265) },
        { "climb", (273, 278) },
        { "idle_up", (286, 287) },
        { "idle_left", (299, 300) },
        { "idle_down", (312, 313) },
        { "idle_right", (325, 326) },
        { "combat_up", (288, 289) },
        { "combat_left", (301, 302) },
        { "combat_down", (314, 315) },
        { "combat_right", (327, 328) },
        { "jump_up", (338, 342) },
        { "jump_left", (351, 355) },
        { "jump_down", (364, 368) },
        { "jump_right", (377, 381) },
        { "sit_up", (390, 392) },
        { "sit_left", (403, 405) },
        { "sit_down", (416, 418) },
        { "sit_right", (429, 431) },
        { "emote_up", (393, 395) },
        { "emote_left", (406, 408) },
        { "emote_down", (419, 421) },
        { "emote_right", (432, 434) },
        { "run_up", (442, 449) },
        { "run_left", (455, 462) },
        { "run_down", (468, 475) },
        { "run_right", (481, 488) },
        { "one_hand_halfslash_up", (500, 502) },
        { "one_hand_halfslash_left", (513, 515) },
        { "one_hand_halfslash_down", (526, 528) },
        { "one_hand_halfslash_right", (539, 541) },
        { "one_hand_backslash_up", (494, 506) },
        { "one_hand_backslash_left", (507, 519) },
        { "one_hand_backslash_down", (520, 532) },
        { "one_hand_backslash_right", (533, 545) },
        { "one_hand_slash_up", (546, 551) },
        { "one_hand_slash_left", (559, 564) },
        { "one_hand_slash_down", (572, 577) },
        { "one_hand_slash_right", (585, 590) }
    };

    private void OnValidate()
    {
        if (equipmentData == null || equipmentData.equipmentItems == null || equipmentData.equipmentItems.Length == 0)
            return; // Skip if no equipment data is assigned

        selectedIndex = Mathf.Clamp(selectedIndex, -1, equipmentData.equipmentItems.Length - 1);
    }

    private void Start()
    {
        // When the game starts, update all animations once so that the equipment is correctly applied.
        RefreshAllAnimations();
    }

    private void OnSelectedIndexChanged()
    {
        Debug.Log($"SelectedIndex changed to {selectedIndex}. Refreshing animations...");
        RefreshAllAnimations(); // Automatically refresh animations
    }

    public void RefreshAllAnimations()
    {
        // Verify that we have valid equipment data and animations.
        if (equipmentData == null || equipmentData.equipmentItems == null || equipmentData.equipmentItems.Length == 0)
        {
            Debug.LogWarning("EquipmentData or equipment items are not assigned or empty.");
            return;
        }

        if (animator == null)
        {
            Debug.LogError("Animator component is not assigned.");
            return;
        }

        if (animationClips == null || animationClips.Length == 0)
        {
            Debug.LogError("No animation clips are assigned.");
            return;
        }

        if (selectedIndex == -1)
        {
            Debug.LogWarning("No item selected.");
            return;
        }

        EquipmentData.EquipmentItem selectedItem = equipmentData.equipmentItems[selectedIndex];
        if (selectedItem == null || selectedItem.slicedSpritesArray == null || selectedItem.slicedSpritesArray.Length == 0)
        {
            Debug.LogError($"Selected item is null or has no sprites for '{selectedItem.itemType}'.");
            return;
        }

        // Use this GameObject's name as the target part.
        // (Make sure your child GameObject is named exactly as the equipment part—e.g., "head", "body", "hair", etc.)
        string partName = gameObject.name.ToLower();

        foreach (AnimationClip clip in animationClips)
        {
            string animationName = clip.name.ToLower();
            // Expect animation clip names to be prefixed with the part name (e.g., "head_idle_down")
            string prefix = partName + "_";
            string searchKey = animationName.StartsWith(prefix) ? animationName.Substring(prefix.Length) : animationName;

            if (animationMappings.TryGetValue(searchKey, out var indices))
            {
                // Use the part name for the binding path so that it always updates the correct child GameObject.
                string propertyName = $"{partName}: m_Sprite";

                Debug.Log($"Updating animation '{clip.name}' for property '{propertyName}'");
                UpdateAnimationKeyframes(clip, selectedItem.slicedSpritesArray, indices.startIndex, indices.endIndex, propertyName);
            }
            else
            {
                Debug.LogWarning($"No mapping found for animation '{clip.name}' (search key: '{searchKey}'). Skipping.");
            }
        }
    }

    private void UpdateAnimationKeyframes(AnimationClip clip, Sprite[] slicedSprites, int startIndex, int endIndex, string propertyName)
    {
        if (slicedSprites == null || slicedSprites.Length == 0 || clip == null)
        {
            Debug.LogError("Invalid sprites or animation clip.");
            return;
        }

        if (startIndex < 0 || endIndex >= slicedSprites.Length || startIndex > endIndex)
        {
            Debug.LogError($"Invalid sprite indices for animation '{clip.name}'. StartIndex: {startIndex}, EndIndex: {endIndex}");
            return;
        }

        // Split the propertyName (formatted as "part: m_Sprite") to extract the target path.
        EditorCurveBinding spriteBinding = new EditorCurveBinding
        {
            path = propertyName.Split(':')[0].Trim(),  // this will be the part name (e.g., "head")
            type = typeof(SpriteRenderer),
            propertyName = "m_Sprite"
        };

        int frameCount = endIndex - startIndex + 1;
        ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[frameCount];
        float timePerFrame = clip.length / frameCount;

        for (int i = 0; i < frameCount; i++)
        {
            keyframes[i] = new ObjectReferenceKeyframe
            {
                time = i * timePerFrame,
                value = slicedSprites[startIndex + i]
            };
        }

        AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, keyframes);
        Debug.Log($"Updated animation '{clip.name}' with property '{propertyName}' using sprites {startIndex} to {endIndex}");
    }
}
