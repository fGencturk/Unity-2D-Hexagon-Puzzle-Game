using Hexagon;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private Text currentScoreText,
            highestScoreText;
        [SerializeField] public int scorePerHex = 5;
        public int currentScore { get; private set; }

        public readonly string highestScoreKey = "highestScore";

        private string _highScoreEmptyText;
        private int _highestScore;
    
        void Start()
        {
            currentScore = 0;
            _highScoreEmptyText = highestScoreText.text;
            _highestScore = PlayerPrefs.GetInt(highestScoreKey, -1);
            if (_highestScore == -1)
            {
                highestScoreText.text = "";
            }

            GameManager.instance.onHexagonGroupExplode += OnScore;
        }

        public void OnScore(int count)
        {
            currentScore += count * scorePerHex;
            currentScoreText.text = currentScore.ToString();
        }

        public void OnGameOver()
        {
            if (currentScore > _highestScore)
            {
                PlayerPrefs.SetFloat(highestScoreKey, currentScore);
                PlayerPrefs.Save();
                _highestScore = currentScore;
                highestScoreText.text = _highScoreEmptyText + _highestScore;
            }
        }
    }
}
