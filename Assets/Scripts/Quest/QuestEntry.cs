using UnityEngine;

public class QuestEntry : MonoBehaviour
{
    #region Inspector

    [SerializeField] private GameObject statusIcon;

    #endregion

    public void SetQuestStatus(bool fulfilled)
    {
        statusIcon.SetActive(fulfilled);
    }
}
