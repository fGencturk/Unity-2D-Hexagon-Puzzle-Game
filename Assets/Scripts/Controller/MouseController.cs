using System.Collections.Generic;
using Hexagon;
using Selection;
using UnityEngine;

namespace Controller
{
    public class MouseController : MonoBehaviour
    {
        private Vector2 _firstHexPosition,
            _spriteSize;

        [SerializeField] private SelectionHandler _selectionHandler;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _firstHexPosition = Board.instance.GetElement(0, 0).gameObject.transform.position;
                _spriteSize = Board.instance.GetSpriteSizeInUnits();
                HandleRows(_firstHexPosition, position);
            }
        }

        void HandleRows(Vector2 initialPos, Vector2 mousePos)
        {
            // Finding index of the clicked hexagon
            Vector2 relativePos = mousePos - initialPos;
            relativePos.y = -relativePos.y;
            float xDifference = _spriteSize.x * Board.XMovementMultiplier,
                yDifference = _spriteSize.y / 2;
            Vector2Int indexes = new Vector2Int((int) (relativePos.x / xDifference) , (int) (relativePos.y / yDifference));

            indexes.y += indexes.x % 2 == 1 ? 1 : 0;
            indexes.y = indexes.y / 2 * 2;
            indexes.y += indexes.x % 2 == 1 ? -1 : 0;
            indexes.x = Mathf.Clamp(indexes.x, 0, Board.instance.boardSize.x - 1);
            indexes.y = Mathf.Clamp(indexes.y, 0, Board.instance.boardSize.y * 2 - 1);

            Hex hex = Board.instance.GetElement(indexes);
            HexagonVertexes hexagonVertex = hex.GetSelectionSide(mousePos);
            hexagonVertex = HandleEdgeCases(hex, hexagonVertex);
            _selectionHandler.SelectHex(hex, hexagonVertex);
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
