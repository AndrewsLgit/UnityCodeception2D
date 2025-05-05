using UnityEngine;

public class Platform : MonoBehaviour
{
    #region Private Variables

    [SerializeField] private Collider2D _collider;

    #endregion
    
    #region Unity API
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.m_currentPlatform = this;
        }
    }
    
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.m_currentPlatform = null;
        }
        
    }
    
    #endregion
}
