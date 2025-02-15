using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class AnimationKeyframeCopier : EditorWindow
{
    [Header("Folders")]
    [Tooltip("Drag the folder containing body animations here.")]
    public DefaultAsset bodyAnimationsFolder; // Folder containing body animations

    [Tooltip("Drag the folder containing torso animations here.")]
    public DefaultAsset torsoAnimationsFolder; // Folder containing torso animations

    [MenuItem("Tools/Animation Keyframe Copier")]
    public static void ShowWindow()
    {
        GetWindow<AnimationKeyframeCopier>("Animation Keyframe Copier");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Animation Keyframe Copier", EditorStyles.boldLabel);

        // Drag-and-drop fields for body and torso folders
        bodyAnimationsFolder = (DefaultAsset)EditorGUILayout.ObjectField(
            "Body Animations Folder", bodyAnimationsFolder, typeof(DefaultAsset), false);

        torsoAnimationsFolder = (DefaultAsset)EditorGUILayout.ObjectField(
            "Torso Animations Folder", torsoAnimationsFolder, typeof(DefaultAsset), false);

        // Validate folders before showing the button
        if (bodyAnimationsFolder != null && torsoAnimationsFolder != null)
        {
            if (GUILayout.Button("Copy Keyframes"))
            {
                string bodyFolderPath = AssetDatabase.GetAssetPath(bodyAnimationsFolder);
                string torsoFolderPath = AssetDatabase.GetAssetPath(torsoAnimationsFolder);

                if (IsValidFolder(bodyFolderPath) && IsValidFolder(torsoFolderPath))
                {
                    CopyKeyframes(bodyFolderPath, torsoFolderPath);
                }
                else
                {
                    Debug.LogError("Please make sure both selected objects are valid folders.");
                }
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Please select both the Body and Torso animation folders.", MessageType.Warning);
        }
    }

    private bool IsValidFolder(string path)
    {
        return AssetDatabase.IsValidFolder(path);
    }

    private void CopyKeyframes(string bodyFolderPath, string torsoFolderPath)
    {
        // Get all body animations
        string[] bodyAnimationPaths = AssetDatabase.FindAssets("t:AnimationClip", new[] { bodyFolderPath });
        string[] torsoAnimationPaths = AssetDatabase.FindAssets("t:AnimationClip", new[] { torsoFolderPath });

        Dictionary<string, AnimationClip> torsoAnimations = new Dictionary<string, AnimationClip>();

        // Load all torso animations
        foreach (string path in torsoAnimationPaths)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(path);
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);
            if (clip != null)
            {
                torsoAnimations[clip.name] = clip;
            }
        }

        // Process body animations
        foreach (string path in bodyAnimationPaths)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(path);
            AnimationClip bodyClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);

            if (bodyClip != null && torsoAnimations.TryGetValue(bodyClip.name, out AnimationClip torsoClip))
            {
                AddTorsoKeyframesToBodyAnimation(bodyClip, torsoClip);
            }
        }

        Debug.Log("Keyframe copying complete!");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void AddTorsoKeyframesToBodyAnimation(AnimationClip bodyClip, AnimationClip torsoClip)
    {
        // Get keyframes from torso animation
        EditorCurveBinding[] torsoBindings = AnimationUtility.GetObjectReferenceCurveBindings(torsoClip);

        foreach (var binding in torsoBindings)
        {
            if (binding.propertyName == "m_Sprite")
            {
                ObjectReferenceKeyframe[] torsoKeyframes = AnimationUtility.GetObjectReferenceCurve(torsoClip, binding);

                // Modify the binding to target the torso GameObject
                EditorCurveBinding torsoBinding = new EditorCurveBinding
                {
                    path = "torso", // Path to the torso GameObject
                    type = binding.type,
                    propertyName = binding.propertyName
                };

                // Add the torso keyframes to the body animation clip
                AnimationUtility.SetObjectReferenceCurve(bodyClip, torsoBinding, torsoKeyframes);
                Debug.Log($"Added torso keyframes to animation: {bodyClip.name}");
            }
        }
    }
}
