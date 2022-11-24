using System.Collections.Generic;

using UnityEngine;

public class StateUpdater : MonoBehaviour
{
    #region Inspector

    [Tooltip("List of states to be added to the GameState when calling UpdateStates().")]
    [SerializeField] private List<State> stateUpdates;

    #endregion

    public void UpdateStates()
    {
        FindObjectOfType<GameState>().Add(stateUpdates);
    }
}
