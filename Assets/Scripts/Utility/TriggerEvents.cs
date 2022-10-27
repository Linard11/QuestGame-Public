using UnityEngine;
using UnityEngine.Events;

public class TriggerEvents : MonoBehaviour
{
    private const string UntaggedTag = "Untagged";
    private const string PlayerTag = "Player";
    
    #region Inspector

    [Tooltip("Invoked when OnTriggerEnter() is called.")]
    [SerializeField] private UnityEvent<Collider> onTriggerEnter;
    
    [Tooltip("Invoked when OnTriggerExit() is called.")]
    [SerializeField] private UnityEvent<Collider> onTriggerExit;

    [Tooltip("Enable to filter the interacting collider by a specified tag.")]
    [SerializeField] private bool filterOnTag = true;

    [Tooltip("Tag of the interacting Collider to filter on.")]
    [SerializeField] private string reactOn = PlayerTag;
    
    #endregion
    
    #region Unity Event Functions

    // Called only in the editor when changing values in the inspector.
    private void OnValidate()
    {
        // Replaces an 'empty' reactOn field with "Untagged".
        if (string.IsNullOrWhiteSpace(reactOn))
        {
            reactOn = UntaggedTag;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (filterOnTag && !other.CompareTag(reactOn))
        {
            return;
        }
        
        onTriggerEnter.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (filterOnTag && !other.CompareTag(reactOn))
        {
            return;
        }
        
        onTriggerExit.Invoke(other);
    }

    #endregion
}
