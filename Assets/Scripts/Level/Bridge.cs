using System;

using DG.Tweening;

using UnityEngine;

public class Bridge : MonoBehaviour
{
    #region Inspector

    [SerializeField] private Transform platform;

    [SerializeField] private Vector3 retractedPosition;

    [SerializeField] private Vector3 extendedPosition;

    [SerializeField] private bool startExtended;

    [SerializeField] private float moveDuration = 1f;

    [SerializeField] private Ease ease = DOTween.defaultEaseType;

    #endregion

    private bool isExtended;

    #region Unity Event Functions

    private void Awake()
    {
        isExtended = startExtended;
        platform.localPosition = startExtended ? extendedPosition : retractedPosition;
    }

    #endregion

    public void Toggle()
    {
        if (isExtended)
        {
            Retract();
        }
        else
        {
            Extend();
        }
    }

    public void Extend()
    {
        isExtended = true;
        MovePlatform(extendedPosition);
    }

    public void Retract()
    {
        isExtended = false;
        MovePlatform(retractedPosition);
    }

    private void MovePlatform(Vector3 targetPosition)
    {
        float speed = (retractedPosition - extendedPosition).magnitude / moveDuration;
        platform.DOKill();
        platform.DOLocalMove(targetPosition, speed)
                .SetSpeedBased()
                .SetEase(ease)
                .OnComplete(() =>
                {
                    Debug.Log("End of bridge movement");
                });
        Debug.Log("Start of bridge movement");
    }

}
