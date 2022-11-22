using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class Reactor : MonoBehaviour
{
    #region Inspector

    [Tooltip("AND connected conditions that all need to be fulfilled.")]
    [SerializeField] private List<State> conditions;

    [Tooltip("Invoked when the conditions become all fulfilled.")]
    [SerializeField] private UnityEvent onFulfilled;

    [Tooltip("Invoked when the conditions return to being unfulfilled.")]
    [SerializeField] private UnityEvent onUnfulfilled;

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
        CheckConditions();
        GameState.StateChanged += CheckConditions;
    }

    private void OnDisable()
    {
        GameState.StateChanged -= CheckConditions;
    }

    #endregion

    private void CheckConditions()
    {
        bool newFulfilled = gameState.CheckConditions(conditions);
        
        // From false -> true
        if (!fulfilled && newFulfilled)
        {
            onFulfilled.Invoke();
        }
        // From true -> false
        else if (fulfilled && !newFulfilled)
        {
            onUnfulfilled.Invoke();
        }

        fulfilled = newFulfilled;
    }
}
