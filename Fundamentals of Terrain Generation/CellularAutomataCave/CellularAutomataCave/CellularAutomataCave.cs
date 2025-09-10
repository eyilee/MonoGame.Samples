using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace CellularAutomataCave
{
    public class CellularAutomataCave
    {
        private static readonly Random _random = new ();

        private readonly Texture2D _texture;
        private readonly int _cellSize = 4;

        private readonly int _size;
        private readonly int _iteration;
        private readonly float _aliveRate;
        private readonly int _minCaveSize;

        private static readonly Tuple<int, int>[] _neighborOffsets =
        [
            new (-1, 1),
            new (0, 1),
            new (1, 1),
            new (-1, 0),
            new (1, 0),
            new (-1, -1),
            new (0, -1),
            new (1, -1)
        ];

        private bool[][] _cellMap;
        private int _currentIndex = 0;
        private int _currentIteration = 0;

        private bool[] _traversalMap;
        private readonly Queue<Tuple<int, int>> _traversalQueue = new ();
        private readonly List<Tuple<int, int>> _currentCave = [];
        private readonly List<Tuple<int, int>> _mainCave = [];
        private readonly List<List<Tuple<int, int>>> _subCaves = [];

        public enum EStepState
        {
            CellularAutomata,
            FloodFill,
            ConnectCaves,
            Finished,
        }

        private EStepState _state;

        private int GetIndex (int x, int y)
        {
            x = ((x % _size) + _size) % _size;
            y = ((y % _size) + _size) % _size;
            return x + (y * _size);
        }

        private Tuple<int, int> GetCoordinates (int index)
        {
            int x = index % _size;
            int y = index / _size;
            return new Tuple<int, int> (x, y);
        }

        public CellularAutomataCave (int size, int iteration, float aliveRate)
        {
            _texture = new Texture2D (Core.GraphicsDevice, 1, 1);
            _texture.SetData ([Color.White]);

            if (size <= 0)
            {
                throw new ArgumentException ("Size must be greater than 0", nameof (size));
            }

            _size = size;
            _iteration = iteration;
            _aliveRate = aliveRate;
            _minCaveSize = int.Log2 (size) * int.Log2 (size);

            Reset ();
        }

        public void Reset ()
        {
            _cellMap = new bool[2][];

            for (int i = 0; i < _cellMap.Length; i++)
            {
                _cellMap[i] = new bool[_size * _size];
                Array.Fill (_cellMap[i], false);
            }

            _currentIndex = 0;
            _currentIteration = 0;

            for (int x = 0; x < _size; x++)
            {
                for (int y = 0; y < _size; y++)
                {
                    _cellMap[_currentIndex][GetIndex (x, y)] = _random.NextDouble () < _aliveRate;
                }
            }

            _traversalMap = new bool[_size * _size];
            Array.Fill (_traversalMap, false);

            _traversalQueue.Clear ();
            _currentCave.Clear ();
            _mainCave.Clear ();
            _subCaves.Clear ();

            _state = EStepState.CellularAutomata;
        }

        public void NextStep ()
        {
            if (_state == EStepState.CellularAutomata)
            {
                NextCellularAutomataStep ();
            }
            else if (_state == EStepState.FloodFill)
            {
                NextFloodFillStep ();
            }
            else if (_state == EStepState.ConnectCaves)
            {
                NextConnectCavesStep ();
            }
        }

        private void NextCellularAutomataStep ()
        {
            int nextIndex = (_currentIndex + 1) % _cellMap.Length;

            if (_currentIteration >= _iteration)
            {
                _state = EStepState.FloodFill;
                return;
            }

            _currentIteration++;

            for (int x = 0; x < _size; x++)
            {
                for (int y = 0; y < _size; y++)
                {
                    int aliveNeighbors = 0;
                    foreach ((int xOffset, int yOffset) in _neighborOffsets)
                    {
                        if (x + xOffset < 0 || x + xOffset >= _size || y + yOffset < 0 || y + yOffset >= _size)
                        {
                            continue;
                        }

                        if (_cellMap[_currentIndex][GetIndex (x + xOffset, y + yOffset)])
                        {
                            aliveNeighbors++;
                        }
                    }

                    bool isAlive = _cellMap[_currentIndex][GetIndex (x, y)];
                    if (isAlive)
                    {
                        if (aliveNeighbors < 3)
                        {
                            _cellMap[nextIndex][GetIndex (x, y)] = false;
                        }
                        else
                        {
                            _cellMap[nextIndex][GetIndex (x, y)] = true;
                        }
                    }
                    else
                    {
                        if (aliveNeighbors > 4)
                        {
                            _cellMap[nextIndex][GetIndex (x, y)] = true;
                        }
                        else
                        {
                            _cellMap[nextIndex][GetIndex (x, y)] = false;
                        }
                    }
                }
            }

            _currentIndex = nextIndex;
        }

        private void NextFloodFillStep ()
        {
            bool hasChanged = false;

            for (int index = 0; index < _traversalMap.Length; index++)
            {
                if (_traversalMap[index])
                {
                    continue;
                }

                (int x, int y) = GetCoordinates (index);

                _traversalQueue.Clear ();
                _traversalQueue.Enqueue (new Tuple<int, int> (x, y));

                _currentCave.Clear ();

                while (_traversalQueue.Count > 0)
                {
                    (int currentX, int currentY) = _traversalQueue.Dequeue ();

                    if (_traversalMap[GetIndex (currentX, currentY)])
                    {
                        continue;
                    }

                    _traversalMap[GetIndex (currentX, currentY)] = true;

                    if (_cellMap[_currentIndex][GetIndex (currentX, currentY)])
                    {
                        _currentCave.Add (new Tuple<int, int> (currentX, currentY));

                        if (currentX - 1 >= 0 && !_traversalMap[GetIndex (currentX - 1, currentY)])
                        {
                            _traversalQueue.Enqueue (new Tuple<int, int> (currentX - 1, currentY));
                        }

                        if (currentY + 1 < _size && !_traversalMap[GetIndex (currentX, currentY + 1)])
                        {
                            _traversalQueue.Enqueue (new Tuple<int, int> (currentX, currentY + 1));
                        }

                        if (currentX + 1 < _size && !_traversalMap[GetIndex (currentX + 1, currentY)])
                        {
                            _traversalQueue.Enqueue (new Tuple<int, int> (currentX + 1, currentY));
                        }

                        if (currentY - 1 >= 0 && !_traversalMap[GetIndex (currentX, currentY - 1)])
                        {
                            _traversalQueue.Enqueue (new Tuple<int, int> (currentX, currentY - 1));
                        }
                    }
                }

                if (_currentCave.Count > 0)
                {
                    if (_currentCave.Count >= _minCaveSize)
                    {
                        if (_currentCave.Count > _mainCave.Count)
                        {
                            if (_mainCave.Count > 0)
                            {
                                _subCaves.Add ([.. _mainCave]);
                            }

                            _mainCave.Clear ();
                            _mainCave.AddRange (_currentCave);
                        }
                        else
                        {
                            _subCaves.Add ([.. _currentCave]);
                        }
                    }
                    else
                    {
                        foreach ((int caveX, int caveY) in _currentCave)
                        {
                            _cellMap[_currentIndex][GetIndex (caveX, caveY)] = false;
                        }
                    }

                    _currentCave.Clear ();

                    hasChanged = true;
                }
            }

            if (!hasChanged)
            {
                _state = EStepState.ConnectCaves;
            }
        }

        private void NextConnectCavesStep ()
        {
            bool hasChanged = false;

            while (_subCaves.Count > 0)
            {
                List<Tuple<int, int>> subcave = _subCaves[^1];
                (int subCaveX, int subCaveY) = subcave[_random.Next (0, subcave.Count)];
                (int mainCaveX, int mainCaveY) = _mainCave[_random.Next (0, _mainCave.Count)];

                List<int> path = AstarPathFinding.FindPath (_cellMap[_currentIndex], _size, GetIndex (subCaveX, subCaveY), GetIndex (mainCaveX, mainCaveY));
                if (path.Count > 0)
                {
                    foreach (int pathIndex in path)
                    {
                        (int pathX, int pathY) = GetCoordinates (pathIndex);
                        _cellMap[_currentIndex][GetIndex (pathX, pathY)] = true;
                    }
                }

                _subCaves.RemoveAt (_subCaves.Count - 1);

                hasChanged = true;

                break;
            }

            if (!hasChanged)
            {
                _state = EStepState.Finished;
            }
        }

        public void Draw (SpriteBatch spriteBatch)
        {
            Color aliveColor = new (224, 224, 224, 255);
            Color deadColor = new (32, 32, 32, 255);

            for (int index = 0; index < _cellMap[_currentIndex].Length; index++)
            {
                DrawCell (spriteBatch, index, _cellMap[_currentIndex][index] ? aliveColor : deadColor);
            }
        }

        private void DrawCell (SpriteBatch spriteBatch, int index, Color color)
        {
            (int x, int y) = GetCoordinates (index);
            spriteBatch.Draw (_texture, GetCellRectangle (x, y), color);
        }

        private Rectangle GetCellRectangle (int x, int y)
        {
            int halfSize = _size * _cellSize / 2;
            return new Rectangle ((x * _cellSize) - halfSize, halfSize - (y * _cellSize) - _cellSize, _cellSize, _cellSize);
        }
    }
}
