using System;
using System.Collections.Generic;

namespace Hexagon
{
    public enum HexagonVertexes
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        Left,
        Right
    }

    static class HexagonVertexesMethods
    {

        private static HexagonVertexes[] _clockwiseOrder = new[]
        {
            HexagonVertexes.Left, HexagonVertexes.TopLeft, HexagonVertexes.TopRight, HexagonVertexes.Right,
            HexagonVertexes.BottomRight, HexagonVertexes.BottomLeft
        };
        
        public static List<HexagonEdges> GetNeighborEdges(this HexagonVertexes hexagonVertex)
        {
            if (hexagonVertex == HexagonVertexes.Left)
            {
                return new List<HexagonEdges>()
                {
                    HexagonEdges.TopLeft,
                    HexagonEdges.BottomLeft
                };
            }

            if (hexagonVertex == HexagonVertexes.Right)
            {
                return new List<HexagonEdges>()
                {
                    HexagonEdges.BottomRight,
                    HexagonEdges.TopRight
                };
            }

            if (hexagonVertex == HexagonVertexes.BottomLeft)
            {
                return new List<HexagonEdges>()
                {
                    HexagonEdges.BottomLeft,
                    HexagonEdges.Bottom
                };
            }

            if (hexagonVertex == HexagonVertexes.BottomRight)
            {
                return new List<HexagonEdges>()
                {
                    HexagonEdges.BottomRight, 
                    HexagonEdges.Bottom
                };
            }

            if (hexagonVertex == HexagonVertexes.TopRight)
            {
                return new List<HexagonEdges>()
                {
                    HexagonEdges.TopRight, 
                    HexagonEdges.Top
                };
            }

            return new List<HexagonEdges>()
            {
                HexagonEdges.TopLeft, 
                HexagonEdges.Top
            };
        }

        public static HexagonVertexes Next(this HexagonVertexes selection, bool clockwise = true)
        {
            int index = Array.IndexOf(_clockwiseOrder, selection);
            int increment = clockwise ? 1 : -1;
            index = (index + increment + _clockwiseOrder.Length) % _clockwiseOrder.Length;
            return _clockwiseOrder[index];
        }
    }
}