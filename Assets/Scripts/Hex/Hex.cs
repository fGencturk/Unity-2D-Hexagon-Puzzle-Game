using System;
using System.Collections.Generic;
using Selection;
using UnityEngine;

namespace DefaultNamespace
{
    public class Hex : MonoBehaviour
    {
        public static Dictionary<HexSides, Vector2Int> directionToVector = new Dictionary<HexSides, Vector2Int>()
        {
            {HexSides.Top, new Vector2Int(0, -2)},
            {HexSides.TopRight, new Vector2Int(1, -1)},
            {HexSides.TopLeft, new Vector2Int(-1, -1)},
            {HexSides.Bottom, new Vector2Int(0, 2)},
            {HexSides.BottomRight, new Vector2Int(1, 1)},
            {HexSides.BottomLeft, new Vector2Int(-1, 1)}
        };
        public static Dictionary<HexSides, HexSides> nextDirectionClockwise = new Dictionary<HexSides, HexSides>()
        {
            {HexSides.Top, HexSides.TopRight},
            {HexSides.TopRight, HexSides.BottomRight},
            {HexSides.TopLeft, HexSides.Top},
            {HexSides.Bottom, HexSides.BottomLeft},
            {HexSides.BottomRight, HexSides.Bottom},
            {HexSides.BottomLeft, HexSides.TopLeft}
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

        public bool HasNeighborHex(HexSides hexSides)
        {
            // First row does not have Top-Neighbors
            if (position.y == 0 && (hexSides == HexSides.TopLeft || 
                                    hexSides == HexSides.Top ||
                                    hexSides == HexSides.TopRight))
            {
                return false;
            }
            // Second row does not have Top-Neighbor
            if (position.y == 1 && (hexSides == HexSides.Top))
            {
                return false;
            }
            // Last row does not have Bottom-Neighbors
            if (position.y >= Board.instance.boardSize.y * 2 - 1 && (hexSides == HexSides.BottomLeft || 
                                                           hexSides == HexSides.Bottom || 
                                                           hexSides == HexSides.BottomRight))
            {
                return false;
            }
            // Second to last row does not have Bottom-Neighbor
            if (position.y == Board.instance.boardSize.y * 2 - 2 && (hexSides == HexSides.Bottom))
            {
                return false;
            }
            // First column does not have Left-Neighbors
            if (position.x == 0 && (hexSides == HexSides.BottomLeft || 
                                    hexSides == HexSides.TopLeft))
            {
                return false;
            }
            // Last column does not have Right-Neighbors
            if (position.x == Board.instance.boardSize.x - 1 && (hexSides == HexSides.BottomRight || 
                                                                 hexSides == HexSides.TopRight))
            {
                return false;
            }
            return true;
        }
        
        public SelectionAreas GetSelectionSide(Vector2 mousePos)
        {
            // 6 sides of the hexagon
            Vector2 spriteSize = Board.instance.GetSpriteSizeInUnits();
            Vector2 centerPos = (Vector2) transform.position + new Vector2(spriteSize.x / 2, -spriteSize.y / 2 ),
                collisionBox = new Vector2(spriteSize.x / 2, spriteSize.y / 6);
            

            if (mousePos.y >= centerPos.y + collisionBox.y)
            {
                if (mousePos.x >= centerPos.x)
                {
                    return SelectionAreas.TopRight;
                }
                else
                {
                    return SelectionAreas.TopLeft;
                }
            } else if (mousePos.y >= centerPos.y - collisionBox.y)
            {
                if (mousePos.x >= centerPos.x)
                {
                    return SelectionAreas.Right;
                }
                else
                {
                    return SelectionAreas.Left;
                }
            }
            else
            {
                if (mousePos.x >= centerPos.x)
                {
                    return SelectionAreas.BottomRight;
                }
                else
                {
                    return SelectionAreas.BottomLeft;
                }
            }
        }

        public Hex GetNeighbor(HexSides hexSides)
        {
            Vector2Int neighborPosition = position + directionToVector[hexSides];
            return Board.instance.GetElement(neighborPosition);
        }
    }
}