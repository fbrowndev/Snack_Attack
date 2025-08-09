using UnityEditor.Rendering.LookDev;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class SnackController : MonoBehaviour
{
    #region Variables
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpForce = 6f;
    [SerializeField] private float _rollSpeedMultiplier = 1.5f;
    [SerializeField] private float _groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _rotationSpeed = 720f;

    [Header("Snack Ability")]
    public SnackAbility currentAbility; //maybe make private later
    [SerializeField] float _abilityCooldown = 5f;
    private float _abilityTimer;

    [Header("Tool")]
    public SnackTool equippedTool; //maybe make private later

    private PlayerInputActions _inputActions;
    private Rigidbody _rb;
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private bool _isGrounded;
    private bool _isRolling;
    private bool _jumpPressed;
    private Animator _animator;

    [Header("Camera Settings")]
    [SerializeField] private Transform _cameraTransform;

    #endregion

    void Awake()
    {
        _inputActions = new PlayerInputActions();
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckGround();
        HandleAbilityCooldown();

        if(_jumpPressed && _isGrounded)
        {
            Jump();
            _jumpPressed = false;
        }

        if(Keyboard.current.qKey.wasPressedThisFrame)
        {
            UseTool();
        }

        if(Keyboard.current.eKey.wasPressedThisFrame)
        {
            ActivateAbility();
        }

        PlayerRotation();
    }

    void FixedUpdate()
    {
        Move();
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
        _inputActions.Player.Move.performed += OnMove;
        _inputActions.Player.Move.canceled += OnMove;
        _inputActions.Player.Jump.performed += ctx => _jumpPressed = true;
        _inputActions.Player.Look.performed += ctx => _lookInput = ctx.ReadValue<Vector2>();
        _inputActions.Player.Look.canceled += ctx => _lookInput = Vector2.zero;
    }

    private void OnDisable()
    {
        _inputActions.Player.Move.performed -= OnMove;
        _inputActions.Player.Move.canceled -= OnMove;
        _inputActions.Player.Jump.performed -= OnJump;
        _inputActions.Player.Disable();
    }

    #region Movement Methods

    private void CheckGround()
    {
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, _groundCheckDistance, _groundLayer);

        // Debug visualization
        //Color rayColor = _isGrounded ? Color.green : Color.red;
        //Debug.DrawRay(transform.position, Vector3.down * _groundCheckDistance, rayColor);
    }

    private void Move()
    {

        Vector3 camForward = _cameraTransform.forward;
        Vector3 camRight = _cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDirection = (camRight * _moveInput.x + camForward * _moveInput.y).normalized;

        Vector3 movement = moveDirection * _moveSpeed;

        Vector3 newVelocity = new Vector3(movement.x, _rb.linearVelocity.y, movement.z);

        if(_isRolling)
        {
            newVelocity *= _rollSpeedMultiplier;
        }

        _rb.linearVelocity = newVelocity;

        if(_animator)
        {
            _animator.SetFloat("Speed" , movement.magnitude);
        }

        if(moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }

    }

    private void Jump()
    {
        _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);

        if(_animator)
        {
            _animator.SetTrigger("Jump");
        }
    }

    private void HandleMouseRotation()
    {
        Ray ray = _mainCamera.ScreenPointToRay(_lookInput);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f))
        {
            Vector3 lookDirection = hitInfo.point - transform.position;
            lookDirection.y = 0f;

            if (lookDirection.sqrMagnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            }
        }
    }

    private void HandleStickRotation()
    {
        if (_lookInput.sqrMagnitude > 0.1f)
        {
            Vector3 direction = new Vector3(_lookInput.x, 0f, _lookInput.y);
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
    }

    private void PlayerRotation()
    {
        if(_inputActions.controlSchemes.ToString() == "KeyboardMouse")
        {
            HandleMouseRotation();
        }
        else
        {
            HandleStickRotation();
        }
    }

    #endregion

    #region Ability Methods
    private void ActivateAbility()
    {
        if (currentAbility != null && _abilityTimer <= 0)
        {
            currentAbility.Activate();
            _abilityTimer = _abilityCooldown;
        }
    }

    private void HandleAbilityCooldown()
    {
        if(_abilityTimer > 0)
        {
            _abilityTimer -= Time.deltaTime;
        }
    }

    private void UseTool()
    {
        if(equippedTool != null)
        {
            equippedTool.Use();
        }
    }

    #endregion


    #region Input System Binds
    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            _jumpPressed = true;
        }
    }

    public void OnRoll(InputAction.CallbackContext context)
    {
        _isRolling = context.ReadValue<float>() > 0;
    }

    #endregion
}
