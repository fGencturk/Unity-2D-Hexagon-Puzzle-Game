using System.Collections.Generic;
using UnityEngine;

namespace Hexagon
{
    public class Board
    {
        // Although board keeps elements in two dimensional array, it exposes them as a array with doubled-coordinates
        // private Hex[,] _board and GetElement function achieves this behavior.
        // See Representation : https://pbs.twimg.com/media/Dd7Gk-FVQAEYryG.jpg "double-height"
        private Hex[,] _board;
        public Vector2Int boardSize;
        
        public Board(Vector2Int boardSize)
        {
            this.boardSize = boardSize;
            _board = new Hex[this.boardSize.y, this.boardSize.x];
        }

        public Hex GetElement(int row, int column)
        {
            row = row / 2;
            return _board[row, column];
        }
        public Hex GetElement(Vector2Int position)
        {
            return GetElement(position.y, position.x);
        }

        public void SetElement(int row, int column, Hex hex)
        {
            hex.ChangeIndex(new Vector2Int(column, row));
            row = row / 2;
            _board[row, column] = hex;
        }

        public void SetElement(Vector2Int pos, Hex hex)
        {
            SetElement(pos.y, pos.x, hex);
        }

        public IEnumerable<Hex> GetTopRowIterator()
        {
            return new BoardTopRowIterator(this);
        }
    }
}