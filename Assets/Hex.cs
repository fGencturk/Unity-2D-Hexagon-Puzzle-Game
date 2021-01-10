using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class Hex : MonoBehaviour
    {
        private static Dictionary<HexDirection, Vector2Int> directions = new Dictionary<HexDirection, Vector2Int>()
        {
            {HexDirection.Top, new Vector2Int(0, -2)},
            {HexDirection.TopRight, new Vector2Int(1, -1)},
            {HexDirection.TopLeft, new Vector2Int(-1, -1)},
            {HexDirection.Bottom, new Vector2Int(0, 2)},
            {HexDirection.BottomRight, new Vector2Int(1, 1)},
            {HexDirection.BottomLeft, new Vector2Int(-1, 1)}
        };

        public Vector2Int position;
        public int hexType { get; private set; }

        public void Initialize(Vector2Int position, int hexType)
        {
            this.position = position;
            this.hexType = hexType;
        }
    }
}