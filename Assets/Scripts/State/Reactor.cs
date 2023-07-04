using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class Reactor : MonoBehaviour
{
    #region Inspector

    [Tooltip("AND connected conditions that all need to be fulfilled.")]
    [SerializeField] private List<State> conditions;

    [SerializeField] private UnityEvent onFulfilled;

    [SerializeField] private UnityEvent onUnfulfilled;

    [Tooltip("Optional field to reference a QuestEntry, if this reactor represents a quest.")]
    [SerializeField] private QuestEntry questEntry;

    #endregion

    private GameState gameState;

    private bool fulfilled = false;

    #region Unity Event Functions

    private void Awake()
    {
        gameState = FindObjectOfType<GameState>();
    }

    private void OnEnable()
    {
        if (questEntry != null)
        {
            questEntry.gameObject.SetActive(true);
            questEntry.SetQuestStatus(false);
        }

        CheckConditions();
        GameState.StateChanged += CheckConditions;
    }

    private void OnDisable()
    {
        if (questEntry != null)
        {
            questEntry.gameObject.SetActive(false);
        }

        GameState.StateChanged -= CheckConditions;
    }

    #endregion

    private void CheckConditions()
    {
        bool newFulfilled = gameState.CheckConditions(conditions);

        // false -> true
        if (!fulfilled && newFulfilled)
        {
            if (questEntry != null)
            {
                questEntry.SetQuestStatus(true);
            }
            onFulfilled.Invoke();
        }
        // true -> false
        else if(fulfilled && !newFulfilled)
        {
            if (questEntry != null)
            {
                questEntry.SetQuestStatus(false);
            }
            onUnfulfilled.Invoke();
        }

        fulfilled = newFulfilled;
    }
}
