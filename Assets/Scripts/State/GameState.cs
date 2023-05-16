using System;
using System.Collections.Generic;

using UnityEngine;

public class GameState : MonoBehaviour
{
    public static event Action StateChanged;
    
    #region Inspector

    [Tooltip("A list of states representing the current trackable state of the game.")]
    [SerializeField] private List<State> states;

    #endregion

    public State Get(string id)
    {
        foreach (State state in states)
        {
            if (state.id == id)
            {
                return state;
            }
        }

        return null;
    }

    public void Add(string id, int amount, bool invokeEvent = true)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            Debug.LogError("Id of state is empty. Make sure to give each state an id.", this);
            return;
        }

        if (amount == 0)
        {
            Debug.LogWarning($"Trying to add 0 to id '{id}'. This will result in to change to the state.", this);
            return;
        }
        
        State state = Get(id);

        if (state == null)
        {
            State newState = new State(id, amount);
            states.Add(newState);
        }
        else
        {
            state.amount += amount;
        }

        if (invokeEvent)
        {
            StateChanged?.Invoke();
        }
    }

    public void Add(State state, bool invokeEvent = true)
    {
        Add(state.id, state.amount, invokeEvent);
    }

    public void Add(List<State> states)
    {
        foreach (State state in states)
        {
            Add(state, false);
        }
        StateChanged?.Invoke();
    }

    public bool CheckConditions(List<State> conditions)
    {
        foreach (State condition in conditions)
        {
            State state = Get(condition.id);
            int stateAmount = state != null ? state.amount : 0;
            if (stateAmount < condition.amount)
            {
                return false;
            }
        }

        return true;
    }
}
