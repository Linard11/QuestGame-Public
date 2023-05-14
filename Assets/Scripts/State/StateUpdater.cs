using System.Collections.Generic;

using UnityEngine;

public class StateUpdater : MonoBehaviour
{
    #region Inspector
    
    [SerializeField] private List<State> stateUpdates;

    #endregion

    public void UpdateStates()
    {
        FindObjectOfType<GameState>().Add(stateUpdates);
    }
    
}
