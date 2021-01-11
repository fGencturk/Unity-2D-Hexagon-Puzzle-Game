using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class Hex : MonoBehaviour
    {
        public static Dictionary<HexDirection, Vector2Int> directionToVector = new Dictionary<HexDirection, Vector2Int>()
        {
            {HexDirection.Top, new Vector2Int(0, -2)},
            {HexDirection.TopRight, new Vector2Int(1, -1)},
            {HexDirection.TopLeft, new Vector2Int(-1, -1)},
            {HexDirection.Bottom, new Vector2Int(0, 2)},
            {HexDirection.BottomRight, new Vector2Int(1, 1)},
            {HexDirection.BottomLeft, new Vector2Int(-1, 1)}
        };
        public static Dictionary<HexDirection, HexDirection> nextDirectionClockwise = new Dictionary<HexDirection, HexDirection>()
        {
            {HexDirection.Top, HexDirection.TopRight},
            {HexDirection.TopRight, HexDirection.BottomRight},
            {HexDirection.TopLeft, HexDirection.Top},
            {HexDirection.Bottom, HexDirection.BottomLeft},
            {HexDirection.BottomRight, HexDirection.Bottom},
            {HexDirection.BottomLeft, HexDirection.TopLeft}
        };

        public Vector2Int position;
        public int hexType { get; private set; }

        public void Initialize(Vector2Int position, int hexType)
        {
            this.position = position;
            this.hexType = hexType;
            
            // TODO refactor next behavior
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            SpriteRenderer targetRenderer = Board.instance.sprites[hexType].GetComponent<SpriteRenderer>();
            renderer.sprite = targetRenderer.sprite;
        }

        public bool HasNeighborHex(HexDirection hexDirection)
        {
            // First row does not have Top-Neighbors
            if (position.y == 0 && (hexDirection == HexDirection.TopLeft || 
                                    hexDirection == HexDirection.Top ||
                                    hexDirection == HexDirection.TopRight))
            {
                return false;
            }
            // Second row does not have Top-Neighbor
            if (position.y == 1 && (hexDirection == HexDirection.Top))
            {
                return false;
            }
            // Last row does not have Bottom-Neighbors
            if (position.y == Board.instance.boardSize.y * 2 -1 && (hexDirection == HexDirection.BottomLeft || 
                                                           hexDirection == HexDirection.Bottom || 
                                                           hexDirection == HexDirection.BottomRight))
            {
                return false;
            }
            // Second to last row does not have Bottom-Neighbor
            if (position.y == Board.instance.boardSize.y * 2 -1 && (hexDirection == HexDirection.Bottom))
            {
                return false;
            }
            // First column does not have Left-Neighbors
            if (position.x == 0 && (hexDirection == HexDirection.BottomLeft || 
                                    hexDirection == HexDirection.TopLeft))
            {
                return false;
            }
            // Last column does not have Right-Neighbors
            if (position.x == Board.instance.boardSize.x - 1 && (hexDirection == HexDirection.BottomRight || 
                                                                 hexDirection == HexDirection.TopRight))
            {
                return false;
            }
            return true;
        }

        public Hex GetNeighbor(HexDirection hexDirection)
        {
            Vector2Int neighborPosition = position + directionToVector[hexDirection];
            return Board.instance.GetElement(neighborPosition);
        }
    }
}