using System;
using UnityEngine;
using UnityEngine.InputSystem;
using InputSystem = Input.InputSystem;


public interface IShipController
{
    public Vector2 MousePosition { get; }
    public Vector2 MoveInput { get; }

    public void SubscribeToAttackEvent(Action attackAction);
    public void UnsubscribeFromAttackEvent(Action attackAction);
}
public class ShipController : MonoBehaviour, InputSystem.IPlayerActions, IShipController
{
    
    #region Private Variables

    private Vector2 _moveInput;
    private Vector2 _mousePosition;
    private InputSystem _inputSystem;
    private Action _onAttackEvent;

    #endregion
    
    #region Public Variables
    
    public Vector2 MousePosition => _mousePosition;
    public Vector2 MoveInput => _moveInput;

    #endregion
    
    #region Unity API
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _inputSystem = new InputSystem();
        _inputSystem.Enable();
        _inputSystem.Player.SetCallbacks(this);
    }

    // Update is called once per frame
    void Update()
    {
        _mousePosition = _inputSystem.Player.Look.ReadValue<Vector2>();
        _moveInput = _inputSystem.Player.Move.ReadValue<Vector2>();
    }

    private void OnDisable()
    {
        _inputSystem.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
    }

    public void OnLook(InputAction.CallbackContext context)
    {
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed) _onAttackEvent?.Invoke();
    }
    
    #endregion

    #region Main Methods
    
    public void SubscribeToAttackEvent(Action attackAction)
    {
        _onAttackEvent += attackAction;
    }

    public void UnsubscribeFromAttackEvent(Action attackAction)
    {
        _onAttackEvent -= attackAction;
    }
    
    #endregion
}
