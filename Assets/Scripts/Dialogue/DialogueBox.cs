using System;
using System.Collections;
using System.Collections.Generic;

using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

using Ink.Runtime;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    public static event Action<DialogueBox> DialogueContinued;
    public static event Action<DialogueBox, int> ChoiceSelected;

    #region Inspector

    [SerializeField] private TextMeshProUGUI dialogueSpeaker;

    [SerializeField] private TextMeshProUGUI dialogueText;

    [SerializeField] private Button continueButton;

    [Header("Choices")]

    [SerializeField] private Transform choicesContainer;

    [SerializeField] private Button choiceButtonPrefab;

    #endregion

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    #region Unity Event Functions

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

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
        if (line.speaker != null)
        {
            dialogueSpeaker.SetText(line.speaker);
        }

        dialogueText.SetText(line.text);

        // Read out other information such as a speaker image;

        DisplayButtons(line.choices);
    }

    private void DisplayButtons(List<Choice> choices)
    {
        Selectable newSelection;

        if (choices == null || choices.Count == 0)
        {
            ShowContinueButton(true);
            ShowChoices(false);
            newSelection = continueButton;
        }
        else
        {
            ClearChoices();
            List<Button> choiceButtons = GenerateChoices(choices);

            ShowContinueButton(false);
            ShowChoices(true);
            newSelection = choiceButtons[0];
        }

        StartCoroutine(DelayedSelect(newSelection));
    }

    private IEnumerator DelayedSelect(Selectable newSelection)
    {
        //yield return new WaitForSeconds(0.1f);
        yield return null; // Wait for next Update() / next frame

        newSelection.Select();
    }

    private void ClearChoices()
    {
        foreach (Transform child in choicesContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private List<Button> GenerateChoices(List<Choice> choices)
    {
        List<Button> choiceButtons = new List<Button>();

        for (int i = 0; i < choices.Count; i++)
        {
            Choice choice = choices[i];

            Button button = Instantiate(choiceButtonPrefab, choicesContainer);

            button.onClick.AddListener(() =>
            {
                ChoiceSelected?.Invoke(this, choice.index);
            } );

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
        choicesContainer.gameObject.SetActive(show);
    }

    #region Animations

    public Tween DOShow()
    {
        this.DOKill();

        return DOTween.Sequence(this)
                      .Append(DOMove(Vector2.zero).From(new Vector2(0, -250)))
                      .Join(DOFade(1));
    }

    public Tween DOHide()
    {
        this.DOKill();

        Sequence sequence = DOTween.Sequence(this);

        sequence.Append(DOMove(new Vector2(0, -250)).From(Vector2.zero));
        sequence.Join(DOFade(0));

        return sequence;
    }

    private TweenerCore<Vector2, Vector2, VectorOptions> DOMove(Vector2 targetPosition)
    {
        return rectTransform.DOAnchorPos(targetPosition, 0.75f).SetEase(Ease.OutBack);
    }

    private TweenerCore<float, float, FloatOptions> DOFade(float targetAlpha)
    {
        return canvasGroup.DOFade(targetAlpha, 0.75f).SetEase(Ease.InOutSine);
    }

    #endregion
}
