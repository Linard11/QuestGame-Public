using UnityEngine;

public class GameController : MonoBehaviour
{
    private PlayerController player;
    private DialogueController dialogueController;
    private MenuController menuController;

    #region Unity Event Functions

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        dialogueController = FindObjectOfType<DialogueController>();
        menuController = FindObjectOfType<MenuController>();
    }

    private void OnEnable()
    {
        DialogueController.DialogueClosed += EndDialogue;

        MenuController.BaseMenuOpening += EnterPauseMode;
        MenuController.BaseMenuClosed += EnterPlayMode;
    }

    private void Start()
    {
        EnterPlayMode();
    }

    private void OnDisable()
    {
        DialogueController.DialogueClosed -= EndDialogue;

        MenuController.BaseMenuOpening -= EnterPauseMode;
        MenuController.BaseMenuClosed -= EnterPlayMode;
    }

    #endregion

    #region Modes

    public void EnterPlayMode()
    {
        Time.timeScale = 1;
        // In the editor: Unlock with ESC.
        Cursor.lockState = CursorLockMode.Locked;
        player.EnableInput();
        menuController.enabled = true;
    }

    private void EnterDialogueMode()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        player.DisableInput();
        menuController.enabled = false;
    }

    public void EnterCutsceneMode()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        player.DisableInput();
        menuController.enabled = false;
    }

    private void EnterPauseMode()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        player.DisableInput();
        menuController.enabled = true;
    }

    #endregion

    public void StartDialogue(string dialoguePath)
    {
        EnterDialogueMode();
        dialogueController.StartDialogue(dialoguePath);
    }

    private void EndDialogue()
    {
        EnterPlayMode();
    }
}
