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
            GameObject currentSelection = EventSystem.current.currentSelectedGameObject;
            if (currentSelection != null)
            {
                selectOnClose = currentSelection.GetComponent<Selectable>();
            }
        }

        // Option 1
        Select(selectOnOpen);

        // Option 2
        // DOTween.Sequence(this)
        //        .AppendInterval(0.01f)
        //        .AppendCallback(() =>
        //        {
        //            Select(selectOnOpen);
        //        });

        // Option 3
        // this.StartCoroutine(DelayedSelect(selectOnOpen));

        this.DOKill();
        return DOTween.Sequence(this)
                      .SetUpdate(true);
        // Add tweens to sequence
    }

    public Tween Close()
    {
        if (selectPreviousOnClose && selectOnClose != null)
        {
            // Here also choose one of the three options how to select:
            Select(selectOnClose);
        }

        this.DOKill();
        return DOTween.Sequence(this) // Add tweens to sequence
                      .SetUpdate(true) // Needs to be independent of timescale for pause menu.
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

    // private IEnumerator DelayedSelect(Selectable newSelection)
    // {
    //     yield return null;
    //     Select(newSelection);
    // }

    private void Select(Selectable selectable)
    {
        if (selectable == null) { return; }
        selectable.Select();
    }
}
