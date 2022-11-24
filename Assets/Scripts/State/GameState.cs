using System;
using System.Collections.Generic;

using UnityEngine;

public class GameState : MonoBehaviour
{
    public static event Action<GameState> StateChanged;
    
    #region Inspector

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
            StateChanged?.Invoke(this);
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
        StateChanged?.Invoke(this);
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
