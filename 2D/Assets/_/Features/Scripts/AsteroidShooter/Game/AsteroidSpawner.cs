using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class AsteroidSpawner : MonoBehaviour
{
    #region Private Variables

    [FormerlySerializedAs("_spawnPoint")] [SerializeField] private Transform _spawnArea;
    [SerializeField] private int _maxAsteroids;
    [SerializeField] private bool isSpawnerCoroutine = false;
    private ProjectilePool _asteroidPool;
    private float _spawnInterval = 1.5f;
    private float _spawnTimer = 0f;
    private GameManager _gameManager;
    private Ship _player;

    #endregion
    
    #region Unity API
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _asteroidPool = gameObject.GetComponent<ProjectilePool>();
        _gameManager = FindFirstObjectByType<GameManager>();
        _player = FindFirstObjectByType<Ship>();

    }

    // Update is called once per frame
    void Update()
    {
        if (isSpawnerCoroutine) StartCoroutine(SpawnAsteroidCoroutine());
        else SpawnAsteroid();
    }
    #endregion
    
    #region Main Methods

    private void SpawnAsteroid()
    {
        _spawnTimer += Time.deltaTime;
        int activeAsteroids = _asteroidPool.ActiveProjectileCount;
        if (_spawnTimer >= _spawnInterval && activeAsteroids < _maxAsteroids)
        {
            GameObject asteroid = _asteroidPool.GetFirstAvailableProjectile();
            
            var asteroidBehavior = asteroid.GetComponent<AsteroidBehavior>();
            asteroidBehavior.m_onAsteroidDestroyed.AddListener(_gameManager.OnScore);
            //asteroidBehavior.m_onPlayerHit.AddListener(_player.OnDeath);
            
            var randomPos = new Vector2(Random.Range(-_spawnArea.localScale.x, _spawnArea.localScale.x), Random.Range(-_spawnArea.localScale.y, _spawnArea.localScale.y));
            asteroid.transform.position = randomPos; //+ (Vector2)_spawnPoint.localScale/2;
            asteroid.SetActive(true);
            //Debug.Log($"Spawning asteroid: {asteroid.name}");
            _spawnTimer = 0f;
        }
    }

    private IEnumerator SpawnAsteroidCoroutine()
    {
        yield return new WaitForSeconds(_spawnInterval);

        if (_asteroidPool.ActiveProjectileCount >= _maxAsteroids) yield break;
        
        GameObject asteroid = _asteroidPool.GetFirstAvailableProjectile();
        
        var asteroidBehavior = asteroid.GetComponent<AsteroidBehavior>();
        asteroidBehavior.m_onAsteroidDestroyed.AddListener(_gameManager.OnScore);
        asteroidBehavior.m_onPlayerHit.AddListener(_player.OnDeath);
        
        var randomPos = new Vector2(Random.Range(-_spawnArea.localScale.x, _spawnArea.localScale.x),
            Random.Range(-_spawnArea.localScale.y, _spawnArea.localScale.y));
        asteroid.transform.position = randomPos; //+ (Vector2)_spawnPoint.localScale/2;
        asteroid.SetActive(true);
    }
    
    #endregion

}
