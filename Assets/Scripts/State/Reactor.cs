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
        
        // false -> true
        if (!fulfilled && newFulfilled)
        {
            onFulfilled.Invoke();
        }
        // true -> false
        else if(fulfilled && !newFulfilled)
        {
            onUnfulfilled.Invoke();
        }

        fulfilled = newFulfilled;
    }
}
