using System;
using Hexagon;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager instance;
        
        [SerializeField] private Text currentScoreText,
            highestScoreText;
        [SerializeField] public int scorePerHex = 5;
        public int currentScore { get; private set; }

        public readonly string highestScoreKey = "highestScore";

        private string _highScoreEmptyText;
        private int _highestScore;

        private void Awake()
        {
            if (instance)
            {
                Debug.LogError("There is already a ScoreManager instance. Destroying self.");
                Destroy(gameObject);
                return;
            }
            instance = this;
        }

        void Start()
        {
            SetCurrentScore(0);
            _highScoreEmptyText = highestScoreText.text;
            _highestScore = PlayerPrefs.GetInt(highestScoreKey, -1);
            if (_highestScore == -1)
            {
                highestScoreText.text = "";
            }

            GameManager.instance.onHexagonGroupExplode += OnScore;
            GameManager.instance.onGameOver += OnGameOver;
            GameManager.instance.onRestart += OnRestart;
        }

        public void OnScore(int count)
        {
            SetCurrentScore(currentScore + count * scorePerHex);
        }

        void OnRestart()
        {
            SetCurrentScore(0);
        }

        void SetCurrentScore(int score)
        {
            currentScore = score;
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
