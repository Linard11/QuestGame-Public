using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    #region Unity Event Functions

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        transform.rotation = mainCamera.transform.rotation;
    }

    #endregion
}
