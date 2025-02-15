using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AnimationClipGenerator", menuName = "Tools/AnimationClipGenerator")]
public class AnimationClipGeneratorFromData : ScriptableObject
{
    [Header("Input Settings")]
    public Sprite[] slicedSprites; // Array of sliced sprites
    public float defaultFrameRate = 12f; // Default frame rate

    [Header("Output Settings")]
    public string outputFolder = "Assets/Animations"; // Folder to save the generated animations

    [Header("Child GameObject Name")]
    // The name of the child GameObject that the animation will target.
    // This value will also be used as a prefix for each generated animation clip.
    public string childGameObjectName = "body";

    [ContextMenu("Generate Animations")]
    public void GenerateAnimations()
    {
        if (slicedSprites == null || slicedSprites.Length == 0)
        {
            Debug.LogError("No sliced sprites provided!");
            return;
        }

        List<AnimationConfig> animationsToGenerate = new List<AnimationConfig>
        {
            new AnimationConfig("spellcast_up", 0, 6),
            new AnimationConfig("spellcast_left", 13, 19),
            new AnimationConfig("spellcast_down", 26, 32),
            new AnimationConfig("spellcast_right", 39, 45),
            new AnimationConfig("thrust_up", 52, 59),
            new AnimationConfig("thrust_left", 65, 72),
            new AnimationConfig("thrust_down", 78, 85),
            new AnimationConfig("walk_up", 104, 112),
            new AnimationConfig("walk_left", 117, 125),
            new AnimationConfig("walk_down", 130, 138),
            new AnimationConfig("walk_right", 143, 151),
            new AnimationConfig("slash_up", 156, 161),
            new AnimationConfig("slash_left", 169, 174),
            new AnimationConfig("slash_down", 182, 187),
            new AnimationConfig("slash_right", 195, 200),
            new AnimationConfig("shoot_up", 208, 220),
            new AnimationConfig("shoot_left", 221, 233),
            new AnimationConfig("shoot_down", 234, 246),
            new AnimationConfig("shoot_right", 247, 259),
            new AnimationConfig("hurt", 260, 265),
            new AnimationConfig("climb", 273, 278),
            new AnimationConfig("idle_up", 286, 287),
            new AnimationConfig("idle_left", 299, 300),
            new AnimationConfig("idle_down", 312, 313),
            new AnimationConfig("idle_right", 325, 326),
            new AnimationConfig("combat_up", 288, 289),
            new AnimationConfig("combat_left", 301, 302),
            new AnimationConfig("combat_down", 314, 315),
            new AnimationConfig("combat_right", 327, 328),
            new AnimationConfig("jump_up", 338, 342),
            new AnimationConfig("jump_left", 351, 355),
            new AnimationConfig("jump_down", 364, 368),
            new AnimationConfig("jump_right", 377, 381),
            new AnimationConfig("sit_up", 390, 392),
            new AnimationConfig("sit_left", 403, 405),
            new AnimationConfig("sit_down", 416, 418),
            new AnimationConfig("sit_right", 429, 431),
            new AnimationConfig("emote_up", 393, 395),
            new AnimationConfig("emote_left", 406, 408),
            new AnimationConfig("emote_down", 419, 421),
            new AnimationConfig("emote_right", 432, 434),
            new AnimationConfig("run_up", 442, 449),
            new AnimationConfig("run_left", 455, 462),
            new AnimationConfig("run_down", 468, 475),
            new AnimationConfig("run_right", 481, 488),
            new AnimationConfig("one_hand_halfslash_up", 500, 502),
            new AnimationConfig("one_hand_halfslash_left", 513, 515),
            new AnimationConfig("one_hand_halfslash_down", 526, 528),
            new AnimationConfig("one_hand_halfslash_right", 539, 541),
            new AnimationConfig("one_hand_backslash_up", 494, 506),
            new AnimationConfig("one_hand_backslash_left", 507, 519),
            new AnimationConfig("one_hand_backslash_down", 520, 532),
            new AnimationConfig("one_hand_backslash_right", 533, 545),
            new AnimationConfig("one_hand_slash_up", 546, 551),
            new AnimationConfig("one_hand_slash_left", 559, 564),
            new AnimationConfig("one_hand_slash_down", 572, 577),
            new AnimationConfig("one_hand_slash_right", 585, 590)
        };

        foreach (var config in animationsToGenerate)
        {
            CreateAnimationClip(config);
        }

        Debug.Log("Animations generated successfully!");
    }

    private void CreateAnimationClip(AnimationConfig config)
    {
        if (config.startIndex < 0 || config.endIndex >= slicedSprites.Length || config.startIndex > config.endIndex)
        {
            Debug.LogError($"Invalid indices for animation '{config.animationName}'! Skipping...");
            return;
        }

        AnimationClip clip = new AnimationClip();
        clip.frameRate = config.frameRate;

        // The binding targets the child GameObject (by name) and its SpriteRenderer's m_Sprite property.
        EditorCurveBinding spriteBinding = new EditorCurveBinding
        {
            path = childGameObjectName,
            type = typeof(SpriteRenderer),
            propertyName = "m_Sprite"
        };

        int frameCount = config.endIndex - config.startIndex + 1;
        ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[frameCount];
        float timePerFrame = 1f / config.frameRate;

        for (int i = 0; i < frameCount; i++)
        {
            keyframes[i] = new ObjectReferenceKeyframe
            {
                time = i * timePerFrame,
                value = slicedSprites[config.startIndex + i]
            };
        }

        AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, keyframes);

        // Use the childGameObjectName as the prefix for the animation clip name.
        string prefixedAnimationName = $"{childGameObjectName}_{config.animationName}";
        string clipPath = $"{outputFolder}/{prefixedAnimationName}.anim";
        AssetDatabase.CreateAsset(clip, clipPath);

        Debug.Log($"Animation '{prefixedAnimationName}' created and saved to {clipPath}.");
    }

    [System.Serializable]
    public class AnimationConfig
    {
        public string animationName;
        public int startIndex;
        public int endIndex;
        public float frameRate;

        public AnimationConfig(string name, int start, int end, float frameRate = 12f)
        {
            animationName = name;
            startIndex = start;
            endIndex = end;
            this.frameRate = frameRate;
        }
    }
}
