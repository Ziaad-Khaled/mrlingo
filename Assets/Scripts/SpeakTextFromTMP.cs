using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SpeakTextFromTMP : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;
    private Button m_button;

    private TTSPlayer speakerAdapter;

    private void Awake()
    {
        m_button = GetComponent<Button>();
    }

    private void Start()
    {
        speakerAdapter = FindObjectOfType<TTSPlayer>();
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
        string textToSpeak = textMeshPro.text;
        if(LanguageSettings.SelectedLanguage == "German")
        {
            textToSpeak = "Das ist " + textMeshPro.text;
        }
        else if(LanguageSettings.SelectedLanguage == "Spanish")
        {
            textToSpeak = "Esto es " + textMeshPro.text;
        }
        
        string language;
        
        if (LanguageSettings.SelectedLanguage == "German")
        {
            language = "de";
        }
        else if (LanguageSettings.SelectedLanguage == "Spanish")
        {
            language = "es";
        }
        else
        {
            language = "en"; // Default to English
        }
        
        speakerAdapter.StartCoroutine(speakerAdapter.Speak(textToSpeak, language));
    }
}