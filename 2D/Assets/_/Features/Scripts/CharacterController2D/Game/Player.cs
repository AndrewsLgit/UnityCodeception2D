using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    #region Public Variables

    public enum STATE
    {
        MOVE,
        JUMP,
        FALL,
        HANG,
    }

    public STATE m_currentState;
    [FormerlySerializedAs("currentPlatform")] public Platform m_currentPlatform;

    #endregion
    
    #region Private Variables
    
    private ICharacterController2D _characterController2D;
    private Rigidbody2D _rigidbody2D;
    // Player collision checkers and collider
    [SerializeField] private Transform _leftCollisionChecker;
    [SerializeField] private Transform _rightCollisionChecker;
    [SerializeField] private Vector2 _capsuleSize = new Vector2(.5f, .5f);
    [SerializeField] private Collider2D _collider;
    [SerializeField] private float _colliderWaitTime;
    
    [FormerlySerializedAs("_groundLayer")]
    [Header("Ground Check")]
    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private Transform _bottomOverlap;
    [SerializeField] private float _groundCheckRadius = 0.2f;
    private bool _isGrounded;
    private bool _wasGroundedPreviousFrame;

    [Header("Jumping")] 
    private int _doubleJumpNb = 0;
    [SerializeField] private int _maxJumpNb = 1;
    [SerializeField] private float _jumpMoveForce = 15;
    private float _jumpBuffer;
    [SerializeField] private float _jumpBufferMax = .5f;
    private float _linearVelocityXOnJumpStart;

    [SerializeField] private bool _isApplyingJump;
    [SerializeField] private float _jumpForce = 10;
    //[SerializeField] private float _coyoteTime = 0.5f;
    [SerializeField] private float _jumpVelocityXModifier;
    [SerializeField] private float _lowJumpFactor;
    
    [Header("Movement")]
    [SerializeField] private float _xMovement;
    [SerializeField] private float _currentVelocity;
    [SerializeField] private float _currentSpeed = 2;
    [FormerlySerializedAs("_maxSpeed")] [SerializeField] private float _maxVelocity = 10;
    [SerializeField] private float _movementSmoothTime = .1f;

    #endregion
    
    #region Unity API
    
    private void Awake()
    {
        _characterController2D = GetComponent<ICharacterController2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _characterController2D.SubscribeToJumpStartEvent(OnJumpStart);
        _characterController2D.SubscribeToJumpEndEvent(OnJumpEnd);
        _characterController2D.SubscribeToJumpDownEvent(OnJumpDown);
    }

    private void FixedUpdate()
    {
        float sign = Mathf.Sign(_rigidbody2D.linearVelocity.x); // to know player movement direction
        switch (m_currentState)
        {
            case STATE.MOVE:
                _doubleJumpNb = 0;
                _rigidbody2D.linearVelocity = new Vector2(_xMovement * _currentSpeed * Time.fixedDeltaTime,
                    _rigidbody2D.linearVelocityY);
                break;
            case STATE.JUMP:
                LimitJumpHeight();
                AirborneMove();
                break;
            case STATE.FALL:
                _rigidbody2D.AddForce(new Vector2(_characterController2D.MoveInput.x * _jumpForce, 0));
                if (Mathf.Abs(_rigidbody2D.linearVelocity.x) > _linearVelocityXOnJumpStart * _jumpVelocityXModifier)
                {
                    //float sign = Mathf.Sign(_rigidbody2D.linearVelocity.x); // to know player movement direction
                    _rigidbody2D.linearVelocity = new Vector2(sign * _linearVelocityXOnJumpStart * _jumpVelocityXModifier, _rigidbody2D.linearVelocity.y);
                }
                break;
            case STATE.HANG:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (Mathf.Abs(_rigidbody2D.linearVelocity.x) > _maxVelocity)
        {
            //float sign = Mathf.Sign(_rigidbody2D.linearVelocity.x);
            _rigidbody2D.linearVelocity = new Vector2(sign * _maxVelocity, _rigidbody2D.linearVelocity.y);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Figure out what Mathf.SmoothDamp actually does
        _xMovement = Mathf.SmoothDamp(_xMovement, _characterController2D.MoveInput.x, ref _currentVelocity, _movementSmoothTime);
        if (_xMovement < 0 && !CanMoveLeft()) _xMovement = 0;
        if (_xMovement > 0 && !CanMoveRight()) _xMovement = 0;
        
        _jumpBuffer += Time.deltaTime;
        _isGrounded = Physics2D.OverlapCircle(_bottomOverlap.position, _groundCheckRadius, _groundLayerMask);
        
        // TODO: Evaluate current state
        switch (m_currentState)
        {
            case STATE.MOVE:
                if (!_isGrounded && _wasGroundedPreviousFrame) GoToFallState();
                break;
            case STATE.JUMP:
                if (_rigidbody2D.linearVelocity.y < 0) GoToFallState();
                break;
            case STATE.FALL:
                if (_isGrounded && !_wasGroundedPreviousFrame)
                {
                    if (_isApplyingJump && _jumpBuffer < _jumpBufferMax) GoToJumpState();
                    else GoToMoveState();
                }
                break;
            case STATE.HANG:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        _wasGroundedPreviousFrame = _isGrounded;
    }

    private void OnDestroy()
    {
        _characterController2D.UnsubscribeFromJumpStartEvent(OnJumpStart);
        _characterController2D.UnsubscribeFromJumpEndEvent(OnJumpEnd);
        _characterController2D.UnsubscribeFromJumpDownEvent(OnJumpDown);
    }

    #endregion
    
    #region Main Methods

    private void Jump()
    {
        switch (m_currentState)
        {
            case STATE.HANG:
                break;
            default:
                m_currentState = STATE.JUMP;
                ResetVerticalVelocity();
                _isApplyingJump = true;
                // _linearVelocity on jumpstart = Mathf.Max(1, Mathf.Abs(_rigidbody.linearVelocity.x))
                _rigidbody2D.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
                break;
        }
    }
    
    private void OnJumpStart()
    {
        _jumpBuffer = 0;
        _isApplyingJump = true;
        if (_isGrounded || _doubleJumpNb <= _maxJumpNb) Jump();
    }

    private void OnJumpEnd()
    {
        _isApplyingJump = false;
    }
    private void OnJumpDown()
    {
        if (m_currentPlatform != null)
        {
            _collider.enabled = false;
            StartCoroutine(ReactivateCollider());
            m_currentPlatform = null;
        }
    }
    
    #endregion
    
    #region Utils
    
    private IEnumerator ReactivateCollider()
    {
        yield return new WaitForSeconds(_colliderWaitTime);
        _collider.enabled = true;
    }

    private bool CanMoveLeft()
    {
        return !Physics2D.OverlapCapsule(_leftCollisionChecker.position, _capsuleSize, CapsuleDirection2D.Vertical, 0,
            _groundLayerMask);
    }
    private bool CanMoveRight()
    {
        return !Physics2D.OverlapCapsule(_rightCollisionChecker.position, _capsuleSize, CapsuleDirection2D.Vertical, 0,
            _groundLayerMask);
    }

    private void GoToFallState()
    {
        m_currentState = STATE.FALL;
    }

    private void GoToJumpState()
    {
        m_currentState = STATE.JUMP;
    }

    private void GoToMoveState()
    {
        m_currentState = STATE.MOVE;
    }

    private void ResetVerticalVelocity()
    {
        var currentVelocity = _rigidbody2D.linearVelocity;
        currentVelocity.y = 0;
        _rigidbody2D.linearVelocity = currentVelocity;
    }
    
    private void LimitJumpHeight()
    {
        if (_rigidbody2D.linearVelocity.y > 0 && _isApplyingJump == false)
        {
            var velocity = _rigidbody2D.linearVelocity;
            velocity += Vector2.up * (Physics2D.gravity.y * _lowJumpFactor * Time.deltaTime);
            _rigidbody2D.linearVelocity = velocity;
        }
    }
    
    private void AirborneMove()
    {
        _rigidbody2D.AddForce(new Vector2(_xMovement*_jumpMoveForce,0));
        if (Mathf.Abs(_rigidbody2D.linearVelocity.x) > _linearVelocityXOnJumpStart*_jumpVelocityXModifier)
        {
            float sign = Mathf.Sign(_rigidbody2D.linearVelocity.x);
            _rigidbody2D.linearVelocity = new Vector2(sign*_linearVelocityXOnJumpStart*_jumpVelocityXModifier,_rigidbody2D.linearVelocity.y);
        }
    }
    
    #endregion
}
