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
        if (player == null)
        {
            Debug.LogError("No player found in the scene.", this);
        }
        
        dialogueController = FindObjectOfType<DialogueController>();
        if (dialogueController == null)
        {
            Debug.LogError("No DialogueController found in the scene.", this);
        }
        
        menuController = FindObjectOfType<MenuController>();
        if (menuController == null)
        {
            Debug.LogError("No MenuController found in the scene.", this);
        }
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

    public void EnterDialogueMode()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
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

    public void EnterPauseMode()
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
