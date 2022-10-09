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

    private void EnterPlayMode()
    {
        // In the editor: Unlock with ESC.
        Cursor.lockState = CursorLockMode.Locked;
    }

    #endregion
}
