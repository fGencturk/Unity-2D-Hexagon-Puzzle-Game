using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class Board : MonoBehaviour
    {
        public static readonly float XMovementMultiplier = .75f; // 0.75 is constant for (distance between hexagons in the grid) / (hexagon width)
        
        // Although board keeps elements in two dimensional array, it exposes them as a array with doubled-coordinates
        // private Hex[,] _board and GetElement function achieves this behavior.
        // See Representation : https://pbs.twimg.com/media/Dd7Gk-FVQAEYryG.jpg "double-height"
        [SerializeField] public Vector2Int boardSize = new Vector2Int(8,9);
        [SerializeField] public List<GameObject> sprites;
        
        public static Board instance { get; private set; }
        
        private Hex[,] _board;
        private Vector2 _spriteSize;
        public void Awake()
        {
            instance = this;
            _board = new Hex[boardSize.y, boardSize.x];
            
            // Assuming all sprites have the same width and height
            SpriteRenderer spriteRenderer = sprites[0].GetComponent<SpriteRenderer>();
            _spriteSize = spriteRenderer.bounds.size;
        }

        public Hex GetElement(int row, int column)
        {
            row = row / 2;
            return _board[row, column];
        }
        public Hex GetElement(Vector2Int position)
        {
            return GetElement(position.y, position.x);
        }

        public void SetElement(int row, int column, Hex hex)
        {
            row = row / 2;
            _board[row, column] = hex;
        }

        public bool CheckMatchNeighborClockwise(Hex hex, HexDirection hexDirection)
        {
            // If there is no tile in directions, return false
            if (hex.HasNeighborHex(hexDirection) ||
                hex.HasNeighborHex(Hex.nextDirectionClockwise[hexDirection]))
            {
                return false;
            }
            // Otherwise, check other hex tiles for match
            Vector2Int hex1Position = hex.position + Hex.directionToVector[hexDirection],
                hex2Position = hex.position + Hex.directionToVector[Hex.nextDirectionClockwise[hexDirection]];
            Hex hex1 = GetElement(hex1Position),
                hex2 = GetElement(hex2Position);

            return hex.hexType == hex1.hexType && hex.hexType == hex2.hexType;
        }

        public Vector2 GetBoardSizeInUnits()
        {
            Vector2 spriteSize = GetSpriteSizeInUnits();
            return new Vector2(spriteSize.x * XMovementMultiplier * (boardSize.x - 1) + spriteSize.x, spriteSize.y * (boardSize.y + 0.5f));
        }

        public Vector2 GetSpriteSizeInUnits()
        {
            return _spriteSize;
        }
    }
}