using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private static readonly int MovementSpeed = Animator.StringToHash("MovementSpeed");

    private static readonly int Grounded = Animator.StringToHash("Grounded");

    #region Inspector

    [Header("Movement")]

    [Min(0)]
    [Tooltip("The maximum speed of the player in uu/s.")]
    [SerializeField] private float movementSpeed = 5f;

    [Min(0)]
    [Tooltip("How fast the movement speed is in-/decreasing.")]
    [SerializeField] private float speedChangeRate = 10f;

    [Min(0)]
    [Tooltip("How fast the character rotates around it's y-axis.")]
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Slope Movement")]

    [Min(0)]
    [Tooltip("How much additional gravity force to apply while walking down a slope. In uu/s.")]
    [SerializeField] private float pullDownForce = 5f;

    [Tooltip("Layer mask used for the raycast while walking on a slope.")]
    [SerializeField] private LayerMask raycastMask = 1; // 1 is "Default" Physics Layer.

    [Min(0)]
    [Tooltip("Length of the raycast for checking for slopes in uu.")]
    [SerializeField] private float raycastLength = 0.5f;

    [Header("Camera")]

    [Tooltip("The focus and rotation point of the camera.")]
    [SerializeField] private Transform cameraTarget;

    [Range(-89f, 0)]
    [Tooltip("The minimum vertical camera angle. Lower half of the horizon.")]
    [SerializeField] private float verticalCameraRotationMin = -30f;

    [Range(0, 89f)]
    [Tooltip("The maximum vertical camera angle. Upper half of the horizon.")]
    [SerializeField] private float verticalCameraRotationMax = 70f;

    [Min(0)]
    [Tooltip("Sensitivity of the horizontal camera rotation. deg/s for controller.")]
    [SerializeField] private float cameraHorizontalSpeed = 200f;

    [Min(0)]
    [Tooltip("Sensitivity of the vertical camera rotation. deg/s for controller.")]
    [SerializeField] private float cameraVerticalSpeed = 130f;

    [Header("Mouse Settings")]

    // TODO Put in UI Settings.
    [Range(0, 2f)]
    [Tooltip("Additional mouse rotation speed multiplier.")]
    [SerializeField] private float mouseCameraSensitivity = 1f;

    [Header("Controller Settings")]

    // TODO Put in UI Settings.
    [Range(0, 2f)]
    [Tooltip("Additional controller rotation speed multiplier.")]
    [SerializeField] private float controllerCameraSensitivity = 1f;

    // TODO Put in UI Settings.
    [Tooltip("Invert Y-axis for controller.")]
    [SerializeField] private bool invertY = true;

    [Header("Animations")]

    [Tooltip("Animator of the character mesh.")]
    [SerializeField] private Animator animator;

    [Min(0)]
    [Tooltip("Time in sec the character has to be in the air before the animator reacts.")] 
    [SerializeField] private float coyoteTime = 0.2f;

    #endregion

    private CharacterController characterController;

    private GameInput input;
    private InputAction lookAction;
    private InputAction moveAction;

    private Vector2 lookInput;
    private Vector2 moveInput;

    private Quaternion characterTargetRotation = Quaternion.identity;

    private Vector2 cameraRotation;

    private Vector3 lastMovement;

    private bool isGrounded = true;
    private float airTime;

    private Interactable selectedInteractable;

    #region Unity Events Functions

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        input = new GameInput();
        lookAction = input.Player.Look;
        moveAction = input.Player.Move;

        // Subscribe to input events.
        input.Player.Interact.performed += Interact;

        characterTargetRotation = transform.rotation;
        cameraRotation = cameraTarget.rotation.eulerAngles;
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void Update()
    {
        ReadInput();

        Rotate(moveInput);
        Move(moveInput);

        CheckGround();

        UpdateAnimator();
    }

    private void LateUpdate()
    {
        RotateCamera(lookInput);
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void OnDestroy()
    {
        // Unsubscribe to input events.
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

    private void ReadInput()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        lookInput = lookAction.ReadValue<Vector2>();
    }

    #endregion

    #region Movement

    private void Rotate(Vector2 moveInput)
    {
        if (moveInput != Vector2.zero)
        {
            Vector3 inputDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;

            Vector3 worldInputDirections = cameraTarget.TransformDirection(inputDirection);
            worldInputDirections.y = 0;

            characterTargetRotation = Quaternion.LookRotation(worldInputDirections);
        }

        if (Quaternion.Angle(transform.rotation, characterTargetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, characterTargetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            transform.rotation = characterTargetRotation;
        }
    }

    private void Move(Vector2 moveInput)
    {
        float targetSpeed = moveInput == Vector2.zero ? 0 : movementSpeed * moveInput.magnitude;

        Vector3 currentVelocity = lastMovement;
        currentVelocity.y = 0;

        float currentSpeed = currentVelocity.magnitude;

        // Check if we are near the target speed.
        if (Mathf.Abs(currentSpeed - targetSpeed) > 0.01f)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, speedChangeRate * Time.deltaTime);
        }
        else
        {
            currentSpeed = targetSpeed;
        }

        // In a sense "vectorize the quaternion" (loosing one axis: the roll).
        Vector3 targetRotation = characterTargetRotation * Vector3.forward;

        Vector3 movement = targetRotation * currentSpeed;

        characterController.SimpleMove(movement);

        if (Physics.Raycast(transform.position + Vector3.up * 0.01f,
                            Vector3.down,
                            out RaycastHit hit,
                            raycastLength,
                            raycastMask,
                            QueryTriggerInteraction.Ignore))
        {
            if (Vector3.ProjectOnPlane(movement, hit.normal).y < 0)
            {
                characterController.Move(Vector3.down * (pullDownForce * Time.deltaTime));
            }
        }

        lastMovement = movement;
    }

    #endregion

    #region Ground Check

    private void CheckGround()
    {
        if (characterController.isGrounded)
        {
            airTime = 0;
        }
        else
        {
            airTime += Time.deltaTime;
        }

        isGrounded = airTime < coyoteTime;
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

            lookInput *= deltaTimeMultiplier * sensitivity;

            cameraRotation.x += lookInput.y * cameraVerticalSpeed * (!isMouseLook && invertY ? -1 : 1);
            cameraRotation.y += lookInput.x * cameraHorizontalSpeed;

            cameraRotation.x = NormalizeAngle(cameraRotation.x);
            cameraRotation.y = NormalizeAngle(cameraRotation.y);

            cameraRotation.x = Mathf.Clamp(cameraRotation.x, verticalCameraRotationMin, verticalCameraRotationMax);
        }

        cameraTarget.rotation = Quaternion.Euler(cameraRotation.x, cameraRotation.y, 0);
    }

    private float NormalizeAngle(float angle)
    {
        // Limits the angle (-360, 360)
        angle %= 360;

        // Limits the angle [0, 360).
        if (angle < 0)
        {
            angle += 360;
        }

        // Remaps the angle from [0, 360) to [-180, 180).
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

        // Xbox controller on Windows: XInputControllerWindows
        // PS4 controller on Windows: DualShock4GamepadHID
        return lookAction.activeControl.device.name == "Mouse";
    }

    #endregion

    #region Animator

    private void UpdateAnimator()
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
            // return;
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
