using FMODUnity;

using UnityEngine;
using UnityEngine.Events;

public class PlayerSounds : MonoBehaviour
{
    #region Inspector

    [SerializeField] private StudioEventEmitter stepSound;
    
    [SerializeField] private StudioEventEmitter landSound;

    [Tooltip("FMOD Parameter name for the step sound.")]
    [SerializeField] private string stepSoundParameterName = "surface";

    [Tooltip("Default step sound material, if no PhysicMaterial is explicitly set on the collider the player is walking on.")]
    [SerializeField] private PhysicMaterial defaultStepSoundPhysicMaterial;

    [Header("Raycast")]

    [SerializeField] private LayerMask layerMask;
    
    [Header("Unity Events")]

    [SerializeField] private UnityEvent onStep;
    
    [SerializeField] private UnityEvent onLand;
    
    #endregion
    
    #region Animation Events

    public void PlaySound(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight < 0.5f) { return; }
        
        switch (animationEvent.stringParameter.ToLowerInvariant())
        {
            case "step":
                stepSound.Play();
                ChangeStepSound(stepSound); // Call AFTER calling function to play!
                onStep.Invoke();
                break;
            case "land":
                landSound.Play();
                ChangeStepSound(landSound);
                onLand.Invoke();
                break;
            default:
                Debug.LogWarning($"Unknown sound parameter: '{animationEvent.stringParameter}'.", this);
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
        int stepSoundParameterValue = GetStepSoundParameterValue(groundPhysicMaterial);

        emitter.SetParameter(stepSoundParameterName, stepSoundParameterValue);
    }

    private int GetStepSoundParameterValue(PhysicMaterial physicMaterial)
    {
        if (physicMaterial == null)
        {
            physicMaterial = defaultStepSoundPhysicMaterial;
        }
        
        switch (physicMaterial.name)
        {
            case "Grass": // PhysicMaterial name, not the parameter label!
                return 0; // Parameter Value in FMOD!
            case "Wood":
                return 1;
            // TODO Edit & Extend with more PhysicMaterials and parameter labels in FMOD.
            default:
                Debug.LogWarning($"PhysicMaterial name '{physicMaterial.name}' is missing in the switch statement." +
                                 $" Extend your switch or check your spelling.");
                return 0;
        }
    }
}
