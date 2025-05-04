using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Private Variables

    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _highScoreText;
    [SerializeField] private ScoreSO _scoreSO;

    #endregion

    #region Unity API
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //_scoreSO = Instantiate(_scoreSO);
        //_scoreSO.m_score = 0;
        //_scoreSO.m_highScore = 0;
        
        _scoreSO.ResetScore();
    }

    // Update is called once per frame
    void Update()
    {
        DisplayScore();
    }
    
    #endregion

    #region Main Methods

    public void OnScore()
    {
        _scoreSO.IncreaseScore();
    }

    private void DisplayScore()
    {
        _scoreText.text = $"Score: {_scoreSO.Score}";
        _highScoreText.text = $"High Score: {_scoreSO.HighScore}";
    }

    #endregion
}
