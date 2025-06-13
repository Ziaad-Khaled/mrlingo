using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;

public class RandomNameFiller : MonoBehaviour
{
    [Tooltip("Text asset containing names, one per line")]
    public SerializedDictionary<string, TextAsset> namesTextAssetDictionary;

    [Tooltip("TextMesh component to assign to a random TextMeshProUGUI")]
    public TextMesh myTextMesh;

    [Tooltip("Reference to the MCQManager component")]
    public MCQManager mcqManager;

    [Tooltip("TextMeshProUGUI components to fill with random names")]
    public List<TextMeshProUGUI> textFields;

    private List<string> namesList = new List<string>();
    private TextAsset namesTextAsset;

    void Start()
    {
        namesTextAsset = namesTextAssetDictionary[LanguageSettings.SelectedLanguage];
        
        // Validate the TextAsset
        if (namesTextAsset == null)
        {
            Debug.LogError("No TextAsset assigned!");
            return;
        }

        // Parse names from the TextAsset
        namesList = new List<string>(namesTextAsset.text.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries));
        if (namesList.Count == 0)
        {
            Debug.LogError("No names found in the TextAsset!");
            return;
        }

        // Validate TextMesh and textFields
        if (myTextMesh == null)
        {
            Debug.LogError("TextMesh is not assigned.");
            return;
        }
        if (textFields.Count == 0)
        {
            Debug.LogError("No TextMeshProUGUI components assigned.");
            return;
        }

        // Choose a random index for the correct answer
        int correctIndex = Random.Range(0, textFields.Count);
        textFields[correctIndex].text = myTextMesh.text;

        // Prepare distractors by excluding the correct answer
        List<string> distractors = new List<string>(namesList);
        if (distractors.Contains(myTextMesh.text))
        {
            distractors.Remove(myTextMesh.text);
        }
        if (distractors.Count == 0)
        {
            Debug.LogError("No distractors available after removing the correct answer.");
            return;
        }

        // Fill remaining text fields with distractors
        for (int i = 0; i < textFields.Count; i++)
        {
            if (i != correctIndex && textFields[i] != null)
            {
                string randomName = distractors[Random.Range(0, distractors.Count)];
                textFields[i].text = randomName;
            }
        }

        // Communicate the correct index to MCQManager
        if (mcqManager != null)
        {
            mcqManager.correctAnswerIndex = correctIndex;
            Debug.Log($"Set correct answer index to {correctIndex}, text: {textFields[correctIndex].text}");
        }
        else
        {
            Debug.LogError("MCQManager is not assigned in RandomNameFiller.");
        }
    }
}