using System.Collections.Generic;
using Hexagon;
using Selection;
using UnityEngine;
using Utility;

namespace Controller
{
    public class MouseController : MonoBehaviour
    {
        [SerializeField] private SelectionHandler _selectionHandler;
        
        private HexPositionCalculator _hexPositionCalculator;
        private Vector2 _firstHexPosition, _lastHexPosition;
        private Camera _camera;

        void Start()
        {
            _camera = Camera.main;
            _hexPositionCalculator = GameManager.instance.positionCalculator;
            _firstHexPosition = _hexPositionCalculator.firstPosition;
            _lastHexPosition = _hexPositionCalculator.lastPosition;
        }
        
        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePositionWorldPoint = _camera.ScreenToWorldPoint(Input.mousePosition);
                if (mousePositionWorldPoint.x < _firstHexPosition.x || mousePositionWorldPoint.y > _firstHexPosition.y ||
                    mousePositionWorldPoint.x > _lastHexPosition.x || mousePositionWorldPoint.y < _lastHexPosition.y)
                {
                    return;
                }
                
                Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                RaycastHit2D hitInfo = Physics2D.Raycast(_camera.ScreenToWorldPoint(mousePosition), Vector2.zero);
                
                if(hitInfo)
                {
                    Hex hex = hitInfo.transform.gameObject.GetComponent<Hex>();
                    
                    HexagonVertexes hexagonVertex = hex.GetSelectionSide(mousePositionWorldPoint);
                    hexagonVertex = HandleEdgeCases(hex, hexagonVertex);
                    _selectionHandler.SelectHex(hex, hexagonVertex);
                }
            }
        }

        HexagonVertexes HandleEdgeCases(Hex hex, HexagonVertexes hexagonVertex)
        {
            HexagonVertexes outHexagonVertex = hexagonVertex;
            foreach(var item in new SelectionAreaIterator(outHexagonVertex))
            {
                outHexagonVertex = item;
                if (HasRequiredNeighbors(hex, outHexagonVertex))
                {
                    break;
                }
            }
            return outHexagonVertex;
        }
        
        bool HasRequiredNeighbors(Hex hex, HexagonVertexes hexagonVertex)
        {
            List<HexagonEdges> requiredNeighbors = hexagonVertex.GetNeighborEdges();
            foreach (var hexSide in requiredNeighbors)
            {
                if (!hex.HasNeighborHex(hexSide))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
