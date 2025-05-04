using UnityEngine;

public class CameraWrapper : MonoBehaviour
{
    #region Private Variables

    private Camera _camera;
    private Vector3 _cameraPosition;
    private Transform _playerTransform;

    #endregion
    
    #region Unity API
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _camera = _camera ==null ? Camera.main : _camera;
        _playerTransform = FindFirstObjectByType<Ship>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        _cameraPosition = _camera.WorldToViewportPoint(_playerTransform.position);
        _playerTransform.position = CheckPosition(_cameraPosition);
    }
    
    #endregion

    #region Main Methods

    private Vector2 CheckPosition(Vector2 playerPos)
    {
        var newPos = (Vector2)_playerTransform.position;
        /*if (playerPos.x < 0.0 && playerPos.y < 0.0 || playerPos.x > 1.0 && playerPos.y > 1.0)
        {
            newPos = new Vector2(-_playerTransform.position.x, -_playerTransform.position.y);
        }*/

        /*if (playerPos.x < 0.0 || playerPos.x > 1.0)
        {
            newPos.x = -newPos.x;
        }

        if (playerPos.y < 0.0 || playerPos.y > 1.0)
        {
            newPos.y = newPos.y;
        }*/

        switch (playerPos.x)
        {
            case < 0.0f : case > 1.0f:
                newPos.x = newPos.x * -1;
                break;
            default:
                break;
        }

        switch (playerPos.y)
        {
            case < 0.0f : case > 1.0f:
                newPos.y = newPos.y * -1;
                break;
            default:
                break;
        }

        //else newPos = _playerTransform.position;

        return newPos;
    }

    #endregion
}
