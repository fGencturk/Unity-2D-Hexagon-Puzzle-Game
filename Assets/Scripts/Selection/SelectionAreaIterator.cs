using System.Collections;
using System.Collections.Generic;

namespace Selection
{
    public class SelectionAreaIterator : IEnumerable<SelectionAreas>
    {
        private SelectionAreas _selectionArea;

        private int count = 0;

        public SelectionAreaIterator(SelectionAreas selectionArea)
        {
            _selectionArea = selectionArea;
        }


        public IEnumerator<SelectionAreas> GetEnumerator()
        {
            SelectionAreas clockwise = _selectionArea,
                counterClockwise = _selectionArea;
            yield return _selectionArea;
            for (count = 0; count < 5; count++)
            {
                if (count % 2 == 0)
                {
                    clockwise = SelectionAreasUtility.Next(clockwise);
                    yield return clockwise;
                }
                else
                {
                    counterClockwise = SelectionAreasUtility.Next(counterClockwise, false);
                    yield return counterClockwise;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}