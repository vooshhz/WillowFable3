using UnityEngine;
using System.Collections.Generic;

public class CharacterAnimator : MonoBehaviour
{
    // Body parts renderers
    public SpriteRenderer headRenderer;
    public SpriteRenderer bodyRenderer;
    public SpriteRenderer hairRenderer;
    public SpriteRenderer torsoRenderer;
    public SpriteRenderer legsRenderer;


    // Data sources
    public EquipmentData headData;
    public EquipmentData bodyData;
    public EquipmentData hairData;
    public EquipmentData torsoData;
    public EquipmentData legsData;

    // Equipped items
    public int headItemNumber = 20001;
    public int bodyItemNumber = 10001;
    public int hairItemNumber = 30001;
    public int torsoItemNumber = 40001;
    public int legsItemNumber = 50001;


    // Current animation state
    private int currentFrame;
    private float frameTimer;
    public float frameRate = 0.1f; // 10 frames per second

    private string currentAnimation;

    // Animation Frames Dictionary
    private readonly Dictionary<string, (int startFrame, int endFrame)> animationFrames = new Dictionary<string, (int, int)>
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

    private int currentAnimationStart;
    private int currentAnimationEnd;
    private bool loopAnimation;

    private void Start()
    {
        PlayIdle(PlayerFacing.Down);
    }

    private void Update()
    {
        UpdateAnimation();
    }

    /// <summary>
    /// Applies character state from NetworkCharacter.
    /// </summary>
    public void ApplyCharacterState(CharacterState state, PlayerFacing direction)
    {
        if (state == CharacterState.Running)
        {
            PlayRun(direction);
        }
        else
        {
            PlayIdle(direction);
        }
    }

    /// <summary>
    /// Plays an idle animation based on facing direction.
    /// </summary>
    public void PlayIdle(PlayerFacing facing)
    {
        PlayAnimation($"idle_{facing.ToString().ToLower()}", true);
    }

    /// <summary>
    /// Plays a run animation based on facing direction.
    /// </summary>
    public void PlayRun(PlayerFacing facing)
    {
        PlayAnimation($"run_{facing.ToString().ToLower()}", true);
    }

    /// <summary>
    /// Core function to play an animation by name.
    /// </summary>
    public void PlayAnimation(string animationName, bool looping)
    {
        // Prevent restarting the same animation
        if (currentAnimation == animationName) return;

        currentAnimation = animationName;

        if (!animationFrames.TryGetValue(animationName, out var frames))
        {
            Debug.LogError($"Animation '{animationName}' not found.");
            return;
        }

        currentFrame = frames.startFrame;
        frameTimer = 0f;
        loopAnimation = looping;
        currentAnimationStart = frames.startFrame;
        currentAnimationEnd = frames.endFrame;

        SetFrame(currentFrame); // Show the first frame immediately
    }

    private void UpdateAnimation()
    {
        frameTimer += Time.deltaTime;

        if (frameTimer >= frameRate)
        {
            frameTimer -= frameRate;
            currentFrame++;

            if (currentFrame > currentAnimationEnd)
            {
                if (loopAnimation)
                {
                    currentFrame = currentAnimationStart;
                }
                else
                {
                    currentFrame = currentAnimationEnd;
                }
            }

            SetFrame(currentFrame);
        }
    }

    /// <summary>
    /// Sets the correct frame for each body part.
    /// </summary>
    public void SetFrame(int frameIndex)
    {
        currentFrame = frameIndex;

        if (headRenderer != null && headData != null)
            headRenderer.sprite = GetSpriteFromItem(headData, headItemNumber, frameIndex);

        if (bodyRenderer != null && bodyData != null)
            bodyRenderer.sprite = GetSpriteFromItem(bodyData, bodyItemNumber, frameIndex);

        if (hairRenderer != null && hairData != null)
            hairRenderer.sprite = GetSpriteFromItem(hairData, hairItemNumber, frameIndex);

        if (torsoRenderer != null && torsoData != null)
            torsoRenderer.sprite = GetSpriteFromItem(torsoData, torsoItemNumber, frameIndex);

        if (legsRenderer != null && legsData != null)
            legsRenderer.sprite = GetSpriteFromItem(legsData, legsItemNumber, frameIndex);
    }

    /// <summary>
    /// Fetches the correct sprite for an equipment item.
    /// </summary>
    private Sprite GetSpriteFromItem(EquipmentData data, int itemNumber, int frameIndex)
    {
        foreach (var item in data.equipmentItems)
        {
            if (item.itemNumber == itemNumber)
            {
                if (frameIndex >= 0 && frameIndex < item.slicedSpritesArray.Length)
                    return item.slicedSpritesArray[frameIndex];
                else
                    Debug.LogWarning($"Frame index {frameIndex} is out of range for item {item.itemName}");
            }
        }

        Debug.LogWarning($"Item number {itemNumber} not found in {data.name}");
        return null;
    }

    /// <summary>
    /// Refreshes the character's current animation frame.
    /// </summary>
    public void RefreshCurrentFrame()
    {
        SetFrame(currentFrame);
    }
}
