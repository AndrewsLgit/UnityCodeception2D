using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    #region Private Variables
    
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private int _projectilePoolSize;

    // List containing all instances of our prefab.
    private List<GameObject> _projectileList = new List<GameObject>();

    #endregion
    
    #region Unity API
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateInstances();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion
    
    #region Main Methods

    private void CreateInstances()
    {
        // Instantiate all the projectiles in our pool
        // set active = false, we will activate our projectiles on use
        for (int i = 0; i < _projectilePoolSize; i++)
        {
            var instance = Instantiate(_projectilePrefab, transform);
            instance.SetActive(false);
            _projectileList.Add(instance);
        }
    }
    
    // Return one inactive instance for use (for example when shooting, we set our projectile to active
    public GameObject GetFirstAvailableProjectile()
    {
        foreach (var instance in _projectileList)
        {
            if (instance.activeSelf == false)
            {
                return instance;
            }
        }
        // if there are no inactive instances (all instances are in use) then we create a new instance and add it to our list
        var newInstance = Instantiate(_projectilePrefab, transform);
        newInstance.SetActive(false);
        _projectileList.Add(newInstance);
        return newInstance;
    }
    
    // Useful to activate a max number of instances, for example, my AsteroidSpawner spawns(activates) asteroids as long as there's less than 5 asteroids active.
    public int ActiveProjectileCount => _projectileList.Count(x => x.activeSelf);
    
    #endregion
}
