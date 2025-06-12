#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TTSSpeakerReflectionAdapter))]
public class TTSSpeakerReflectionAdapterEditor : Editor
{
    // Editor-only cache of the text to speak
    private string _editorText = "Hello, I am a TTS test.";

    public override void OnInspectorGUI()
    {
        // Draw the default Inspector (exposes Speaker Input reference, etc.)
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("‣ Quick Speak Test", EditorStyles.boldLabel);

        // Text field for input
        _editorText = EditorGUILayout.TextField("Text To Speak", _editorText);

        // Disable the speak button in Edit Mode (needs play mode to run TTS)
        using (new EditorGUI.DisabledScope(!Application.isPlaying))
        {
            if (GUILayout.Button("Speak"))
            {
                var adapter = (TTSSpeakerReflectionAdapter)target;
                adapter.Speak(_editorText);
            }
        }
    }
}
#endif