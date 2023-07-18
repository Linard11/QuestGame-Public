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

        selectOnOpen.Select();

        this.DOKill();
        return DOTween.Sequence(this)
                      .SetUpdate(true)
                      .Play();
    }

    public Tween Close()
    {
        if (selectPreviousOnClose && selectOnClose != null)
        {
            selectOnClose.Select();
            selectOnClose = null;
        }

        this.DOKill();
        return DOTween.Sequence(this)
                      .SetUpdate(true)
                      .OnComplete(
                          () =>
                          {
                              gameObject.SetActive(false);
                          }
                       )
                      .Play();
    }

    public Tween Show()
    {
        gameObject.SetActive(true);

        this.DOKill();
        return DOTween.Sequence(this)
                      .SetUpdate(true)
                      .Play();
    }

    public Tween Hide()
    {
        this.DOKill();
        return DOTween.Sequence(this)
                      .SetUpdate(true)
                      .OnComplete(
                          () =>
                          {
                              gameObject.SetActive(false);
                          }
                      )
                      .Play();
    }
}
