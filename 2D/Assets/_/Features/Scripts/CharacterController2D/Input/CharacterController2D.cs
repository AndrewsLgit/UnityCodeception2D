using System;
using CC2DInput;
using UnityEngine;
using UnityEngine.InputSystem;

public interface ICharacterController2D
{
    void SubscribeToJumpStartEvent(Action jumpAction);
    void SubscribeToJumpEndEvent(Action jumpAction);
    void UnsubscribeFromJumpStartEvent(Action jumpAction);
    void UnsubscribeFromJumpEndEvent(Action jumpAction);
        
    void SubscribeToJumpDownEvent(Action jumpAction);
    void UnsubscribeFromJumpDownEvent(Action jumpAction);
    Vector2 MoveInput { get; }
}

public class CharacterController2D : MonoBehaviour,  ICharacterController2D, CC2DInputSystem.IPlayerActions
{
    #region Public Variables

    public Vector2 MoveInput => _moveInput;

    #endregion
        
    #region Private Variables

    private Vector2 _moveInput;
    private Action _onJumpStart;
    private Action _onJumpEnd;
    private Action _onJumpDown;
    private CC2DInputSystem _gameInputSystem;

    #endregion
    
    #region Unity API

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gameInputSystem = new CC2DInputSystem();
        _gameInputSystem.Enable();
        _gameInputSystem.Player.SetCallbacks(this);
    }

    private void OnDisable()
    {
        _gameInputSystem.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        _moveInput = _gameInputSystem.Player.Move.ReadValue<Vector2>();
    }
    
    #endregion

    #region Main Methods
    
    public void OnMove(InputAction.CallbackContext context)
    {
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started) _onJumpStart?.Invoke();
        if (context.canceled) _onJumpEnd?.Invoke();
    }

    public void OnJumpDown(InputAction.CallbackContext context)
    {
        if (context.started) _onJumpDown?.Invoke();
    }
    
    #endregion

    #region Utils
    
    public void SubscribeToJumpStartEvent(Action jumpAction)
    {
        _onJumpStart  += jumpAction;
    }

    public void SubscribeToJumpEndEvent(Action jumpAction)
    {
        _onJumpEnd += jumpAction;
    }

    public void UnsubscribeFromJumpStartEvent(Action jumpAction)
    {
        _onJumpStart  -= jumpAction;
    }

    public void UnsubscribeFromJumpEndEvent(Action jumpAction)
    {
        _onJumpEnd  -= jumpAction;
    }

    public void SubscribeToJumpDownEvent(Action jumpAction)
    {
        _onJumpDown  += jumpAction;
    }

    public void UnsubscribeFromJumpDownEvent(Action jumpAction)
    {
        _onJumpDown  -= jumpAction;
    }
    
    #endregion
}
