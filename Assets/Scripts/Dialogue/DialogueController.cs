using System;
using System.Collections.Generic;
using System.Linq;

using Ink;
using Ink.Runtime;

using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueController : MonoBehaviour
{
    private const string SpeakerSeparator = ":";
    private const string EscapedColon = "::";
    private const string EscapedColonPlaceholder = "§";
    
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
        DialogueBox.ChoiceSelected += OnChoiceSelected;
    }

    private void Start()
    {
        dialogueBox.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        DialogueBox.DialogueContinued -= OnDialogueContinued;
        DialogueBox.ChoiceSelected -= OnChoiceSelected;
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
        EventSystem.current.SetSelectedGameObject(null);
        
        DialogueClosed?.Invoke();
    }

    private void ContinueDialogue()
    {
        if (IsAtEnd())
        {
            CloseDialogue();
            return;
        }

        DialogueLine line;
        if (CanContinue())
        {
            string inkLine = inkStory.Continue();
            if (string.IsNullOrWhiteSpace(inkLine))
            {
                ContinueDialogue();
                return;
            }
            line = ParseText(inkLine);
        }
        else
        {
            line = new DialogueLine();
        }
        
        line.choices = inkStory.currentChoices;

        dialogueBox.DisplayText(line);
    }

    private void OnDialogueContinued(DialogueBox _)
    {
        ContinueDialogue();
    }

    private void OnChoiceSelected(DialogueBox _, int choiceIndex)
    {
        inkStory.ChooseChoiceIndex(choiceIndex);
        ContinueDialogue();
    }

    #endregion

    #region Ink

    private DialogueLine ParseText(string inkLine)
    {
        DialogueLine line = new DialogueLine();

        inkLine = inkLine.Replace(EscapedColon, EscapedColonPlaceholder);
        
        List<string> parts = inkLine.Split(SpeakerSeparator).ToList();

        string speaker;
        string text;
        
        switch (parts.Count)
        {
            case 1:
                speaker = null;
                text = parts[0];
                break;
            case 2:
                speaker = parts[0];
                text = parts[1];
                break;
            default:
                Debug.LogWarning($"Ink dialogue line was split at more {SpeakerSeparator} than expected." +
                                 $" Please make sure to use {EscapedColon} for {SpeakerSeparator} inside text");
                goto case 2;
        }

        line.speaker = speaker?.Trim();
        line.text = text.Trim().Replace(EscapedColonPlaceholder, SpeakerSeparator);

        return line;
    }

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
    public List<Choice> choices;

    // Here we can also add other information like speaker images or sounds.
    //public Sprite speakerImage;
}
