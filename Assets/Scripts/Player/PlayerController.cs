using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    private InputAction moveAction;
    
    #region Inspector
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float speedChangeRate = 10f;
    #endregion

    private CharacterController characterController;
    
    private GameInput input;
    private Vector2 moveInput;
    private Vector3 lastMovement;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        
        input = new GameInput();

        moveAction = input.Player.Move;
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

    }

    void Rotate(Vector2 moveInput)
    {
        if (moveInput != Vector2.zero)
        {
            Vector3 inputDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
            transform.rotation = Quaternion.LookRotation((inputDirection));
        }
    }

    void Move(Vector2 moveInput)
    {
        float targetSpeed = moveInput == Vector2.zero ? 0 : movementSpeed * moveInput.magnitude;
        Vector3 currentVelocity = lastMovement;
        currentVelocity.y = 0;
        float currentSpeed = currentVelocity.magnitude;

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
}
