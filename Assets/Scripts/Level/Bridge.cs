using System;
using UnityEngine;
using DG.Tweening;

public class Bridge : MonoBehaviour
{
    #region Inspector
    
    [Tooltip("The platform that will move when extending/retracting.")]
    [SerializeField] private Transform platform;

    [Tooltip("Local position of the platform in the retracted state.")]
    [SerializeField] private Vector3 retractedPosition;

    [Tooltip("Local position of the platform in the extended state.")]
    [SerializeField] private Vector3 extendedPosition;

    [Tooltip("If starting in the extended state.")]
    [SerializeField] private bool startExtended;

    [Header("Animation")] 
    [Min(0)]
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
        //platform.DOLocalMove(targetPosition, speed).SetSpeedBased();
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
