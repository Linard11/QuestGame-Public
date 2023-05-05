using System;

using UnityEngine;
[Serializable]
public class State
{
    public string id;

    public int amount;


    public State(string id, int amount)
    {
        this.id = id;
        this.amount = amount;
    }
}
