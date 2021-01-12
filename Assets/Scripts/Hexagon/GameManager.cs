﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;
using Random = UnityEngine.Random;

namespace Hexagon
{
    public class GameManager : MonoBehaviour
    {
        private class ShiftedHex
        {
            public Hex hex;
            public int shiftedRow;
        }
        
        public static GameManager instance;

        [HideInInspector] public bool canTakeAction = false;
        
        [SerializeField] public List<GameObject> hexPrefabs;
        [SerializeField] public float hexMoveAnimationDuration = 2f;
        public HexPositionCalculator positionCalculator { get; private set; }
        
        [SerializeField] private Vector2Int boardSize = new Vector2Int(8,9);
        [SerializeField] private bool forceRewindAfterNoMatch = true;
        [SerializeField] private LevelGenerator levelGenerator;
        


        private Dictionary<int, Hex> _explodedHexs = new Dictionary<int, Hex>();
        private Board _board;
        public Hex GetElement(Vector2Int indexes)
        {
            return _board.GetElement(indexes);
        }
        
        private void Awake()
        {
            if (instance)
            {
                Debug.LogError("There is already a GameManager instance. Destroying self.");
                Destroy(gameObject);
                return;
            }
            instance = this;
            
            // Assuming all sprites have the same width and height
            SpriteRenderer spriteRenderer = hexPrefabs[0].GetComponent<SpriteRenderer>();
            Vector2 hexagonSize = spriteRenderer.bounds.size;
            
            positionCalculator = new HexPositionCalculator(boardSize, hexagonSize);
            _board = new Board(boardSize);
            
            levelGenerator.FillBoard();
            canTakeAction = true;
        }
        
        public void Rotate(Hex hex, HexagonEdges firstNeighbor, bool clockwise = true)
        {
            canTakeAction = false;
            HexagonEdges secondNeighbor = firstNeighbor.Next();
            if (!hex.HasNeighborHex(firstNeighbor) || !hex.HasNeighborHex(secondNeighbor))
            {
                return;
            }

            List<Hex> hexs = new List<Hex>()
            {
                hex,
                hex.GetNeighbor(firstNeighbor),
                hex.GetNeighbor(secondNeighbor)
            };
            if (!clockwise)
            {
                hexs.Reverse();
            }
            
            List<Vector2Int> tempPositions = new List<Vector2Int>();
            foreach (var curHex in hexs)
            {
                tempPositions.Add(curHex.index);
            }

            for (int i = 0; i < 3; i++)
            {
                int nextIndex = (i + 1) % 3;
                Vector2Int indexes = tempPositions[nextIndex];
                _board.SetElement(indexes, hexs[i]);
            }
            // Reverse rotation method will be invoked if there is no match
            void Reverse()
            {
                if (forceRewindAfterNoMatch)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Vector2Int indexes = tempPositions[i];
                        _board.SetElement(indexes, hexs[i]);
                    }
                    
                }
            }
            
