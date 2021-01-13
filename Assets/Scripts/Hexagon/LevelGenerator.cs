using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

namespace Hexagon
{
    public class LevelGenerator : MonoBehaviour
    {
        public void FillBoard()
        {
            HexPositionCalculator positionCalculator = GameManager.instance.positionCalculator;

            // Fill even-columns
            for (int r = 0; r < positionCalculator.boardSize.y * 2; r+=2)
            {
                for (int c = 0; c < positionCalculator.boardSize.x; c += 2)
                {
                    GameManager.instance.CreateHexGameObject(new Vector2Int(c, r));
                }
            }
            // Fill odd-columns
            for (int r = 1; r < positionCalculator.boardSize.y * 2; r+=2)
            {
                for (int c = 1; c < positionCalculator.boardSize.x; c += 2)
                {
                    Hex hex = GameManager.instance.CreateHexGameObject( new Vector2Int(c, r));
                    int randomIndex = GetRandomIndex(GetExcludedHextypes(hex));
                    hex.Initialize(new Vector2Int(c,r),  randomIndex);
                }
            }
        }

        public HashSet<int> GetExcludedHextypes(Hex hex)
        {
            HexagonEdges[] checkMatchingTriangle = new[]
            {
                HexagonEdges.BottomLeft, HexagonEdges.TopLeft, HexagonEdges.Top, HexagonEdges.TopRight
            };
            HashSet<int> excluding = new HashSet<int>();

            // In odd columns check if there is 2-Neighbors with the same color.
            // If so, exclude that color in random number creation in Hex being created at the moment
            foreach (var edge in checkMatchingTriangle)
            {
                // If has those neighbors
                if (hex.HasNeighborHex(edge) && hex.HasNeighborHex(edge.Next()))
                {
                    // If those neighbors are the same type
                    Hex neighbor1 = hex.GetNeighbor(edge),
                        neighbor2 = hex.GetNeighbor(edge.Next());
                    if (neighbor1.hexType == neighbor2.hexType)
                    {
                        excluding.Add(neighbor1.hexType);
                    }
                }
            }

            return excluding;
        }
        
        int GetRandomIndex(HashSet<int> exclude)
        {
            var range = Enumerable.Range(0, GameManager.instance.hexPrefabs.Count).Where(i => !exclude.Contains(i));
            int index = Random.Range(0, GameManager.instance.hexPrefabs.Count - exclude.Count);
            return range.ElementAt(index);
        }
    }
}