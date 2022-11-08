using System;
using System.Collections;
using System.Collections.Generic;

using Ink.Runtime;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    public static event Action<DialogueBox> DialogueContinued;
    public static event Action<DialogueBox, int> ChoiceSelected;
    
    #region Inspector

    [Tooltip("Text component that displays the currently speaking actor.")]
    [SerializeField] private TextMeshProUGUI dialogueSpeaker;
    
    [Tooltip("Text component that contains the displayed dialogue.")]
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Tooltip("Button to continue the dialogue.")]
    [SerializeField] private Button continueButton;

    [Header("Choices")]
    
    [Tooltip("Transform container that holds buttons for each available choice.")]
    [SerializeField] private Transform choiceContainer;

    [Tooltip("Prefab for the choice buttons.")]
    [SerializeField] private Button choiceButtonPrefab;

    #endregion

    #region Unity Event Functions

    private void Awake()
    {
        continueButton.onClick.AddListener(
            () =>
            {
                DialogueContinued?.Invoke(this);
            }
        );
    }

    private void OnEnable()
    {
        // Make sure the text boxes are clear.
        dialogueSpeaker.SetText(string.Empty);
        dialogueText.SetText(string.Empty);
    }

    #endregion

    public void DisplayText(DialogueLine dialogueLine)
    {
        // Do not update the speaker if new speaker is null.
        // Use it to keep the old speaker.
        if (dialogueLine.speaker != null)
        {
            dialogueSpeaker.SetText(dialogueLine.speaker);
        }
        dialogueText.SetText(dialogueLine.text);
        
        // Read out other information such as speaker image
        
        DisplayButtons(dialogueLine.choices);
    }

    private void DisplayButtons(List<Choice> choices)
    {
        Selectable newSelection;
        
        // If DialogueLine has no Choices, show continueButton.
        if (choices == null || choices.Count == 0)
        {
            ShowContinueButton(true);
            ShowChoices(false);
            newSelection = continueButton;
        }
        else // Show the Choices
        {
            ClearChoices();
            List<Button> choiceButtons = GenerateChoices(choices);
            
            ShowContinueButton(false);
            ShowChoices(true);
            newSelection = choiceButtons[0];
        }

        StartCoroutine(DelayedSelect(newSelection));
    }

    private void ClearChoices()
    {
        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private List<Button> GenerateChoices(List<Choice> choices)
    {
        List<Button> choiceButtons = new List<Button>(choices.Count);

        for (int i = 0; i < choices.Count; i++)
        {
            Choice choice = choices[i];

            Button button = Instantiate(choiceButtonPrefab, choiceContainer);

            int index = i;
            button.onClick.AddListener(
                () =>
                {
                    ChoiceSelected?.Invoke(this, index);
                }
            );

            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.SetText(choice.text);
            button.name = choice.text;
            
            choiceButtons.Add(button);
        }

        return choiceButtons;
    }

    private void ShowContinueButton(bool show)
    {
        continueButton.gameObject.SetActive(show);
    }

    private void ShowChoices(bool show)
    {
        choiceContainer.gameObject.SetActive(show);
    }

    private IEnumerator DelayedSelect(Selectable selectable)
    {
        yield return null;
        
        selectable.Select();
    }
}
