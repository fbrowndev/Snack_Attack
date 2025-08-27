using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class ThirdPersonController : MonoBehaviour
{
    #region Variables
    //General Movement variables
    [Header("Player Movement")]
    [SerializeField] private float _moveSpeed = 4f;
    [SerializeField] private float _sprintSpeed = 8.5f;
    [SerializeField] private float _jumpHeight = 1.2f;
    [SerializeField] private float _gravity = -15.0f;
    [Tooltip("Speed of character turning to face movement direction"), Range(0.0f, 0.3f) , SerializeField] private float _RotationSmoothTime = 0.12f;
    [Tooltip("Acceleration and Deceleration"), SerializeField] private float _speedChangeRate = 10.0f;

    //Camera related variables
    [Header("Camera Settings")]
    [SerializeField] GameObject _cinemachineCameraTarget;
    [Tooltip("How far in degrees can you move the camera up"), SerializeField] private float _topClamp = 70.0f;
    [Tooltip("How far in degrees can you move the camera down"), SerializeField] private float _bottomClamp = -30.0f;
    [Serialize] float _cameraAngleOverride = 0.0f; 
    [SerializeField] private GameObject _mainCamera;
    [SerializeField] private bool _lockCameraPosition = false;
    [SerializeField] private Vector2 _lookSensitivity = new Vector2(7.5f, 5.0f);

    //Ground Check variables
    [Header("Ground and Jump")]
    [SerializeField] private float _jumpTimeout = 0.50f;
    [SerializeField] private float _fallTimeout = 0.15f;
    [SerializeField] private float _groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private bool _isGrounded = true;
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;
      
    private float _speed;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    private Vector3 _cameraStartingPosition;
    private Quaternion _cameraStartingRotation;

    private const float _threshold = 0.01f;

    private PlayerInput _playerInput;
    private SnackInputs _snackInputs;
    private CharacterController _controller;

    #endregion

    void Awake()
    {
        if(_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }


    void Start()
    {
        _cinemachineTargetYaw = _cinemachineCameraTarget.transform.rotation.eulerAngles.y;
        _controller = GetComponent<CharacterController>();
        _snackInputs = GetComponent<SnackInputs>(); 
        _playerInput = GetComponent<PlayerInput>();

        //Saving starting camera position and rotation
        _cameraStartingPosition = _cinemachineCameraTarget.transform.position;
        _cameraStartingRotation = _cinemachineCameraTarget.transform.rotation;

        _jumpTimeoutDelta = _jumpTimeout;
        _fallTimeoutDelta = _fallTimeout;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        GroundCheck();
        JumpAndGravity();
    }

    void LateUpdate()
    {
        CameraRotation();
    }

    private bool IsCurrentDeviceMouse()
    {
        if (_playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            Debug.Log("True");
            return true; 
        }
        else
        {
            Debug.Log("False");
            return false;
        }
    }

    void CameraRotation()
    {
        if(_snackInputs.look.sqrMagnitude >= _threshold && !_lockCameraPosition)
        {
            float deltaTimeMultiplier = IsCurrentDeviceMouse() ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw += _snackInputs.look.x * deltaTimeMultiplier * _lookSensitivity.x;
            _cinemachineTargetPitch += _snackInputs.look.y * deltaTimeMultiplier * _lookSensitivity.y;
        }

        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _bottomClamp, _topClamp);

        _cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + _cameraAngleOverride, _cinemachineTargetYaw, 0.0f);
    }

    #region Movement functions
    void Move()
    {
        float targetSpeed = _snackInputs.sprint ? _sprintSpeed : _moveSpeed;

        if(_snackInputs.move == Vector2.zero)
        {
            targetSpeed = 0;
        }

        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = _snackInputs.analogMovement ? _snackInputs.move.magnitude : 1f;

        if(currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * _speedChangeRate);
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        //Add in animation ques here possibly?

        Vector3 inputDirection = new Vector3(_snackInputs.move.x, 0.0f, _snackInputs.move.y).normalized;

        if(_snackInputs.move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, _RotationSmoothTime);

            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
    }

    private void GroundCheck()
    {

        _isGrounded = Physics.Raycast(transform.position, Vector3.down, _groundCheckDistance, _groundLayer);

        Color rayColor = _isGrounded ? Color.green : Color.red;
        Debug.DrawRay(transform.position, Vector3.down * _groundCheckDistance, rayColor);
    }

    private void JumpAndGravity()
    {
        if(_isGrounded)
        {

            _fallTimeoutDelta = _fallTimeout;

            //Possibly add animation here?

            if(_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            if (_snackInputs.jump && _jumpTimeoutDelta <= 0.0f)
            {
                _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);

                //Add animation here?
            }

            if(_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            _jumpTimeoutDelta = _jumpTimeout;

            if(_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }

            _snackInputs.jump = false;
        }

        if(_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += _gravity * Time.deltaTime;
        }
    }

    #endregion

    #region Math Functions

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    #endregion
}
