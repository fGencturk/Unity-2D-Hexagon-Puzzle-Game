using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hexagon
{
    public enum HexagonEdges
    {
        Top,
        TopLeft,
        TopRight,
        Bottom,
        BottomLeft,
        BottomRight
    }
    static class HexagonEdgesMethods
    {

        private static HexagonEdges[] _clockwiseOrder = new HexagonEdges[]
        {
            HexagonEdges.Top, HexagonEdges.TopRight, HexagonEdges.BottomRight,HexagonEdges.Bottom, HexagonEdges.BottomLeft, HexagonEdges.TopLeft
        };
        
        private static Dictionary<HexagonEdges, Vector2Int> _directionToVector = new Dictionary<HexagonEdges, Vector2Int>()
        {
            {HexagonEdges.Top, new Vector2Int(0, -2)},
            {HexagonEdges.TopRight, new Vector2Int(1, -1)},
            {HexagonEdges.TopLeft, new Vector2Int(-1, -1)},
            {HexagonEdges.Bottom, new Vector2Int(0, 2)},
            {HexagonEdges.BottomRight, new Vector2Int(1, 1)},
            {HexagonEdges.BottomLeft, new Vector2Int(-1, 1)}
        };

        public static HexagonEdges Next(this HexagonEdges edge, bool clockwise = true)
        {
            int index = Array.IndexOf(_clockwiseOrder, edge);
            int increment = clockwise ? 1 : -1;
            index = (index + increment + _clockwiseOrder.Length) % _clockwiseOrder.Length;
            return _clockwiseOrder[index];
        }

        public static Vector2Int GetVector(this HexagonEdges edge)
        {
            return _directionToVector[edge];
        }
    }
}