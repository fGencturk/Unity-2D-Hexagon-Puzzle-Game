using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hexagon;
using UnityEngine;

namespace Selection
{
    public class SelectionHandler : MonoBehaviour
    {
        [SerializeField] private GameObject rightSelectionPrefab, leftSelectionPrefab;
        [SerializeField] private GameManager _gameManager;
        
        private GameObject _selectionGameObject;
        private Hex _selectedHex;
        private bool _isLeft;
        private Vector2 _rotationPoint;

        public bool SelectHex(Hex hex, HexagonVertexes hexagonVertex, out Vector2 position)
        {
            if (!GameManager.instance.canTakeAction)
            {
                position = Vector2.zero;
                return false;
            }
            if (hexagonVertex == HexagonVertexes.BottomLeft)
            {
                hex = hex.GetNeighbor(HexagonEdges.BottomLeft);
                _isLeft = false;
            } else if (hexagonVertex == HexagonVertexes.BottomRight)
            {
                hex = hex.GetNeighbor(HexagonEdges.BottomRight);
                _isLeft = true;
            } else if (hexagonVertex == HexagonVertexes.TopLeft)
            {
                hex = hex.GetNeighbor(HexagonEdges.TopLeft);
                _isLeft = false;
            } else if (hexagonVertex == HexagonVertexes.TopRight)
            {
                hex = hex.GetNeighbor(HexagonEdges.TopRight);
                _isLeft = true;
            } else if (hexagonVertex == HexagonVertexes.Right)
            {
                _isLeft = false;
            } else
            {
                _isLeft = true;
            }
            
            _selectedHex = hex;

            if (_selectionGameObject)
            {
                Destroy(_selectionGameObject);
            }

            var hexTransform = hex.transform;
            Vector2 hexagonSize = GameManager.instance.positionCalculator.hexagonSize;
            position = hexTransform.position + new Vector3(0, -hexagonSize.y / 2);
            position += _isLeft ? Vector2.zero : new Vector2(hexagonSize.x, 0);
            _selectionGameObject = Instantiate(_isLeft ? leftSelectionPrefab : rightSelectionPrefab, hexTransform.position, Quaternion.identity);
            return true;
        }
        
        void HandleRotate(bool clockwise)
        {
            if (!_selectedHex || !GameManager.instance.canTakeAction)
            {
                return;
            }

            Vector2 spriteSize = GameManager.instance.positionCalculator.hexagonSize; 
            _rotationPoint = (Vector2) _selectedHex.transform.position + new Vector2(0, -spriteSize.y / 2);
            if (!_isLeft)
            {
                _rotationPoint.x += spriteSize.x;
            }
            HexagonEdges firstNeighbor = _isLeft ? HexagonEdges.BottomLeft : HexagonEdges.TopRight;

            _gameManager.Rotate(_selectedHex, firstNeighbor, clockwise);

            _selectedHex = null;
            Destroy(_selectionGameObject);
        }
        public void RotateClockwise()
        {
            HandleRotate(true);
        }

        public void RotateCounterClockwise()
        {
            HandleRotate(false);
        }

        
    }
}