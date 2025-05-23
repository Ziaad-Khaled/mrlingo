using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;


public class MCQManager : MonoBehaviour
{
    public Transform xrCamera;
    public float distanceFromCamera = 2f;

    public Button[] answerButtons;
    public int correctAnswerIndex = 1; // Index starts at 0

    private int selectedAnswerIndex = -1;
    private bool hasSubmitted = false;

    void Start()
    {
        // Position panel in front of user
        if (xrCamera != null)
        {
            Vector3 forward = xrCamera.forward;
            forward.y = 0;
            transform.position = xrCamera.position + forward.normalized * distanceFromCamera;
            transform.LookAt(xrCamera);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }

        // Assign button click listeners
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i; // Capture index for lambda
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
        }
    }

    void OnAnswerSelected(int index)
    {
        selectedAnswerIndex = index;
        Debug.Log("Selected answer index: " + index);
    }

public void SubmitAnswer()
{
    Debug.Log("SubmitAnswer() called. hasSubmitted = " + hasSubmitted);

    if (hasSubmitted)
    {
        Debug.Log("Already submitted, ignoring.");
        return;
    }

    hasSubmitted = true;

    // Reset all button colors
    foreach (Button btn in answerButtons)
    {
        SetButtonColor(btn, Color.white);
    }

    if (selectedAnswerIndex == correctAnswerIndex)
    {
        Debug.Log("✅ Correct!");
        SetButtonColor(answerButtons[selectedAnswerIndex], Color.green);
    }
    else
    {
        Debug.Log("❌ Wrong answer.");
        SetButtonColor(answerButtons[selectedAnswerIndex], Color.red);
        SetButtonColor(answerButtons[correctAnswerIndex], Color.green);
    }

    Debug.Log("SubmitAnswer() finished.");
}


void Update()
{
    Debug.Log($"Update: hasSubmitted={hasSubmitted}, selectedAnswerIndex={selectedAnswerIndex}");

    // Detect trigger press (IndexTrigger)
    bool triggerPressed = false;
    InputDevice device = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

    if (device.TryGetFeatureValue(CommonUsages.triggerButton, out triggerPressed) && triggerPressed)
    {
        if (!hasSubmitted && selectedAnswerIndex != -1)
        {
            SubmitAnswer();
        }
    }
}


    private void SetButtonColor(Button button, Color color)
    {
        ColorBlock cb = button.colors;
        cb.normalColor = color;
        cb.highlightedColor = color;
        cb.pressedColor = color;
        cb.selectedColor = color;
        button.colors = cb;
    }
}
