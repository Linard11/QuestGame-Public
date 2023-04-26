using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    #region Unity Event Functions

    private void Start()
    {
        EnterPlayMode();
    }

    #endregion

    #region Modes

    void EnterPlayMode()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    #endregion
}
