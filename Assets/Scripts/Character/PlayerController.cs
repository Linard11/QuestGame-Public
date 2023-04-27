using System;
using System.Numerics;
using System.Transactions;
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Vector4 = System.Numerics.Vector4;

public class PlayerController : MonoBehaviour
{
    private static readonly int MovementSpeed = Animator.StringToHash("MovementSpeed");
    private static readonly int Grounded = Animator.StringToHash("Grounded");
    
    #region Inspector

    [Header("Movement")] [Min(0)] [Tooltip("The maximume speed of the player")] [SerializeField]
    private float movementSpeed = 5f;

    [Min(0)] [Tooltip("How fast the movement speed is in- /decreasing")] [SerializeField]
    private float speedChangeRate = 10f;

    [Min(0)] [Tooltip("How fast the character rotates around it's y-axis")] [SerializeField]
    private float rotationSpeed = 10f;

    [Header("Slope Movement")]
    [Min(0)]
    [Tooltip("How much additional gravity force to apply while walking down a slope")]
    [SerializeField]
    private float pullDownForce = 5f;

    [Tooltip("Layer mask used for the raycast")] [SerializeField]
    private LayerMask raycastMask;

    [Min(0)] [Tooltip("Length of the raycast for checking for slope in uu")] [SerializeField]
    private float raycastLength = 0.5f;
    
    [Header("Camera")] [Tooltip("The focus and rotation point of the camera")] [SerializeField]
    private Transform cameraTarget;

    [Range(-89f, 0f)] [Tooltip("The minimum vertical camera angle. Lower half of the horizon")] [SerializeField]
    private float verticalCameraRotationMin = -30f;

    [Range(0f, 89f)] [Tooltip("The maximum vertical camera angle. Upper half of the horizon")] [SerializeField]
    private float verticalCameraRotationMax = 70;

    [Min(0)] [Tooltip("Sensitivity of the horizontal camera rotation. deg/s for the controller")] [SerializeField]
    private float cameraHorizontalSpeed = 200f;

    [Min(0)] [Tooltip("Sensitivity of the vertical camera rotation. deg/s for the controller")] [SerializeField]
    private float cameraVerticalSpeed = 130f;

    [Header("Mouse Settings")] [Tooltip("Additional mouse rotation speed multiplier.")] [SerializeField]
    private float mouseCameraSensitivity = 1f;

    [Tooltip("If true, the camera will rotate only if right mouse is clicked")] [SerializeField]
    private bool pressRightMouseToMove;
    
    [Header("Controller Settings")]
    [Range(0f, 2f)]
    [Tooltip("Additional controller rotation speed multiplier.")]
    [SerializeField]
    private float controllerCameraSensitivity = 1f;

    [Tooltip("Invert Y-axis for controller.")] [SerializeField]
    private bool invertY = true;

    [Header("Animation")] [Tooltip("Aniamtor of the characther mesh.")]
    [SerializeField] private Animator animator;
    
    [Min(0)]
    [Tooltip("Time in sec the character has to be in the air before the animator reacts.")]
    [SerializeField] private float coyoteTime = 0.2f;
    #endregion

    #region Private Variables

    private CharacterController characterController;

    private GameInput input;
    private InputAction moveAction;
    private InputAction lookAction;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private bool mouseRightClick;
    
    private Quaternion characterTargetRotation = Quaternion.identity;

    public Vector2 cameraRotation;
    private Vector3 lastMovement;

    /// <summary>If the character is considered to be on the ground. Delayed by coyoteTime.</summary>
    private bool isGrounded = true;
    /// <summary>Time in sec the character is in the air.</summary>
    private float airTime;

    private Interactable selectedInteractable;
    
    #endregion

    #region Unity Event Functions

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        input = new GameInput();
        moveAction = input.Player.Move;
        lookAction = input.Player.Look;

        input.Player.MouseRightClick.performed += ctx => RightClick(true);
        input.Player.MouseRightClick.canceled += ctx => RightClick(false);

