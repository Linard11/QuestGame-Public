using System;
using System.Numerics;
using System.Transactions;
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Vector4 = System.Numerics.Vector4;

public class PlayerController : MonoBehaviour
{
    [Min(0)]
    [Tooltip("The maximume speed of the player")]
    [SerializeField] private float movementSpeed = 5f;

    [Min(0)]
    [Tooltip("How fast the movement speed is in- /decreasing")]
    [SerializeField] private float speedChangeRate = 10f;

    private CharacterController characterController;
    
    private GameInput input;
    private InputAction moveAction;

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

    private void Rotate(Vector2 moveInput)
    {
        if (moveInput != Vector2.zero)
        {
            Vector3 inputDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
            
            gameObject.transform.rotation = Quaternion.LookRotation(inputDirection);
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
