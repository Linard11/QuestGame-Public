using System;

using UnityEngine;

using DG.Tweening;

public class ButtonAnimation : MonoBehaviour
{
    #region Inspector

    [Tooltip("Distance to locally move the button during the press animation, in uu.")]
    [SerializeField] private float yMovement = -0.049f;

    [Tooltip("Color to change the button to while pressing down.")]
    [SerializeField] private Color pressColor = Color.yellow;

    [Tooltip("Time in sec to hold down the button before releasing.")]
    [Min(0)]
    [SerializeField] private float downDuration = 0.3f;

    [Header("In")]
    [Tooltip("Ease of the press animation.")]
    [SerializeField] private Ease easeIn = Ease.InSine;

    [Tooltip("Duration in sec of the press animation.")]
    [Min(0)]
    [SerializeField] private float durationIn = 0.3f;

    [Header("Out")]
    [Tooltip("Ease of the release animation.")]
    [SerializeField] private Ease easeOut = Ease.OutElastic;

    [Tooltip("Duration in sec of the release animation.")]
    [Min(0)]
    [SerializeField] private float durationOut = 0.5f;

    #endregion

    private MeshRenderer meshRenderer;

    private Color originalColor;

    private Sequence sequence;
    
    #region Unity Event Functions
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalColor = meshRenderer.material.color;
    }
    #endregion

    public void PlayAnimation()
    {
        sequence.Complete(true);
        
        sequence = DOTween.Sequence();

        //press down
        sequence.Append(transform.DOLocalMoveY(yMovement, durationIn).SetRelative().SetEase(easeIn))
                .Join(meshRenderer.material.DOColor(pressColor, durationIn).SetEase(Ease.Linear))
                //wait
                .AppendInterval(downDuration)
                //release
                .Append(transform.DOLocalMoveY(-yMovement, durationOut).SetRelative().SetEase(easeOut))
                .Join(meshRenderer.material.DOColor(originalColor, durationOut).SetEase(Ease.Linear));

    }
}
