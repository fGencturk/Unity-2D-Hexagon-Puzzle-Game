using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class LevelGenerator : MonoBehaviour
    {
        private void Start()
        {
            FillBoard();
        }

        public void FillBoard()
        {
            Vector2 _spriteSize = Board.instance.GetSpriteSizeInUnits();
            float xMaxSize = _spriteSize.x * Board.XMovementMultiplier, 
                yMaxSize = _spriteSize.y;
            Vector2 boardSize = Board.instance.GetBoardSizeInUnits();
            float startX = -boardSize.x / 2,
                startY = boardSize.y / 2;

            Hex SpawnHex(int row, int column, int type)
            {
                Vector2 position = new Vector2(startX + xMaxSize * column, startY + -yMaxSize / 2 * row);
                GameObject hexGameObject = Instantiate(Board.instance.sprites[type], position, Quaternion.identity, transform) as GameObject;
                Hex hex = hexGameObject.AddComponent<Hex>();
                hex.Initialize(new Vector2Int(column, row), type );
                Board.instance.SetElement(row, column, hex);
                return hex;
            }
            
            // Fill even-columns
            for (int r = 0; r < Board.instance.boardSize.y * 2; r+=2)
            {
                for (int c = 0; c < Board.instance.boardSize.x; c += 2)
                {
                    int createdType = GetRandomIndex();
                    SpawnHex(r, c, createdType);
                }
            }
            // Fill odd-columns
            for (int r = 1; r < Board.instance.boardSize.y * 2; r+=2)
            {
                for (int c = 1; c < Board.instance.boardSize.x; c += 2)
                {
                    Hex hex = SpawnHex(r, c, 0);
                    HashSet<int> excluding = new HashSet<int>();

                    // In odd columns check if there is 2-Neighbors with the same color.
                    // If so, exclude that color in random number creation in Hex being created at the moment
                    if (hex.HasNeighborHex(HexSides.BottomLeft))
                    {
                        if (hex.GetNeighbor(HexSides.BottomLeft).hexType ==
                            hex.GetNeighbor(HexSides.TopLeft).hexType)
                        {
                            excluding.Add(hex.GetNeighbor(HexSides.BottomLeft).hexType);
                        }
                    }
                    if (hex.HasNeighborHex(HexSides.BottomRight))
                    {
                        if (hex.GetNeighbor(HexSides.BottomRight).hexType ==
                            hex.GetNeighbor(HexSides.TopRight).hexType)
                        {
                            excluding.Add(hex.GetNeighbor(HexSides.TopRight).hexType);
                        }
                    }

                    if (hex.HasNeighborHex(HexSides.Top))
                    {
                        if (hex.GetNeighbor(HexSides.Top).hexType ==
                            hex.GetNeighbor(HexSides.TopLeft).hexType)
                        {
                            excluding.Add(hex.GetNeighbor(HexSides.Top).hexType);
                        }

                        if (hex.HasNeighborHex(HexSides.TopRight) && 
                            hex.GetNeighbor(HexSides.Top).hexType ==
                            hex.GetNeighbor(HexSides.TopRight).hexType)
                        {
                            excluding.Add(hex.GetNeighbor(HexSides.Top).hexType);
                            
                        }
                    }

                    int randomIndex = GetRandomIndex(excluding);
                    hex.Initialize(new Vector2Int(c,r),  randomIndex);
                }
            }
        }
        
        int GetRandomIndex()
        {
            return Random.Range(0, Board.instance.sprites.Count);
        }
        
        int GetRandomIndex(HashSet<int> exclude)
        {
            var range = Enumerable.Range(0, Board.instance.sprites.Count).Where(i => !exclude.Contains(i));
            int index = Random.Range(0, Board.instance.sprites.Count - exclude.Count);
            return range.ElementAt(index);
        }
    }
}