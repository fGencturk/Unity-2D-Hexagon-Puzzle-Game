using DefaultNamespace;
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

        public void SelectHex(Hex hex, SelectionAreas selectionArea)
        {
            _selectedHex = hex;

            if (selectionArea == SelectionAreas.BottomLeft)
            {
                hex = hex.GetNeighbor(HexSides.BottomLeft);
                isLeft = false;
            } else if (selectionArea == SelectionAreas.BottomRight)
            {
                hex = hex.GetNeighbor(HexSides.BottomRight);
                isLeft = true;
            } else if (selectionArea == SelectionAreas.TopLeft)
            {
                hex = hex.GetNeighbor(HexSides.TopLeft);
                isLeft = false;
            } else if (selectionArea == SelectionAreas.TopRight)
            {
                hex = hex.GetNeighbor(HexSides.TopRight);
                isLeft = true;
            } else if (selectionArea == SelectionAreas.Right)
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