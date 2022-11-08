using UnityEngine;

public class InkDialogue : MonoBehaviour
{
    #region Inspector

    [Tooltip("Path to a specified knot.stitch in the ink file.")]
    [SerializeField] private string dialoguePath;

    #endregion

    public void StartDialogue()
    {
        if (string.IsNullOrWhiteSpace(dialoguePath))
        {
            Debug.LogWarning("No dialogue path defined.", this);
            return;
        }
        
        StartDialogue(dialoguePath);
    }

    public void StartDialogue(string dialoguePath)
    {
        FindObjectOfType<GameController>().StartDialogue(dialoguePath);
    }
}