            StartCoroutine(HandleMove(() => Reverse()));
        }

        private IEnumerator HandleMove(Action onNoMatch = null)
        {
            bool isThereMatch = false;
            do
            {
                yield return new WaitForSeconds(hexMoveAnimationDuration);
                FindAllExplodedHexs();
                if (_explodedHexs.Count == 0)
                {
                    if (!isThereMatch)
                    {
                        onNoMatch?.Invoke();
                    }

                    canTakeAction = true;
                    break;
                }

                isThereMatch = true;
                
                ShiftBoard();
                CreateNewHexes();
                DestroyExplodedHexes();
            } while (true);
        }
        
        private void DestroyExplodedHexes()
        {
            foreach (var hex in _explodedHexs.Values)
            {
                Destroy(hex.gameObject);
            }
                
            _explodedHexs.Clear();
        }

        private void CreateNewHexes()
        {
            // Create new hexes for destroyed hexes
            Dictionary<int, int> columnToExplodedHexCount = new Dictionary<int, int>();
            for (int i = 0; i < boardSize.x; i++)
            {
                columnToExplodedHexCount[i] = 0;
            }

            foreach (var hex in _explodedHexs.Values)
            {
                columnToExplodedHexCount[hex.index.x]++;
            }

            foreach (var pair in columnToExplodedHexCount)
            {
                int column = pair.Key;
                int rowCount = pair.Value;
                int startRow = column % 2 == 1 ? 1 : 0;
                for (int row = startRow; row < rowCount * 2; row += 2)
                {
                    Vector2Int position = new Vector2Int(column, row);
                    CreateNewHex(position);
                }
            }
            
        }

        private void CreateNewHex(Vector2Int position)
        {
            int type = Random.Range(0, hexPrefabs.Count);
            GameObject hexGameObject = Instantiate(hexPrefabs[type], new Vector2(0, 10f), Quaternion.identity, transform) as GameObject;
            Hex hex = hexGameObject.AddComponent<Hex>();
            hex.Initialize(new Vector2Int(position.x, position.y), type );
            _board.SetElement(position, hex);
        }

        private void ShiftBoard()
        {
            // Find all hexes which will shift
            Dictionary<int, ShiftedHex> shiftedHexes = new Dictionary<int, ShiftedHex>();
            foreach (Hex hex in _explodedHexs.Values)
            {
                Hex iteratedHex = hex;
                while (iteratedHex.HasNeighborHex(HexagonEdges.Top))
                {
                    iteratedHex = iteratedHex.GetNeighbor(HexagonEdges.Top);
                    int id = iteratedHex.GetInstanceID();
                    // If this Hex already exploded in this turn, no need for shift for this hex
                    if (_explodedHexs.ContainsKey(id))
                    {
                        continue;
                    }

                    if (shiftedHexes.ContainsKey(id))
                    {
                        shiftedHexes[id].shiftedRow++;
                    }
                    else
                    {
                        shiftedHexes[id] = new ShiftedHex()
                        {
                            hex = iteratedHex,
                            shiftedRow = 1
                        };
                    }
                }
            }
            // Handle Shift
            foreach (var shift in shiftedHexes.Values)
            {
                Vector2Int newPosition = shift.hex.index + new Vector2Int(0, shift.shiftedRow * 2);
                _board.SetElement(newPosition, shift.hex);
            }
        }

        private void FindAllExplodedHexs()
        {
            for (int column = 0; column < boardSize.x; column++)
            {
                int firstElementRow = column % 2 == 0 ? 0 : 1;
                Hex element = _board.GetElement(firstElementRow, column);

                while (true)
                {
                    CheckMatchAllNeighbors(element);
                    if (element.HasNeighborHex(HexagonEdges.Bottom))
                    {
                        element = element.GetNeighbor(HexagonEdges.Bottom);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private void CheckMatchAllNeighbors(Hex hex)
        {
            foreach (HexagonEdges edge in Enum.GetValues(typeof(HexagonEdges)))
            {
                if (CheckMatchNeighborClockwise(hex, edge))
                {
                    _explodedHexs[hex.GetInstanceID()] = hex;
                    
                    Hex neighbor = hex.GetNeighbor(edge);
                    _explodedHexs[neighbor.GetInstanceID()] = neighbor;

                    neighbor = hex.GetNeighbor(edge.Next());
                    _explodedHexs[neighbor.GetInstanceID()] = neighbor;
                }
            }
        }
        
        private bool CheckMatchNeighborClockwise(Hex hex, HexagonEdges hexagonEdges)
        {
            // If there is no tile in directions, return false
            if (!hex.HasNeighborHex(hexagonEdges) ||
                !hex.HasNeighborHex(hexagonEdges.Next()))
            {
                return false;
            }
            // Otherwise, check other hex tiles for match
            Hex hex1 = hex.GetNeighbor(hexagonEdges),
                hex2 = hex.GetNeighbor(hexagonEdges.Next());

            return hex.hexType == hex1.hexType && hex.hexType == hex2.hexType;
        }

        public Hex CreateHexGameObject(Vector2Int indexes, int type)
        {
            Vector2 position = positionCalculator.GetPosition(indexes);
            GameObject hexGameObject = Instantiate(GameManager.instance.hexPrefabs[type], position, Quaternion.identity, transform) as GameObject;
            Hex hex = hexGameObject.AddComponent<Hex>();
            hex.Initialize(indexes, type);
            _board.SetElement(indexes, hex);
            return hex;
        }
    }
}