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

    #endregion

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!selectable.interactable) { return; }
        
        selectable.Select();
    }

    // Handling for deselect is necessary when moving the selection with the keyboard/controller
    // while the mouse is still over the button.
    public void OnDeselect(BaseEventData eventData)
    {
        if (!selectable.interactable) { return; }
        
        // Communicate to the selectable that the pointer has left the selectable
        // so it is ignored until the next OnPointerEnter().
        selectable.OnPointerExit(null);
    }
}
