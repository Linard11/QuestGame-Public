using System;
using System.Collections.Generic;
using System.Linq;

using DG.Tweening;

using Ink;
using Ink.Runtime;

using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueController : MonoBehaviour
{
    private const string SpeakerSeparator = ":";
    private const string EscapedColon = "::";
    private const string EscapedColonPlaceholder = "$";
    
    public static event Action DialogueOpened;
    public static event Action DialogueClosed;

    public static event Action<string> InkEvent;
    
    #region Inspector

    [Header("Ink")]
    [Tooltip("Compiled ink text asset.")]
    [SerializeField] private TextAsset inkAsset;

    [Header("UI")]
    [Tooltip("DialogueBox to display the dialogue in.")]
    [SerializeField] private DialogueBox dialogueBox;

    #endregion

    private GameState gameState;
    
    private Story inkStory;

    #region Unity Event Functions

    private void Awake()
    {
        gameState = FindObjectOfType<GameState>();
        
        // Initialize Ink.
        inkStory = new Story(inkAsset.text);
        inkStory.onError += OnInkError;
        inkStory.BindExternalFunction<string>("Unity_Event", Unity_Event);
        inkStory.BindExternalFunction<string>("Get_State", Get_State);
        inkStory.BindExternalFunction<string, int>("Add_State", Add_State);
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
        dialogueBox.DOShow();
        
        DialogueOpened?.Invoke();
    }

    private void CloseDialogue()
    {
        // Deselect everything in the UI.
        EventSystem.current.SetSelectedGameObject(null);

        dialogueBox.DOHide()
                   .OnComplete(() =>
                   {
                       dialogueBox.gameObject.SetActive(false);
                   });
        
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
            // Skip empty lines
            if (string.IsNullOrWhiteSpace(inkLine))
            {
                ContinueDialogue();
                return;
            }
            line = ParseText(inkLine, inkStory.currentTags);
        }
        else
        {
            line = new DialogueLine();
        }

        line.choices = inkStory.currentChoices;
        
        dialogueBox.DisplayText(line);
    }

    private void SelectChoice(int choiceIndex)
    {
        inkStory.ChooseChoiceIndex(choiceIndex);
        ContinueDialogue();
    }

    private void OnDialogueContinued(DialogueBox _)
    {
        ContinueDialogue();
    }

    private void OnChoiceSelected(DialogueBox _, int choiceIndex)
    {
        SelectChoice(choiceIndex);
    }

    #endregion

    #region Ink

    private bool CanContinue()
    {
        return inkStory.canContinue;
    }

    private bool HasChoice()
    {
        return inkStory.currentChoices.Count > 0;
    }

    private bool IsAtEnd()
    {
        return !CanContinue() && !HasChoice();
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

    private DialogueLine ParseText(string inkLine, List<string> tags)
    {
        // Replace :: with $ as a placeholder to prevent splitting.
        inkLine = inkLine.Replace(EscapedColon, EscapedColonPlaceholder);
        
        // Split string into parts at :
        List<string> parts = inkLine.Split(SpeakerSeparator).ToList();

        string speaker = null;
        string text = string.Empty;

        switch (parts.Count)
        {
            case 1:
                text = parts[0];
                break;
            case 2:
                speaker = parts[0];
                text = parts[1];
                break;
            default:
                Debug.LogWarning($"Ink dialogue line was split at more {SpeakerSeparator} than expected. Please make sure to use {EscapedColon} for {SpeakerSeparator}");
                goto case 2;
        }
        
        DialogueLine line = new DialogueLine();

        // Trim whitespaces on both end.
        line.speaker = speaker?.Trim();
        // Replace $ back to : for display on the UI.
        line.text = text.Replace(EscapedColonPlaceholder, SpeakerSeparator).Trim();

        // foreach (string tag in tags)
        // {
        //     switch (tag)
        //     {
        //         case "thought":
        //             // DO STUFF
        //             break;
        //         case "cry":
        //             // DO STUFF
        //             break;
        //     }
        // }
        
        if (tags.Contains("thought"))
        {
            line.text = $"<i>{line.text}</i>";
        }
        
        return line;
    }

    private void Unity_Event(string eventName)
    {
        InkEvent?.Invoke(eventName);
    }

    private object Get_State(string id)
    {
        State state = gameState.Get(id);
        return state != null ? state.amount : 0;
    }

    private void Add_State(string id, int amount)
    {
        gameState.Add(id, amount);
    }

    #endregion
}

public struct DialogueLine
{
    public string speaker;
    public string text;
    public List<Choice> choices;

    // Here we can also add other information like
    // speaker images or sounds.
}
