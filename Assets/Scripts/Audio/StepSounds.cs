using FMODUnity;

using UnityEngine;

public class StepSounds : MonoBehaviour
{
    #region Inspector

    [SerializeField] private StudioEventEmitter stepSound;

    [SerializeField] private StudioEventEmitter landSound;

    [SerializeField] private LayerMask layerMask = 1; // 1 is Default Physics Layer.

    [SerializeField] private PhysicMaterial defaultStepSoundPhysicMaterial;

    #endregion

    #region Animation Events

    public void PlaySound(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight < 0.5f) { return; }

        switch (animationEvent.stringParameter.ToLower())
        {
            case "step":
                stepSound.Play();
                ChangeSoundByGround(stepSound); // Call AFTER Play()!
                break;
            case "land":
                landSound.Play();
                ChangeSoundByGround(landSound);
                break;
            default:
                Debug.LogWarning($"Unknown sound parameter '{animationEvent.stringParameter}'.", this);
                break;
        }
    }

    #endregion

    private void ChangeSoundByGround(StudioEventEmitter emitter)
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

        PhysicMaterial groundPhysicsMaterial = hit.collider.sharedMaterial;

        string parameterLabel = groundPhysicsMaterial != null ? groundPhysicsMaterial.name : defaultStepSoundPhysicMaterial.name;

        FMOD.RESULT result = emitter.EventInstance.setParameterByNameWithLabel("surface", parameterLabel);

        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError($"Parameter label '{parameterLabel}' could not be set. ({result.ToString()})", this);
        }
    }
}
