using Hexagon;
using UnityEngine;

namespace Selection
{
    public class SelectionHandler : MonoBehaviour
    {
        [SerializeField] private GameObject rightSelectionPrefab, leftSelectionPrefab;
        private GameObject _selectionGameObject;
        private Hex _selectedHex;
        private bool isLeft;
        private Vector2 rotationPoint;

        public void SelectHex(Hex hex, HexagonVertexes hexagonVertex)
        {
            _selectedHex = hex;

            if (hexagonVertex == HexagonVertexes.BottomLeft)
            {
                hex = hex.GetNeighbor(HexagonEdges.BottomLeft);
                isLeft = false;
            } else if (hexagonVertex == HexagonVertexes.BottomRight)
            {
                hex = hex.GetNeighbor(HexagonEdges.BottomRight);
                isLeft = true;
            } else if (hexagonVertex == HexagonVertexes.TopLeft)
            {
                hex = hex.GetNeighbor(HexagonEdges.TopLeft);
                isLeft = false;
            } else if (hexagonVertex == HexagonVertexes.TopRight)
            {
                hex = hex.GetNeighbor(HexagonEdges.TopRight);
                isLeft = true;
            } else if (hexagonVertex == HexagonVertexes.Right)
            {
                isLeft = false;
            } else
            {
                isLeft = true;
            }

            if (_selectionGameObject)
            {
                Destroy(_selectionGameObject);
            }
            _selectionGameObject = Instantiate(isLeft ? leftSelectionPrefab : rightSelectionPrefab, hex.transform.position, Quaternion.identity);
        }
    }
}