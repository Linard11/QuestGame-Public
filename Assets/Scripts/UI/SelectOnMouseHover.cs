using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectOnMouseHover : MonoBehaviour, IPointerEnterHandler, IDeselectHandler
{
    private Selectable selectable;

    #region Unity Event Functions

    private void Awake()
    {
        selectable = GetComponent<Selectable>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!selectable.interactable) { return; }

        selectable.Select();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (!selectable.interactable) { return; }

        selectable.OnPointerExit(null);
    }

    #endregion
}
