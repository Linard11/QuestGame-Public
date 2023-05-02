using System;

using Ink;
using Ink.Runtime;

using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public static event Action DialogueOpened;
    public static event Action DialogueClosed;
    
    #region Inspector

    [Header("Ink")]
    
    [SerializeField] private TextAsset inkAsset;

    [Header("UI")]

    [SerializeField] private DialogueBox dialogueBox;

    #endregion

    private Story inkStory;

    #region Unity Event Functions

    private void Awake()
    {
        inkStory = new Story(inkAsset.text);
        inkStory.onError += OnInkError;
    }

    private void OnEnable()
    {
        DialogueBox.DialogueContinued += OnDialogueContinued;
    }

    private void Start()
    {
        dialogueBox.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        DialogueBox.DialogueContinued -= OnDialogueContinued;
    }

    private void OnDestroy()
    {
        inkStory.onError -= OnInkError;
    }

    #endregion

    #region Dialogue Lifecycle

    public void StartDialogue(string dialoguePath)
    {
        OpenDialogue();
        
        // Like '-> knot' in ink.
        inkStory.ChoosePathString(dialoguePath);
        ContinueDialogue();
    }

    private void OpenDialogue()
    {
        dialogueBox.gameObject.SetActive(true);
        
        DialogueOpened?.Invoke();
    }

    private void CloseDialogue()
    {
        dialogueBox.gameObject.SetActive(false);
        // TODO Clean up
        
        DialogueClosed?.Invoke();
    }

    private void ContinueDialogue()
    {
        if (IsAtEnd())
        {
            CloseDialogue();
            return;
        }

        DialogueLine line = new DialogueLine();
        if (CanContinue())
        {
            string inkLine = inkStory.Continue();
            if (string.IsNullOrWhiteSpace(inkLine))
            {
                ContinueDialogue();
                return;
            }

            // TODO Parse text
            line.text = inkLine;
        }
        
        dialogueBox.DisplayText(line);
    }

    private void OnDialogueContinued(DialogueBox _)
    {
        ContinueDialogue();
    }

    #endregion

    #region Ink

    private bool CanContinue()
    {
        return inkStory.canContinue;
    }

    private bool HasChoices()
    {
        return inkStory.currentChoices.Count > 0;
    }

    private bool IsAtEnd()
    {
        return !CanContinue() && !HasChoices();
    }

    private void OnInkError(string message, ErrorType type)
    {
        switch (type)
        {
            case ErrorType.Author:
                break;
            case ErrorType.Warning:
                Debug.LogWarning(message);
                break;
            case ErrorType.Error:
                Debug.LogError(message);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    #endregion
}

public struct DialogueLine
{
    public string speaker;
    public string text;
    
    // Here we can also add other information like speaker images or sounds.
    //public Sprite speakerImage;
}
