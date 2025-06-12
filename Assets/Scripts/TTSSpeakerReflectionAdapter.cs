using System;
using System.Reflection;
using UnityEngine;

public class TTSSpeakerReflectionAdapter : MonoBehaviour
{
    [Tooltip("Reference to the component that contains TTSSpeakerInput")]
    [SerializeField] private MonoBehaviour speakerInput;   // drag TTSSpeakerInput here

    private object     _speakerInstance;      // Meta.WitAi.TTS.Utilities.TTSSpeaker
    private MethodInfo _speakMethod;          // TTSSpeaker.Speak(string)
    private MethodInfo _formatTextMethod;     // TTSSpeakerInput.FormatText(string)  (optional)

    /* ───────────── Unity ───────────── */

    private void Awake()
    {
        if (!CacheReflection())
            Debug.LogError($"{name}: TTSSpeakerReflectionAdapter could not initialise.");
    }

    /* ───────────── Public API ───────────── */

    /// <summary>
    /// Call this from any script (or wire it to a UnityEvent / Button)
    /// to speak the supplied text.
    /// </summary>
    public void Speak(string text)
    {
        if (_speakerInstance == null || _speakMethod == null)
        {
            Debug.LogWarning($"{name}: Speak() called before reflection cache ready.");
            return;
        }

        // Keep TTSSpeakerInput’s custom formatting (DATE placeholders, etc.)
        if (_formatTextMethod != null)
            text = (string)_formatTextMethod.Invoke(speakerInput, new object[] { text });
        
        Debug.Log("Speaking: " + text);

        _speakMethod.Invoke(_speakerInstance, new object[] { text });
    }

    /* ───────────── Reflection ───────────── */

    private bool CacheReflection()
    {
        if (speakerInput == null)
        {
            Debug.LogError($"{name}: SpeakerInput reference not set.");
            return false;
        }

        Type inputType   = speakerInput.GetType(); // TTSSpeakerInput
        FieldInfo fld    = inputType.GetField("_speaker",
                              BindingFlags.Instance | BindingFlags.NonPublic);
        if (fld == null) return Fail("_speaker field not found");

        _speakerInstance = fld.GetValue(speakerInput);
        if (_speakerInstance == null) return Fail("_speaker field is null");

        Type speakerType = _speakerInstance.GetType(); // TTSSpeaker
        _speakMethod = speakerType.GetMethod("Speak",
                            BindingFlags.Instance | BindingFlags.Public,
                            null,
                            new[] { typeof(string) },
                            null);
        if (_speakMethod == null) return Fail("Speak(string) method not found");

        // Optional – pick up TTSSpeakerInput.FormatText(string) so DATE tags still work
        _formatTextMethod = inputType.GetMethod("FormatText",
                               BindingFlags.Instance | BindingFlags.NonPublic);

        return true;

        bool Fail(string msg)
        {
            Debug.LogError($"{name}: {msg}");
            return false;
        }
    }
}
