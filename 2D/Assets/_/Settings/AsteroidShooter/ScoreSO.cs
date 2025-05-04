using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ScoreSO", menuName = "Scriptable Objects/ScoreSO")]
public class ScoreSO : ScriptableObject
{
    private int _score = 0;
    private int _highScore;
    [SerializeField] private int _scoreIncrement = 10;

    public int Score => _score;
    public int HighScore => _highScore;

    public void IncreaseScore()
    {
        _score += _scoreIncrement;
        _highScore = _highScore < _score ? _score : _highScore;
    }
    
    public void ResetScore() => _score = 0;
    public void ResetHighScore() => _highScore = 0;
}
