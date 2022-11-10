using UnityEngine;
using UnityEngine.Events;

public class Collectable : MonoBehaviour
{
    #region Inspector

    [Tooltip("Id of the collectable and the amount to be picked up.")]
    [SerializeField] private State state;

    [Tooltip("Invoked when the collectable is collected.")]
    [SerializeField] private UnityEvent onCollected;

    #endregion

    public void Collect()
    {
        onCollected.Invoke();
        FindObjectOfType<GameState>().Add(state);
        Destroy(gameObject);
    }
}
