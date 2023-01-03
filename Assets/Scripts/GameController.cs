using UnityEngine;

public class GameController : MonoBehaviour
{
    private PlayerController player;
    private DialogueController dialogueController;
    
    #region Unity Event Functions

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("No player found in the scene", this);
        }
        
        dialogueController = FindObjectOfType<DialogueController>();
        
        if (dialogueController == null)
        {
            Debug.LogError("No dialogueController found in the scene", this);
        }
    }

    private void OnEnable()
    {
        DialogueController.DialogueClosed += EndDialogue;
    }

    private void Start()
    {
        EnterPlayMode();
    }

    private void OnDisable()
    {
        DialogueController.DialogueClosed -= EndDialogue;
    }

    #endregion

    #region Modes

    public void EnterPlayMode()
    {
        // In the editor: Unlock with ESC.
        Cursor.lockState = CursorLockMode.Locked;
        player.EnableInput();
    }

    public void EnterDialogueMode()
    {
        Cursor.lockState = CursorLockMode.None;
        player.DisableInput();
    }

    public void EnterCutsceneMode()
    {
        Cursor.lockState = CursorLockMode.Locked;
        player.DisableInput();
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
