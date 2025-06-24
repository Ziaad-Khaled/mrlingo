//  File: Assets/Editor/GoogleTTSTester.cs
//  Purpose: Simple inspector UI to try GoogleTTS.Speak() from the Editor.

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TTSPlayer))]
public class GoogleTTSTester : Editor
{
    // Inspector-only fields (won’t be saved with the scene)
    string testText = "Hallo Welt – Test aus dem Editor!";
    string langTag  = "de";          // e.g. "de", "de-DE", "es", "es-MX"

    public override void OnInspectorGUI()
    {
        // Draw the original GoogleTTS inspector (so you keep its settings)
        DrawDefaultInspector();
        EditorGUILayout.Space();

        // --- Test panel ---------------------------------------------------
        EditorGUILayout.LabelField("Quick-Test", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Enter Play Mode, type some text, then press Speak to hear it.",
            MessageType.Info);

        testText = EditorGUILayout.TextArea(testText, GUILayout.Height(48));
        langTag  = EditorGUILayout.TextField("Language Tag", langTag);

        // Disable the button when not in Play Mode (audio won’t play in Edit Mode)
        using (new EditorGUI.DisabledScope(!Application.isPlaying))
        {
            if (GUILayout.Button("Speak"))
            {
                var tts = (TTSPlayer)target;
                tts.StartCoroutine(tts.Speak(testText, langTag));
            }
        }
    }
}