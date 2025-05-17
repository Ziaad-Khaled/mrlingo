using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulateButtonClick : MonoBehaviour
{
    public Button targetButton;
    public MCQManager mcqManager; // Reference to MCQManager
    public float delay = 2f;
    private bool hasClicked = false;

void Update()
{
    if (!hasClicked && Time.time >= delay)
    {
        Debug.Log("Simulating button click...");
        targetButton.onClick.Invoke();  // Triggers OnAnswerSelected and logs "Selected answer index: 0"
        Debug.Log("Submitting answer...");
        mcqManager.SubmitAnswer();      // Should print the SubmitAnswer logs above
        hasClicked = true;
    }
}

}
