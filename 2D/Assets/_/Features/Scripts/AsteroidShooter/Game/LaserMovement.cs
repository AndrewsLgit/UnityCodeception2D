using UnityEngine;

public class LaserMovement : MonoBehaviour
{
    #region Private Variables
    
    private int _thrust = 15;
    private float _lifeTime = 3;
    
    #endregion

    #region Unity API

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    // Since our objects start disabled, each time they are "instantiated" we set them back to active
    // this would be considered the start of our object's life cycle instead of the Start() method.
    // Here we disable our laser after 3 seconds, this is not necessary in all cases, what should be taken into account is that instead of destroying our instance, we simply disable it
    private void OnEnable()
    {
        Invoke(nameof(Deactivate), _lifeTime);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(Deactivate));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Asteroid"))
        {
            Debug.Log($"Laser hit: {collision.gameObject.name}");
            Deactivate();
            //Destroy(this.gameObject);
        }
    }

    #endregion

    #region Main Methods

    private void Move()
    {
        transform.Translate(Vector3.up * (Time.deltaTime * _thrust));
    }
    
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    #endregion
}
