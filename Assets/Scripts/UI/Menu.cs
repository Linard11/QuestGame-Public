using System.Collections;

using DG.Tweening;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    #region Inspector

    [SerializeField] private Selectable selectOnOpen;

    [SerializeField] private bool selectPreviousOnClose = true;

    [SerializeField] private bool disableOnAwake = true;

    #endregion

    private Selectable selectOnClose;

    #region Unity Event Functions

    private void Awake()
    {
        if (disableOnAwake)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Open().Complete(true);
        }
    }

    #endregion

    public Tween Open()
    {
        gameObject.SetActive(true);

        if (selectPreviousOnClose)
        {
            GameObject previousSelection = EventSystem.current.currentSelectedGameObject;
            if (previousSelection != null)
            {
                selectOnClose = previousSelection.GetComponent<Selectable>();
            }
        }

        // Coroutine only necessary for select animation.
        // Select UI event is not called if it was enabled in the same frame.
        StartCoroutine(DelayedSelect(selectOnOpen));

        // DOTween here
        return DOTween.Sequence()
                      .SetUpdate(true);
    }

    public Tween Close()
    {
        if (selectPreviousOnClose && selectOnClose != null)
        {
            // selectOnClose.StartCoroutine(DelayedSelect(selectOnClose));
            Select(selectOnClose);
        }

        return DOTween.Sequence()
                      .SetUpdate(true)
                      .OnComplete(() =>
                      {
                          gameObject.SetActive(false);
                      });
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator DelayedSelect(Selectable newSelection)
    {
        yield return null; // Wait a frame
        Select(newSelection);
    }

    private void Select(Selectable newSelection)
    {
        if (newSelection == null) { return; }
        newSelection.Select();
    }
}
