using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hexagon
{
    public class BoardTopRowIterator : IEnumerable<Hex>
    {
        private Board _board;

        public BoardTopRowIterator(Board board)
        {
            _board = board;
        }
        
        public IEnumerator<Hex> GetEnumerator()
        {
            for (int column = 0; column < _board.boardSize.x; column++)
            {
                int row = column % 2 == 1 ? 1 : 0;
                yield return _board.GetElement(row, column);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}