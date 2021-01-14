using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hexagon.HexCreator;
using UI;
using UnityEngine;
using UnityEngine.Events;
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
        [HideInInspector] public int hexTypesCount;
        
        [SerializeField] public List<Sprite> hexSprites;
        [SerializeField] public BombWidget bombWidgetPrefab;
        [SerializeField] public Hex hexPrefab;
        public HexPositionCalculator positionCalculator { get; private set; }
        
        [SerializeField] private Vector2Int boardSize = new Vector2Int(8,9);
        [SerializeField] private bool forceRewindAfterNoMatch = true;
        [SerializeField] private LevelGenerator levelGenerator;
        [SerializeField] private RandomHexCreator hexCreator;

        public delegate void OnHexagonGroupExplode (int count);
        public event OnHexagonGroupExplode onHexagonGroupExplode = (int a) => {};
        public delegate void OnGameOverDelegate ();
        public event OnGameOverDelegate onGameOver = () => {  };
        public delegate void OnRestartDelegate ();
        public event OnRestartDelegate onRestart = () => {  };
        public delegate void OnPlayerActionEndDelegate ();
        public event OnPlayerActionEndDelegate OnPlayerActionEnd = () => {};
        
        private Dictionary<int, Hex> _explodedHexs = new Dictionary<int, Hex>();
        private Dictionary<int, ShiftedHex> _shiftedHexes = new Dictionary<int, ShiftedHex>();
        private Dictionary<int, int> _columnToExplodedHexCount = new Dictionary<int, int>();
        private Board _board;
        private int _lastCreatedRow,
            _lastExplodedRow;
        
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
            hexTypesCount = hexSprites.Count;
            // Assuming all sprites have the same width and height
            SpriteRenderer spriteRenderer = hexPrefab.GetComponent<SpriteRenderer>();
            Vector2 hexagonSize = spriteRenderer.bounds.size;
            _lastCreatedRow = boardSize.y * 2 - 1;
            
            positionCalculator = new HexPositionCalculator(boardSize, hexagonSize);
        }

        private void Start()
        {
            Initialize();
        }

        public IEnumerable<Hex> GetTopRowIterator()
        {
            return _board.GetTopRowIterator();
        }
        
        void Initialize()
        {
            _board = new Board(boardSize);
            
            levelGenerator.FillBoard();
            canTakeAction = true;
        }

        private bool IsThereAnyMoveLeft()
        {
            List<Hex> neighbors = new List<Hex>();
            
            foreach (Hex topHex in GetTopRowIterator())
            {
                Hex element = topHex;
                while (true)
                {
                    neighbors.Add(element);
                    foreach (HexagonEdges edge in Enum.GetValues(typeof(HexagonEdges)))
                    {
                        if (element.HasNeighborHex(edge))
                        {
                            neighbors.Add(element.GetNeighbor(edge));
                        }
                    }
                    Dictionary<int, int> hexTypeCount = new Dictionary<int, int>();
                    // There should be 3 Hexagons with the same TYPE in all neighbors including center hexagon
                    // So that there is a possible move
                    foreach (var neighbor in neighbors)
                    {
                        hexTypeCount[neighbor.hexType] = hexTypeCount.ContainsKey(neighbor.hexType)
                            ? hexTypeCount[neighbor.hexType] + 1
                            : 1;
                    }

                    foreach (var count in hexTypeCount.Values)
                    {
                        if (count >= 3)
                        {
                            return true;
                        }
                    }

                    neighbors.Clear();
                    if (!element.HasNeighborHex(HexagonEdges.Bottom))
                    {
                        break;
                    }

                    element = element.GetNeighbor(HexagonEdges.Bottom);
                }
            }

            return false;
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
                hexs[i].animator.StartRotateAnimation(clockwise);
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
                        hexs[i].animator.StartRotateAnimation(!clockwise);
                    }
                    
                }
            }
            
            StartCoroutine(HandleMove(() => Reverse()));
        }

        private IEnumerator HandleMove(Action onNoMatch = null)
        {
            yield return new WaitForSeconds(AnimationData.delayAfterRotation);
            bool isThereMatch = false;
            do
            {
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
                onHexagonGroupExplode(_explodedHexs.Count);

                isThereMatch = true;
                
                CalculateShifts();
                CalculateNewHexagons();
                DestroyExplodedHexes();
                
                yield return new WaitForSeconds(AnimationData.delayAfterExplosion);

                TriggerShift();
                
                yield return new WaitForSeconds(AnimationData.delayAfterRound);
            } while (true);

            if (isThereMatch)
            {
                OnPlayerActionEnd();
            }

            if (!IsThereAnyMoveLeft())
            {
                TriggerGameOver();
            }
        }

        private void TriggerShift()
        {
            
            // Handle New Creation
            foreach (var pair in _columnToExplodedHexCount)
            {
                int column = pair.Key;
                int rowCount = pair.Value;
                int startRow = column % 2 == 1 ? 1 : 0;
                for (int row = startRow; row < rowCount * 2; row += 2)
                {
                    Vector2Int position = new Vector2Int(column, row);
                    CreateHexGameObject(position);
                }
            }
            _lastCreatedRow = boardSize.y * 2 - 1;
            // Handle Shift
            foreach (var shift in _shiftedHexes.Values)
            {
                Vector2Int newPosition = shift.hex.index + new Vector2Int(0, shift.shiftedRow * 2);
                SetElementWithMoveAnimation(newPosition, shift.hex);
            }
        }

        public void TriggerGameOver()
        {
            canTakeAction = false;
            onGameOver();
        }

        private void DestroyExplodedHexes()
        {
            foreach (var hex in _explodedHexs.Values)
            {
                Destroy(hex.gameObject);
            }
                
            _explodedHexs.Clear();
        }

        private void CalculateNewHexagons()
        {
            // Create new hexes for destroyed hexes
            _columnToExplodedHexCount.Clear();
            for (int i = 0; i < boardSize.x; i++)
            {
                _columnToExplodedHexCount[i] = 0;
            }

            _lastCreatedRow = -1;
            foreach (var hex in _explodedHexs.Values)
            {
                _columnToExplodedHexCount[hex.index.x]++;
                int row = _columnToExplodedHexCount[hex.index.x] * 2;
                row += hex.index.x % 2 == 1 ? 1 : 0;
                if (row > _lastCreatedRow)
                {
                    _lastCreatedRow = row;
                }
            }

        }

        public Hex CreateHexGameObject(Vector2Int position)
        {
            Hex hex = hexCreator.CreateHexGameObject(position);
            SetElementWithMoveAnimation(position, hex);
            return hex;
        }

        public void SetElementWithMoveAnimation(Vector2Int position, Hex hex)
        {
            _board.SetElement(position, hex);
            hex.animator.StartMoveAnimation(_lastCreatedRow);
        }

        private void CalculateShifts()
        {
            // Find all hexes which will shift
            _shiftedHexes.Clear();
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

                    if (_shiftedHexes.ContainsKey(id))
                    {
                        _shiftedHexes[id].shiftedRow++;
                    }
                    else
                    {
                        _shiftedHexes[id] = new ShiftedHex()
                        {
                            hex = iteratedHex,
                            shiftedRow = 1
                        };
                    }
                }
            }
        }

        private void FindAllExplodedHexs()
        {
            foreach (Hex topRowElement in GetTopRowIterator())
            {
                Hex element = topRowElement;

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

        public void TriggerRestart()
        {
            foreach(Transform child in hexCreator.transform)
            {
                Destroy(child.gameObject);
            }
            onRestart();
            Initialize();
        }
    }
}