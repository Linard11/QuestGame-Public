using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Min(0)]
    [Tooltip("The maximum movement speed in uu/s.")]
    [SerializeField] private float movementSpeed = 8f;
    
    [Tooltip("How fast the movement is in-/decreasing.")]
    [SerializeField] private float speedChangeRate = 10f;

    private CharacterController characterController;
    
    private GameInput input;
    private InputAction moveAction;

    private Vector2 moveInput;

    private Vector3 lastMovement;

    #region Unity Events Functions

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        
        // Create new input.
        input = new GameInput();
        moveAction = input.Player.Move;

        // TODO Subscribe to input events.
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        
        Rotate(moveInput);
        Move(moveInput);
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void OnDestroy()
    {
        // TODO Unsubscribe from input events.
    }

    #endregion

    #region Movement

    private void Rotate(Vector2 moveInput)
    {
        if (moveInput != Vector2.zero)
        {
            Vector3 inputDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
            transform.rotation = Quaternion.LookRotation(inputDirection);
        }
    }

    private void Move(Vector2 moveInput)
    {
        float targetSpeed = moveInput == Vector2.zero ? 0f : movementSpeed * moveInput.magnitude;

        Vector3 currentVelocity = lastMovement;
        currentVelocity.y = 0f;
        float currentSpeed = currentVelocity.magnitude;

        // Lets the character slowly approach the targetSpeed based on their currentSpeed.
        if (Mathf.Abs(currentSpeed - targetSpeed) > 0.01f)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, speedChangeRate * Time.deltaTime);
        }
        else
        {
            currentSpeed = targetSpeed;
        }

        Vector3 movement = transform.forward * currentSpeed;

        characterController.SimpleMove(movement);

        lastMovement = movement;
    }

    #endregion
}
