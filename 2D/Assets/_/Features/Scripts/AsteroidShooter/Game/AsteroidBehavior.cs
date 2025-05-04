using UnityEngine;
using UnityEngine.Events;

public class AsteroidBehavior : MonoBehaviour
{
    #region Public Variables

    public UnityEvent m_onPlayerHit;
    public UnityEvent m_onAsteroidDestroyed;
    #endregion
    
    #region Private Variables
    
    private Transform _playerTransform;
    private Rigidbody2D _rigidbody;
    private int _thrust = 10;
    
    #endregion

    #region Unity API

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        LookAtPosition(_playerTransform.transform.position);
        Move();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Laser"))
        {
            Debug.Log($"Hit by laser: {collision.gameObject.name}");
            //Destroy(this.gameObject);
            Deactivate();
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"Collision with player: {collision.gameObject.name}");
            m_onPlayerHit.Invoke();
        }
    }

    private void OnEnable()
    {
        //LookAtPosition(_playerInput.GetPlayerPosition());
    }

    #endregion

    #region Main Methods

    private void Move()
    {
        _rigidbody.AddForce(gameObject.transform.up * (Time.deltaTime * _thrust));
    }
    
    private void LookAtPosition(Vector2 targetPos)
    {
        Vector2 direction = (targetPos) - (Vector2)gameObject.transform.position;
        gameObject.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
    }

    private void Deactivate()
    {
        m_onAsteroidDestroyed.Invoke();
        m_onAsteroidDestroyed.RemoveAllListeners();
        m_onPlayerHit.RemoveAllListeners();
        gameObject.SetActive(false);
    }

    #endregion
}
