using System;
using System.Collections.Generic;
using DefaultNamespace;

namespace Selection
{
    public enum SelectionAreas
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        Left,
        Right
    }

    public class SelectionAreasUtility
    {

        private static SelectionAreas[] _clockwiseOrder = new[]
        {
            SelectionAreas.Left, SelectionAreas.TopLeft, SelectionAreas.TopRight, SelectionAreas.Right,
            SelectionAreas.BottomRight, SelectionAreas.BottomLeft
        };
        
        public static List<HexSides> GetRequiredNeighbors(SelectionAreas selectionArea)
        {
            if (selectionArea == SelectionAreas.Left)
            {
                return new List<HexSides>()
                {
                    HexSides.TopLeft,
                    HexSides.BottomLeft
                };
            }

            if (selectionArea == SelectionAreas.Right)
            {
                return new List<HexSides>()
                {
                    HexSides.BottomRight,
                    HexSides.TopRight
                };
            }

            if (selectionArea == SelectionAreas.BottomLeft)
            {
                return new List<HexSides>()
                {
                    HexSides.BottomLeft,
                    HexSides.Bottom
                };
            }

            if (selectionArea == SelectionAreas.BottomRight)
            {
                return new List<HexSides>()
                {
                    HexSides.BottomRight, 
                    HexSides.Bottom
                };
            }

            if (selectionArea == SelectionAreas.TopRight)
            {
                return new List<HexSides>()
                {
                    HexSides.TopRight, 
                    HexSides.Top
                };
            }

            return new List<HexSides>()
            {
                HexSides.TopLeft, 
                HexSides.Top
            };
        }

        public static SelectionAreas Next(SelectionAreas selection, bool clockwise = true)
        {
            int index = Array.IndexOf(_clockwiseOrder, selection);
            int increment = clockwise ? 1 : -1;
            index = (index + increment + _clockwiseOrder.Length) % _clockwiseOrder.Length;
            return _clockwiseOrder[index];
        }
    }
}