using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnimationClipGeneratorFromData))]
public class AnimationClipGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        // Add the "Generate Animations" button
        AnimationClipGeneratorFromData generator = (AnimationClipGeneratorFromData)target;
        if (GUILayout.Button("Generate Animations"))
        {
            generator.GenerateAnimations();
        }
    }
}