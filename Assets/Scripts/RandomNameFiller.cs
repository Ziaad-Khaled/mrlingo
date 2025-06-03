using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RandomNameFiller : MonoBehaviour
{
    [Tooltip("Text asset containing names, one per line")]
    public TextAsset namesTextAsset;

    [Tooltip("TextMeshProUGUI components to fill with random names")]
    public List<TextMeshProUGUI> textFields;

    [Tooltip("TextMesh component to assign to a random TextMeshProUGUI")]
    public TextMesh myTextMesh;

    private List<string> namesList = new List<string>();

    void Start()
    {
        if (namesTextAsset == null)
        {
            Debug.LogError("No TextAsset assigned!");
            return;
        }

        // Split the text asset by new lines to get all names
        namesList = new List<string>(namesTextAsset.text.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries));

        if (namesList.Count == 0)
        {
            Debug.LogError("No names found in the TextAsset!");
            return;
        }

        FillTextFieldsRandomly();

        // Assign TextMesh text to a random TextMeshProUGUI if conditions are met
        if (myTextMesh != null && textFields.Count > 0)
        {
            int randomIndex = Random.Range(0, textFields.Count);
            textFields[randomIndex].text = myTextMesh.text;
        }
        else
        {
            if (myTextMesh == null)
            {
                Debug.LogWarning("TextMesh is not assigned.");
            }
            if (textFields.Count == 0)
            {
                Debug.LogWarning("No TextMeshProUGUI components assigned.");
            }
        }
    }

    void FillTextFieldsRandomly()
    {
        foreach (var textField in textFields)
        {
            if (textField != null)
            {
                // Pick a random name
                string randomName = namesList[Random.Range(0, namesList.Count)];
                textField.text = randomName;
            }
        }
    }
}