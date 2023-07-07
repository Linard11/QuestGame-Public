using UnityEngine;
using UnityEngine.Events;

public class TriggerEvents : MonoBehaviour
{
    private const string PlayerTag = "Player";
    private const string NoTag = "Untagged";

    #region Inspector

    [SerializeField] private UnityEvent<Collider> onTriggerEnter;

    [SerializeField] private UnityEvent<Collider> onTriggerExit;

    [Tooltip("Enable to filter the interacting collider by a specified tag.")]
    [SerializeField] private bool filterOnTag = true;

    [Tooltip("Tag of the interacting collider to filter on.")]
    [SerializeField] private string reactOn = PlayerTag;

    [Header("Advanced")]

    [Tooltip("Treat overlapping triggers as one, by only executing the UnityEvents on the first enter/last exit.")]
    [SerializeField] private bool combineTriggers = true;

    #endregion

    private int triggerCount = 0;

    #region Unity Event Functions

    private void OnValidate()
    {
        // Replaces an 'empty' reactOn filed with "Untagged".
        if (string.IsNullOrWhiteSpace(reactOn))
        {
            reactOn = NoTag;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (filterOnTag && !other.CompareTag(reactOn)) { return; }

        triggerCount++;
        if (triggerCount < 1)
        {
            triggerCount = 1;
        }

        if (combineTriggers && triggerCount != 1) { return; }

        onTriggerEnter.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (filterOnTag && !other.CompareTag(reactOn)) { return; }

        triggerCount--;
        if (triggerCount < 0)
        {
            triggerCount = 0;
        }

        if (combineTriggers && triggerCount != 0) { return; }

        onTriggerExit.Invoke(other);
    }

    #endregion
}
