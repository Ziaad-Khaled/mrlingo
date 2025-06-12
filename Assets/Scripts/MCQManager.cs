using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays an MCQ panel, lets the user pick an answer once,
/// and colours the buttons immediately after the click.
/// </summary>
public class MCQManager : MonoBehaviour
{
    /* ──────────────── Inspector fields ──────────────── */

    [Header("Placement")]
    [SerializeField] private Transform xrCamera;
    [SerializeField] private float distanceFromCamera = 2f;

    [Header("MCQ")]
    [SerializeField] private Button[] answerButtons;
    [SerializeField] public int correctAnswerIndex = 1;   // 0-based

    /* ──────────────── Internal state ──────────────── */

    private int  selectedAnswer = -1;
    private bool answered        = false;

    /* ──────────────── Unity lifecycle ──────────────── */

    private void Start()
    {
        PlacePanelInFrontOfUser();
        HookButtonEvents();
    }

    /* ──────────────── Helpers ──────────────── */

    /// <summary>Positions the panel a fixed distance in front of the XR camera.</summary>
    private void PlacePanelInFrontOfUser()
    {
        if (xrCamera == null) return;

        Vector3 fwd = xrCamera.forward;
        fwd.y = 0;                                             // keep panel upright
        transform.position = xrCamera.position + fwd.normalized * distanceFromCamera;
        transform.LookAt(xrCamera.position, Vector3.up);       // face the user
    }

    /// <summary>Adds a click listener to every answer button.</summary>
    private void HookButtonEvents()
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int idx = i;                                       // capture loop variable
            answerButtons[i].onClick.AddListener(() => OnButtonPressed(idx));
        }
    }

    /// <summary>Called exactly once when the user presses a button.</summary>
    private void OnButtonPressed(int index)
    {
        if (answered) return;                                  // ignore extra clicks

        selectedAnswer = index;
        GradeAnswer();
    }

    /// <summary>Colours buttons and logs the result.</summary>
    private void GradeAnswer()
    {
        answered = true;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            Color colour =
                (i == correctAnswerIndex)        ? Color.green :
                (i == selectedAnswer)            ? Color.red   :
                                                   Color.white;

            PaintButton(answerButtons[i], colour);
        }

        Debug.Log(selectedAnswer == correctAnswerIndex ? "✅ Correct" : "❌ Wrong");
    }

    /// <summary>Sets the colour block and image tint so the change shows instantly.</summary>
    private static void PaintButton(Button btn, Color tint)
    {
        var cb = btn.colors;

        // Preserve each state’s old alpha
        tint.a               = cb.normalColor.a;       cb.normalColor      = tint;
        tint.a               = cb.highlightedColor.a;  cb.highlightedColor = tint;
        tint.a               = cb.pressedColor.a;      cb.pressedColor     = tint;
        tint.a               = cb.selectedColor.a;     cb.selectedColor    = tint;

        btn.colors = cb;

        // Make the idle look change immediately without touching alpha
        if (btn.image)
        {
            var imgCol = tint;
            imgCol.a = btn.image.color.a;
            btn.image.color = imgCol;
        }
    }

}
