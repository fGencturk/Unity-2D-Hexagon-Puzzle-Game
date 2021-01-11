using System;

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

        public static HexagonEdges Next(this HexagonEdges edge, bool clockwise = true)
        {
            int index = Array.IndexOf(_clockwiseOrder, edge);
            int increment = clockwise ? 1 : -1;
            index = (index + increment + _clockwiseOrder.Length) % _clockwiseOrder.Length;
            return _clockwiseOrder[index];
        }
    }
}