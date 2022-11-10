using UnityEngine;
using UnityEngine.Events;

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
            Debug.LogError("No player found in scene.", this);
        }
        
        dialogueController = FindObjectOfType<DialogueController>();

        if (dialogueController == null)
        {
            Debug.LogError("No dialogueController found in scene.", this);
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

    private void EnterPlayMode()
    {
        // In the editor: Unlock with ESC.
        Cursor.lockState = CursorLockMode.Locked;
        player.EnableInput();
    }

    private void EnterDialogueMode()
    {
        Cursor.lockState = CursorLockMode.None;
        player.DisableInput();
    }

    #endregion

    public void StartDialogue(string dialoguePath, UnityEvent onEndDialogue)
    {
        EnterDialogueMode();
        dialogueController.StartDialogue(dialoguePath, onEndDialogue);
    }

    private void EndDialogue()
    {
        EnterPlayMode();
    }
}
