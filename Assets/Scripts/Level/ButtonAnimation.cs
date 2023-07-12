using DG.Tweening;

using UnityEngine;

public class ButtonAnimation : MonoBehaviour
{
    #region Inspector

    [SerializeField] private float yMovement = -0.049f;

    #endregion

    private MeshRenderer meshRenderer;

    private Color originalColor;

    #region Unity Event Functions

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalColor = meshRenderer.sharedMaterial.color;
    }

    #endregion

    public void PlayAnimation()
    {
        this.DOComplete();
        DOTween.Sequence(this)
               .Append(transform.DOLocalMoveY(yMovement, 0.3f).SetRelative().SetEase(Ease.InSine))
               .Join(meshRenderer.material.DOColor(Color.yellow, 0.3f).SetEase(Ease.Linear))
               .AppendInterval(0.3f)
               .Append(transform.DOLocalMoveY(-yMovement, 0.5f).SetRelative().SetEase(Ease.OutElastic))
               .Join(meshRenderer.material.DOColor(originalColor, 0.3f).SetEase(Ease.Linear))
               .Play();
    }
}
