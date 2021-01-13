using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Hexagon.HexCreator
{
    public class DefaultRandomHexCreator : RandomHexCreator
    {
        private float _screenHeight;
        [SerializeField] private int createBombForEachScore = 1000;

        private int _lastBombCreatedAt = 0;
        private void Awake()
        {
            _screenHeight = 2 * Camera.main.orthographicSize;
        }

        public override Hex CreateHexGameObject(Vector2Int indexes)
        {
            List<GameObject> hexPrefabs = GameManager.instance.hexPrefabs;
            int type = Random.Range(0, hexPrefabs.Count);
            Vector2 position = GameManager.instance.positionCalculator.GetPosition(indexes) + new Vector2(0, _screenHeight);
            GameObject hexGameObject = Instantiate(hexPrefabs[type], position, Quaternion.identity, transform) as GameObject;
            Hex hex = hexGameObject.AddComponent<Hex>();
            if (ScoreManager.instance.currentScore >= _lastBombCreatedAt + createBombForEachScore)
            {
                _lastBombCreatedAt += createBombForEachScore;
                hexGameObject.AddComponent<Bomb>();
            }
            hex.Initialize(indexes, type);
            return hex;
        }
    }
}