        input.Player.Interact.performed += Interact;
    }

    private void OnEnable()
    {
        EnableInput();
    }

    private void Update()
    {
        ReadInput();

        Rotate(moveInput);
        Move(moveInput);

        UpdateAnimator();
    }

    private void LateUpdate()
    {
        RotateCamera(lookInput);
    }

    private void OnDisable()
    {
        DisableInput();
    }

    private void OnDestroy()
    {
        input.Player.Interact.performed -= Interact;
    }

    #region Physics

    private void OnTriggerEnter(Collider other)
    {
        TrySelectInteractable(other);
    }

    private void OnTriggerExit(Collider other)
    {
        TryDeselectInteractable(other);
    }

    #endregion
    
    #endregion

    #region Input

    public void EnableInput()
    {
        input.Enable();
    }
    
    public void DisableInput()
    {
        input.Disable();
    }
    
    void ReadInput()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        lookInput = lookAction.ReadValue<Vector2>();
    }

    void RightClick(bool isClicked)
    {
        mouseRightClick = isClicked;
    }
    
    #endregion

    #region Movement
    private void Rotate(Vector2 moveInput)
    {
        if (moveInput != Vector2.zero)
        {
            Vector3 inputDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;

            Vector3 worldInputDirection = cameraTarget.TransformDirection(inputDirection);
            worldInputDirection.y = 0;

            characterTargetRotation = Quaternion.LookRotation(worldInputDirection);
        }

        if (Quaternion.Angle(transform.rotation, characterTargetRotation) > 0.1f)
        {
            transform.rotation =
                Quaternion.Slerp(transform.rotation, characterTargetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            transform.rotation = characterTargetRotation;
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

        Vector3 targetDirection = characterTargetRotation * Vector3.forward;

        Vector3 movement = targetDirection * currentSpeed;
        characterController.SimpleMove(movement);

        if (Physics.Raycast(transform.position + Vector3.up * 0.01f, Vector3.down, out RaycastHit hit, raycastLength,
                raycastMask, QueryTriggerInteraction.Ignore))
        {
            if (Vector3.ProjectOnPlane(movement, hit.normal).y < 0)
            {
                characterController.Move(Vector3.down * (pullDownForce * Time.deltaTime));
            }
        }
        
        
        
        lastMovement = movement;
    }

    #endregion

    #region Camera

    private void RotateCamera(Vector2 lookInput)
    {
        if (lookInput != Vector2.zero)
        {
            bool isMouseLook = IsMouseLook();

            float deltaTimeMultiplier = isMouseLook ? 1 : Time.deltaTime;

            float sensitivity = isMouseLook ? mouseCameraSensitivity : controllerCameraSensitivity;

            lookInput *= deltaTimeMultiplier * sensitivity * ((mouseRightClick && isMouseLook && pressRightMouseToMove) || !isMouseLook || !pressRightMouseToMove? 1 : 0);

            cameraRotation.x += lookInput.y * cameraVerticalSpeed * (!isMouseLook && invertY ? -1 : 1);

            cameraRotation.y += lookInput.x * cameraHorizontalSpeed;

            cameraRotation.x = NormalizeAngle(cameraRotation.x);
            cameraRotation.y = NormalizeAngle(cameraRotation.y);

            cameraRotation.x = Mathf.Clamp(cameraRotation.x, verticalCameraRotationMin, verticalCameraRotationMax);
        }

        cameraTarget.rotation = Quaternion.Euler(cameraRotation.x, cameraRotation.y, 0f);
    }

    private float NormalizeAngle(float angle)
    {
        angle %= 360;

        if (angle < 0)
        {
            angle += 360;
        }

        if (angle > 180)
        {
            angle -= 360;
        }

        return angle;
    }

    private bool IsMouseLook()
    {
        if (lookAction.activeControl == null)
        {
            return true;
        }

        //bool mouseInput = lookAction.activeControl.name == "delta";
        return lookAction.activeControl.name == "delta";
    }

    #endregion
    
    #region Ground Check

    /// <summary>
    /// Measures for how long the character is not on the ground and updates isGrounded delayed by coyoteTime when becoming airborne.
    /// </summary>
    private void CheckGround()
    {
        if (characterController.isGrounded)
        {
            // Reset the "stopwatch".
            airTime = 0;
        }
        else
        {
            // Count up the "stopwatch".
            airTime += Time.deltaTime;
        }

        // Set grounded to true if on the ground (0) or the airTime is still less than the coyoteTime. 
        isGrounded = airTime < coyoteTime;
    }

    #endregion
    
    #region Animator

    void UpdateAnimator()
    {
        Vector3 velocity = lastMovement;
        velocity.y = 0;
        float speed = velocity.magnitude;
        
        animator.SetFloat(MovementSpeed, speed);
        animator.SetBool(Grounded, isGrounded);
    }
    
    #endregion

    #region Interaction

    private void Interact(InputAction.CallbackContext _)
    {
        if (selectedInteractable != null)
        {
            selectedInteractable.Interact();
        }
    }

    private void TrySelectInteractable(Collider other)
    {
        Interactable interactable = other.GetComponent<Interactable>();

        if (interactable == null) { return; }

        if (selectedInteractable != null)
        {
            selectedInteractable.Deselect();
        }

        selectedInteractable = interactable;
        selectedInteractable.Select();
    }
    
    private void TryDeselectInteractable(Collider other)
    {
        Interactable interactable = other.GetComponent<Interactable>();

        if (interactable == null) { return; }

        if (interactable == selectedInteractable)
        {
            selectedInteractable.Deselect();
            selectedInteractable = null;
        }
    }

    #endregion
}
