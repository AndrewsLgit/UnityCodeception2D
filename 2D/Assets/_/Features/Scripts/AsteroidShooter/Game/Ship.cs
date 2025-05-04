using System;
using UnityEngine;

public class Ship : MonoBehaviour
{

    #region Private Variables
    
    [SerializeField] private GameObject _thrusterSprite;
    [SerializeField] private Transform _weaponPoint;
    [SerializeField] private ProjectilePool _laserProjectilePool;

    
    private IShipController _shipController;
    private Camera _camera;
    private Rigidbody2D _shipRigidbody;
    
    [SerializeField] private float _maxSpeed = 10f;
    [SerializeField] private float _speed = 4f;

    #endregion

    #region Unity API
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _shipController = GetComponent<IShipController>();
        _shipRigidbody = GetComponent<Rigidbody2D>();
        
        _camera = Camera.main;
        _thrusterSprite.SetActive(false);
        
        _shipController.SubscribeToAttackEvent(LaserAttack);
    }

    // Update is called once per frame
    void Update()
    {
        LookAtPosition(GetMouseWorldPoint());
        Move();
    }

    private void OnDisable()
    {
        _shipController.UnsubscribeFromAttackEvent(LaserAttack);
        Debug.Log($"Player death | Game object active: {gameObject.activeSelf}");
    }

    #endregion
    
    #region Main Methods

    private void LookAtPosition(Vector2 targetPos)
    {
        Vector2 direction = targetPos - (Vector2)transform.position;
        var playerRotation = Quaternion.LookRotation(Vector3.forward, direction);
        transform.rotation = playerRotation;
    }

    private void Move()
    {
        var moveInput = GetMoveInput();
        if (moveInput.magnitude < _maxSpeed)
        {
            _shipRigidbody.AddForce((transform.up * (moveInput.y * _speed)));
            _shipRigidbody.AddForce((transform.right * (moveInput.x * _speed)));
            
        }
        _thrusterSprite.SetActive(moveInput.magnitude > 0);
        
        //_playerRigidBody2D.AddForce(_movementInput * (_speed * Time.deltaTime));
    }

    private void LaserAttack()
    {
        //GameObject laser = Instantiate(_laserSprite, _weaponPoint.position, _playerRotation);
        GameObject laser = _laserProjectilePool.GetFirstAvailableProjectile();
        laser.transform.position = _weaponPoint.position;
        laser.transform.rotation = transform.rotation;
        laser.SetActive(true);
        Debug.Log($"Shot {laser.name}");
    }

    public void OnDeath()
    {
        gameObject.SetActive(false);
    }
    
    #endregion
    
    #region Utils

    private Vector2 GetMousePosition2D() => _shipController.MousePosition;
    private Vector2 GetMoveInput() => _shipController.MoveInput;
    private Vector2 GetMouseWorldPoint() => _camera.ScreenToWorldPoint(GetMousePosition2D());

    #endregion
}
