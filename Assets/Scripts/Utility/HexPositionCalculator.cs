using UnityEngine;

namespace Utility
{
    public class HexPositionCalculator
    {
        public Vector2Int boardSize { get; private set; }
        public Vector2 hexagonSize { get; private set; }
        public Vector2 firstPosition { get; private set; }
        public Vector2 lastPosition { get; private set; }

        // It is fixed 0.75 value for uniform hexagons
        // It is the distance between LeftVertex and BottomRightVertex
        private readonly float _xDistanceMultiplier = 0.75f;

        public HexPositionCalculator(Vector2Int boardSize, Vector2 hexagonSize)
        {
            this.boardSize = boardSize;
            this.hexagonSize = hexagonSize;
            Vector2 boardSizeInUnits = GetBoardSizeInUnits();
            firstPosition = new Vector2(-boardSizeInUnits.x / 2, boardSizeInUnits.y / 2);
            lastPosition = firstPosition + new Vector2(boardSizeInUnits.x, -boardSizeInUnits.y);
        }

        public float GetXDistanceBetweenNeighborColumns()
        {
            return hexagonSize.x * _xDistanceMultiplier;
        }

        public Vector2 GetPosition(Vector2Int indexes)
        {
            return new Vector2(firstPosition.x + GetXDistanceBetweenNeighborColumns() * indexes.x, 
                firstPosition.y - hexagonSize.y * indexes.y / 2);
        } 
        
        public Vector2 GetBoardSizeInUnits()
        {
            return new Vector2(GetXDistanceBetweenNeighborColumns() * (boardSize.x - 1) + hexagonSize.x, hexagonSize.y * (boardSize.y + 0.5f));
        }
    }
}