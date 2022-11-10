using System;
using System.Collections.Generic;

using UnityEngine;

public class GameState : MonoBehaviour
{
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

    public void Add(string id, int amount)
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
    }

    public void Add(State state)
    {
        Add(state.id, state.amount);
    }

    public void Add(List<State> states)
    {
        foreach (State state in states)
        {
            Add(state);
        }
    }
}

[Serializable]
public class State
{
    [Tooltip("The id of the state. Used to identify the state.")]
    public string id;

    [Tooltip("The value of the state.")]
    public int amount;

    public State(string id, int amount)
    {
        this.id = id;
        this.amount = amount;
    }
}
