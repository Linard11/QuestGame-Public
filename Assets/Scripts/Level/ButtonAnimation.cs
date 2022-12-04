using DG.Tweening;

using UnityEngine;

public class ButtonAnimation : MonoBehaviour
{
    #region Inspector

    [SerializeField] private float yMovement = -0.049f;

    [Min(0)]
    [SerializeField] private float downDuration = 0.3f;

    [Header("In")]

    [SerializeField] private Ease easeIn = Ease.InSine;

    [SerializeField] private float durationIn = 0.3f;

    [Header("Out")]

    [SerializeField] private Ease easeOut = Ease.OutElastic;

    [SerializeField] private float durationOut = 0.5f;

    #endregion

    private Sequence sequence;

    public void PlayAnimation()
    {
        sequence.Complete(true);

        sequence = DOTween.Sequence();

                // Press down
        sequence.Append(transform.DOLocalMoveY(yMovement, durationIn).SetRelative().SetEase(easeIn))
                // Wait
                .AppendInterval(downDuration)
                // Release
                .Append(transform.DOLocalMoveY(-yMovement, durationOut).SetRelative().SetEase(easeOut));

        // Not needed because autoplay is on by default.
        sequence.Play();
    }
}
