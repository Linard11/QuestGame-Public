using System;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    public static event Action<DialogueBox> DialogueContinued;

    #region Inspector

    [SerializeField] private TextMeshProUGUI dialogueSpeaker;
    
    [SerializeField] private TextMeshProUGUI dialogueText;

    [SerializeField] private Button continueButton;

    [Header("Choices")]

    [SerializeField] private Transform choicesContainer;

    [SerializeField] private Button choiceButtonPrefab;

    #endregion

    #region Unity Event Functions

    private void Awake()
    {
        continueButton.onClick.AddListener(() =>
        {
            DialogueContinued?.Invoke(this);
        } );
    }

    private void OnEnable()
    {
        dialogueSpeaker.SetText(string.Empty); // ""
        dialogueText.SetText(string.Empty);
    }

    #endregion

    public void DisplayText(DialogueLine line)
    {
        if (dialogueSpeaker != null)
        {
            dialogueSpeaker.SetText(line.speaker);
        }
        
        dialogueText.SetText(line.text);
        
        // Read out other information such as a speaker image;
    }
}
