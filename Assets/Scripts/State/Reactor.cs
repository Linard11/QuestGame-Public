using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Reactor : MonoBehaviour
{
    #region Inspector

    [SerializeField] private List<State> conditions;

    [SerializeField] private UnityEvent onFulfilled;
    
    [SerializeField] private UnityEvent onUnfulfilled;

    [SerializeField] private QuestEntry questEntry;

    #endregion

    private bool fulfilled = false;

    private GameState gameState;
    
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

        if (fulfilled == false && newFulfilled == true)
        {
            if (questEntry != null)
            {
                questEntry.SetQuestStatus(true);
            }


            onFulfilled.Invoke();
        }
        else if (fulfilled && !newFulfilled)
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
