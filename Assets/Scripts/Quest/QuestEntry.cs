using UnityEngine;

public class QuestEntry : MonoBehaviour
{
    #region Inspector

    [Tooltip("The icon to indicate the status of the quest.")]
    [SerializeField] private GameObject statusIcon;

    #endregion

    #region Unity Event Functions

    private void Awake()
    {
        SetQuestStatus(false);
    }

    #endregion

    public void SetQuestStatus(bool fulfilled)
    {
        statusIcon.SetActive(fulfilled);
    }
}
