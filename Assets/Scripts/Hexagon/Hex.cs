using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hexagon
{
    public class Hex : MonoBehaviour
    {
        public Vector2Int index { get; private set; }
        public int hexType { get; private set; }

        public void Initialize(Vector2Int index, int hexType)
        {
            this.index = index;
            this.hexType = hexType;
            this.name = "Hex (y:" + index.y + ", x:" + index.x + ") - Type :" + hexType;
            
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            SpriteRenderer targetRenderer = GameManager.instance.hexPrefabs[hexType].GetComponent<SpriteRenderer>();
            renderer.sprite = targetRenderer.sprite;
        }

        public void ChangeIndex(Vector2Int index)
        {
            this.index = index;
            this.name = "Hex (y:" + index.y + ", x:" + index.x + ") - Type :" + hexType;

            StartCoroutine(MoveToPosition());
        }

        private IEnumerator MoveToPosition(Action onFinish = null)
        {
            float time = 0,
                duration = GameManager.instance.hexMoveAnimationDuration;
            Vector2 startPosition = transform.position;
            
            Vector2 newPosition = GameManager.instance.positionCalculator.GetPosition(index);
            
            while (time <= duration)
            {
                time += Time.deltaTime;
                transform.position = Vector3.Lerp(startPosition, newPosition, time / duration);
                yield return new WaitForEndOfFrame();
            }
            
            onFinish?.Invoke();
        }

        public bool HasNeighborHex(HexagonEdges hexagonEdges)
        {
            Vector2Int boardSize = GameManager.instance.positionCalculator.boardSize;
            // First row does not have Top-Neighbors
            if (index.y == 0 && (hexagonEdges == HexagonEdges.TopLeft || 
                                    hexagonEdges == HexagonEdges.Top ||
                                    hexagonEdges == HexagonEdges.TopRight))
            {
                return false;
            }
            // Second row does not have Top-Neighbor
            if (index.y == 1 && (hexagonEdges == HexagonEdges.Top))
            {
                return false;
            }
            // Last row does not have Bottom-Neighbors
            if (index.y >= boardSize.y * 2 - 1 && (hexagonEdges == HexagonEdges.BottomLeft || 
                                                           hexagonEdges == HexagonEdges.Bottom || 
                                                           hexagonEdges == HexagonEdges.BottomRight))
            {
                return false;
            }
            // Second to last row does not have Bottom-Neighbor
            if (index.y == boardSize.y * 2 - 2 && (hexagonEdges == HexagonEdges.Bottom))
            {
                return false;
            }
            // First column does not have Left-Neighbors
            if (index.x == 0 && (hexagonEdges == HexagonEdges.BottomLeft || 
                                    hexagonEdges == HexagonEdges.TopLeft))
            {
                return false;
            }
            // Last column does not have Right-Neighbors
            if (index.x == boardSize.x - 1 && (hexagonEdges == HexagonEdges.BottomRight || 
                                                  hexagonEdges == HexagonEdges.TopRight))
            {
                return false;
            }
            return true;
        }
        
        public HexagonVertexes GetSelectionSide(Vector2 mousePos)
        {
            // 6 sides of the hexagon
            Vector2 spriteSize = GameManager.instance.positionCalculator.hexagonSize;
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
            Vector2Int neighborPosition = index + hexagonEdges.GetVector();
            return GameManager.instance.GetElement(neighborPosition);
        }
    }
}