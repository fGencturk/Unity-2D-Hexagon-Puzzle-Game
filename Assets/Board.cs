using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class Board : MonoBehaviour
    {
        // Although board keeps elements in two dimensional array, it exposes them as a array with doubled-coordinates
        // private Hex[,] _board and GetElement function achieves this behavior.
        // See Representation : https://pbs.twimg.com/media/Dd7Gk-FVQAEYryG.jpg "double-height"

        private static readonly float XMovementMultiplier = .75f; // 0.75 is constant for (distance between hexagons in the grid) / (hexagon width)
        
        [SerializeField] private Vector2Int _boardSize = new Vector2Int(8,9);
        [SerializeField] private List<GameObject> sprites;
        
        private Hex[,] _board;
        public void Start()
        {
            _board = new Hex[_boardSize.y, _boardSize.x];
            FillBoard();
        }

        public void FillBoard()
        {
            // Assuming all sprites have the same width and height
            SpriteRenderer spriteRenderer = sprites[0].GetComponent<SpriteRenderer>();
            var bounds = spriteRenderer.bounds;
            
            float xMaxSize = bounds.size.x * XMovementMultiplier, 
                yMaxSize = bounds.size.y;
            Vector2 boardSize = GetBoardSizeInUnits();
            float startX = -boardSize.x / 2,
                startY = boardSize.y / 2;

            for (int r = 0; r < _boardSize.y * 2; r++)
            {
                int startIndex = (r % 2) == 0 ? 0 : 1;
                for (int c = startIndex; c < _boardSize.x; c += 2)
                {
                    int createdType = GetRandomIndex();
                    Vector2 position = new Vector2(startX + xMaxSize * c, startY + -yMaxSize / 2 * r);
                    GameObject hexGameObject = Instantiate(sprites[createdType], position, Quaternion.identity, transform) as GameObject;
                    Hex hex = hexGameObject.AddComponent<Hex>();
                    hex.Initialize(new Vector2Int(r, c), createdType );
                }
            }
        }
        
        public Hex GetElement(int row, int column)
        {
            row = row / 2;
            return _board[row, column];
        }
        
        int GetRandomIndex(int exclude = -1)
        {
            int randNum = Random.Range(0, sprites.Count);
            if (randNum == exclude)
            {
                return GetRandomIndex(exclude);
            }
            return randNum;
        }

        public Vector2 GetBoardSizeInUnits()
        {
            SpriteRenderer spriteRenderer = sprites[0].GetComponent<SpriteRenderer>();
            var bounds = spriteRenderer.bounds;
            return new Vector2(bounds.size.x * XMovementMultiplier * (_boardSize.x - 1) + bounds.size.x, bounds.size.y * _boardSize.y);
        }
    }
}