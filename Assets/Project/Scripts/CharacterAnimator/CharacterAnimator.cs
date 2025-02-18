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

    // Animation Frames Dictionary (You already added this earlier)
    private readonly Dictionary<string, (int startFrame, int endFrame)> animationFrames = new Dictionary<string, (int, int)>
{
    { "run_up", (442, 449) },
    { "run_down", (468, 475) },
    { "run_left", (455, 462) },
    { "run_right", (481, 488) },

    { "idle_up", (286, 287) },
    { "idle_down", (312, 313) },
    { "idle_left", (299, 300) },
    { "idle_right", (325, 326) }
};


    private void Start()
    {
        PlayIdle(PlayerFacing.Down);
    }

    private void Update()
    {
        UpdateAnimation(); // Handles frame timing and sprite updates
    }

    // Plays an idle animation based on facing direction
    public void PlayIdle(PlayerFacing facing)
    {
        PlayAnimation($"idle_{facing.ToString().ToLower()}", true);
    }

    // Plays a walk animation based on facing direction
    public void PlayRun(PlayerFacing facing)
    {
        PlayAnimation($"run_{facing.ToString().ToLower()}", true);
    }

    // Core function to play an animation by name
    public void PlayAnimation(string animationName, bool looping)
    {
        // Only restart if it's not the current animation
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



    private int currentAnimationStart;
    private int currentAnimationEnd;
    private bool loopAnimation;

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


    public void SetFrame(int frameIndex)
    {
        currentFrame = frameIndex;
       // Debug.Log($"Setting frame: {frameIndex}");

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
}
