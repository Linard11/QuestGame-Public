using FMODUnity;

using UnityEngine;
using UnityEngine.Events;

using Debug = UnityEngine.Debug;

public class StepSounds : MonoBehaviour
{
    #region Inspector

    [SerializeField] private StudioEventEmitter stepSound;

    [SerializeField] private StudioEventEmitter landSound;

    [SerializeField] private string stepSoundParameterName = "surface";

    [SerializeField] private PhysicMaterial defaultStepSoundPhysicMaterial;

    [Header("Unity Events")]

    [SerializeField] private UnityEvent onStep;

    [SerializeField] private UnityEvent onLand;

    [Header("Raycast")]

    [SerializeField] private LayerMask layerMask = 1; // 1 is default Physics layer.

    #endregion

    #region Animation Events

    public void PlaySound(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight < 0.5f) { return; }

        switch (animationEvent.stringParameter.ToLower())
        {
            case "step":
                stepSound.Play();
                ChangeStepSound(stepSound); // Call AFTER Play()!
                onStep.Invoke();
                break;
            case "land":
                landSound.Play();
                ChangeStepSound(landSound); // Call AFTER Play()!
                onLand.Invoke();
                break;
            default:
                Debug.LogWarning($"Unknown sound parameter '{animationEvent.stringParameter}'.", this);
                break;
        }
    }

    #endregion

    private void ChangeStepSound(StudioEventEmitter emitter)
    {
        if (!Physics.Raycast(transform.position + Vector3.up * 0.01f,
                            Vector3.down,
                            out RaycastHit hit,
                            5f,
                            layerMask,
                            QueryTriggerInteraction.Ignore))
        {
            return;
        }

        PhysicMaterial groundPhysicMaterial = hit.collider.sharedMaterial;

        string parameterLabel = groundPhysicMaterial != null ? groundPhysicMaterial.name : defaultStepSoundPhysicMaterial.name;

        FMOD.RESULT result = emitter.EventInstance.setParameterByNameWithLabel(stepSoundParameterName, parameterLabel);

        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError($"Parameter label '{parameterLabel}' could not be set. ({result.ToString()})");
        }
    }
}
