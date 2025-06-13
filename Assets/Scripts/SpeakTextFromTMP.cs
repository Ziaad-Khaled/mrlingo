using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SpeakTextFromTMP : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;
    private Button m_button;

    private TTSSpeakerReflectionAdapter speakerAdapter;

    private void Awake()
    {
        m_button = GetComponent<Button>();
    }

    private void Start()
    {
        speakerAdapter = FindObjectOfType<TTSSpeakerReflectionAdapter>();
        if (speakerAdapter == null)
        {
            Debug.LogError("TTSSpeakerReflectionAdapter not found in the scene.");
            return;
        }

        if (textMeshPro == null)
        {
            Debug.LogError("TextMeshProUGUI not assigned.");
        }
    }
    
    private void OnEnable()
    {
        m_button.onClick.AddListener(Speak);
    }
    
    private void OnDisable()
    {
        m_button.onClick.RemoveListener(Speak);
    }

    public void Speak()
    {
        
        var textToSpeak = textMeshPro.text;
        speakerAdapter.Speak(textToSpeak);
    }
}