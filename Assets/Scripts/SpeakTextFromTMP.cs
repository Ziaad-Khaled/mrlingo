using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SpeakTextFromTMP : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private TextMesh textMesh;
    [SerializeField] private TTSPlayer speakerAdapter;
    
    private Button m_button;
    private string textToSpeak;

    private void Awake()
    {
        m_button = GetComponent<Button>();
        if(textMesh != null)
        {
            textToSpeak = textMesh.text;
        }
        else if(textMeshPro != null)
        {
            textToSpeak = textMeshPro.text;
        }
        else
        {
            Debug.LogError("No TextMesh or TextMeshProUGUI assigned.");
        }
    }

    private void Start()
    {
        speakerAdapter = FindObjectOfType<TTSPlayer>();
        if (speakerAdapter == null)
        {
            Debug.LogError("TTSSpeakerReflectionAdapter not found in the scene.");
            return;
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
        if(textMesh != null)
        {
            textToSpeak = textMesh.text;
        }
        else if(textMeshPro != null)
        {
            textToSpeak = textMeshPro.text;
        }
        else
        {
            Debug.LogError("No TextMesh or TextMeshProUGUI assigned.");
        }
        
        if(LanguageSettings.SelectedLanguage == "German")
        {
            textToSpeak = "Das ist " + textToSpeak;
        }
        else if(LanguageSettings.SelectedLanguage == "Spanish")
        {
            textToSpeak = "Esto es " + textToSpeak;
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