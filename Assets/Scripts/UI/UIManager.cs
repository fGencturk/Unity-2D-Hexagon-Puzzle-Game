using System;
using Hexagon;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject _gameOverPanel;

        private void Start()
        {
            _gameOverPanel.SetActive(false);
            GameManager.instance.onGameOver += OnGameOver;
        }

        public void OnGameOver()
        {
            _gameOverPanel.SetActive(true);
        }
        
        public void OnRestart()
        {
            _gameOverPanel.SetActive(false);
            GameManager.instance.TriggerRestart();
        }
    }
}