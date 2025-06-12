using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MCQManager : MonoBehaviour
{
    [Header("Placement")]
    [SerializeField] private Transform xrCamera;
    [SerializeField] private float distanceFromCamera = 2f;

    [Header("MCQ")]
    [SerializeField] private Button[] answerButtons;
    [SerializeField] public int correctAnswerIndex = 1;
    

    private int  selectedAnswer = -1;
    private bool answered       = false;

    private TTSSpeakerReflectionAdapter tts;

    private void Start()
    {
        PlacePanelInFrontOfUser();
        HookButtonEvents();
        
        tts = FindAnyObjectByType<TTSSpeakerReflectionAdapter>();
        if (!tts)
            Debug.LogWarning($"{name}: No TTSSpeakerReflectionAdapter found – buttons will not be spoken.");
    }

    private void PlacePanelInFrontOfUser()
    {
        if (!xrCamera) return;
        Vector3 fwd = xrCamera.forward; fwd.y = 0;
        transform.position = xrCamera.position + fwd.normalized * distanceFromCamera;
        transform.LookAt(xrCamera.position, Vector3.up);
    }

    private void HookButtonEvents()
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int idx = i;  // capture loop var
            answerButtons[i].onClick.AddListener(() => OnButtonPressed(idx));
        }
    }

    private void OnButtonPressed(int index)
    {
        if (answered) return;
        selectedAnswer = index;
        GradeAnswer();
    }

    private void GradeAnswer()
    {
        answered = true;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            Color tint =
                (i == correctAnswerIndex) ? Color.green :
                (i == selectedAnswer)     ? Color.red   :
                                             Color.white;

            PaintButton(answerButtons[i], tint);
        }

        Debug.Log("Tts: " + tts);
        
        if (tts)
        {
            var word = answerButtons[correctAnswerIndex]
                          .GetComponentInChildren<TextMeshProUGUI>()
                          .text;
            
            Debug.Log("about to speak: " + word);
            tts.Speak(word);
        }
    }

    private static void PaintButton(Button btn, Color tint)
    {
        var cb = btn.colors;
        tint.a = cb.normalColor.a;       cb.normalColor      = tint;
        tint.a = cb.highlightedColor.a;  cb.highlightedColor = tint;
        tint.a = cb.pressedColor.a;      cb.pressedColor     = tint;
        tint.a = cb.selectedColor.a;     cb.selectedColor    = tint;
        btn.colors = cb;

        if (btn.image)
        {
            var imgCol = tint;
            imgCol.a       = btn.image.color.a;
            btn.image.color = imgCol;
        }
    }
}
