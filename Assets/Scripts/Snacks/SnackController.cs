using UnityEditor.Rendering.LookDev;
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

    [Header("Snack Ability")]
    //public SnackAbility currentAbility;
    [SerializeField] float _abilityCooldown = 5f;
    private float _abilityTimer;

    //[Header("Tool")]
    //public SnackTool equippedTool;

    private Rigidbody _rb;
    private Vector2 _moveInput;
    private bool _isGrounded;
    private bool _isRolling;
    private bool _jumpPressed;
    private Animator _animator;

    #endregion

    void Awake()
    {
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
    }

    void FixedUpdate()
    {
        Move();
    }

    #region Movement Methods

    private void CheckGround()
    {
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, _groundCheckDistance, _groundLayer);
    }

    private void Move()
    {
        Vector3 movement = new Vector3(_moveInput.x, 0f, _moveInput.y) * _moveSpeed;
        Vector3 newVelocity = new Vector3(movement.x, _rb.linearVelocity.y, movement.z);

        if(_isRolling)
        {
            newVelocity *= _rollSpeedMultiplier;
        }

        _rb.linearVelocity = newVelocity;

        if(_animator)
        {
            _animator.SetFloat("speed", movement.magnitude);
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

    #endregion

    #region Ability Methods
    private void ActivateAbility()
    {
        if (_currentAbility != null && _abilityTimer <= 0)
        {
            _currentAbility.Activate();
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
        if(_equippedTool != null)
        {
            _equippedTool.Use();
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
