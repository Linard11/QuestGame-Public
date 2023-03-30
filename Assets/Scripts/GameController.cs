using UnityEngine;

/// <summary>
/// Controls the overall state of the game, handles the activation/deactivation of the Player and UI.
/// </summary>
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
        // Lock the cursor to the center of the screen & hide it.
        // In the editor: Unlock with ESC.
        Cursor.lockState = CursorLockMode.Locked;
    }

    #endregion
}