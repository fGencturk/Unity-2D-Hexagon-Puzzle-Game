using System.Collections.Generic;
using UnityEngine;

namespace Hexagon
{
    public class Hex : MonoBehaviour
    {
        public static Dictionary<HexagonEdges, Vector2Int> directionToVector = new Dictionary<HexagonEdges, Vector2Int>()
        {
            {HexagonEdges.Top, new Vector2Int(0, -2)},
            {HexagonEdges.TopRight, new Vector2Int(1, -1)},
            {HexagonEdges.TopLeft, new Vector2Int(-1, -1)},
            {HexagonEdges.Bottom, new Vector2Int(0, 2)},
            {HexagonEdges.BottomRight, new Vector2Int(1, 1)},
            {HexagonEdges.BottomLeft, new Vector2Int(-1, 1)}
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

        public bool HasNeighborHex(HexagonEdges hexagonEdges)
        {
            // First row does not have Top-Neighbors
            if (position.y == 0 && (hexagonEdges == HexagonEdges.TopLeft || 
                                    hexagonEdges == HexagonEdges.Top ||
                                    hexagonEdges == HexagonEdges.TopRight))
            {
                return false;
            }
            // Second row does not have Top-Neighbor
            if (position.y == 1 && (hexagonEdges == HexagonEdges.Top))
            {
                return false;
            }
            // Last row does not have Bottom-Neighbors
            if (position.y >= Board.instance.boardSize.y * 2 - 1 && (hexagonEdges == HexagonEdges.BottomLeft || 
                                                           hexagonEdges == HexagonEdges.Bottom || 
                                                           hexagonEdges == HexagonEdges.BottomRight))
            {
                return false;
            }
            // Second to last row does not have Bottom-Neighbor
            if (position.y == Board.instance.boardSize.y * 2 - 2 && (hexagonEdges == HexagonEdges.Bottom))
            {
                return false;
            }
            // First column does not have Left-Neighbors
            if (position.x == 0 && (hexagonEdges == HexagonEdges.BottomLeft || 
                                    hexagonEdges == HexagonEdges.TopLeft))
            {
                return false;
            }
            // Last column does not have Right-Neighbors
            if (position.x == Board.instance.boardSize.x - 1 && (hexagonEdges == HexagonEdges.BottomRight || 
                                                                 hexagonEdges == HexagonEdges.TopRight))
            {
                return false;
            }
            return true;
        }
        
        public HexagonVertexes GetSelectionSide(Vector2 mousePos)
        {
            // 6 sides of the hexagon
            Vector2 spriteSize = Board.instance.GetSpriteSizeInUnits();
            Vector2 centerPos = (Vector2) transform.position + new Vector2(spriteSize.x / 2, -spriteSize.y / 2 ),
                collisionBox = new Vector2(spriteSize.x / 2, spriteSize.y / 6);
            

            if (mousePos.y >= centerPos.y + collisionBox.y)
            {
                if (mousePos.x >= centerPos.x)
                {
                    return HexagonVertexes.TopRight;
                }
                else
                {
                    return HexagonVertexes.TopLeft;
                }
            } else if (mousePos.y >= centerPos.y - collisionBox.y)
            {
                if (mousePos.x >= centerPos.x)
                {
                    return HexagonVertexes.Right;
                }
                else
                {
                    return HexagonVertexes.Left;
                }
            }
            else
            {
                if (mousePos.x >= centerPos.x)
                {
                    return HexagonVertexes.BottomRight;
                }
                else
                {
                    return HexagonVertexes.BottomLeft;
                }
            }
        }

        public Hex GetNeighbor(HexagonEdges hexagonEdges)
        {
            Vector2Int neighborPosition = position + directionToVector[hexagonEdges];
            return Board.instance.GetElement(neighborPosition);
        }
    }
}