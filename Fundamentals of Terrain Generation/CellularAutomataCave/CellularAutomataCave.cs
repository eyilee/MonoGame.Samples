using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Library;
using MonoGame.Library.Graphics;
using System;
using System.Collections.Generic;

namespace CellularAutomataCave
{
    public class CellularAutomataCave : IDisposable
    {
        private readonly Random _random = new ();

        private readonly Canvas _canvas;

        private readonly int _width;

        private readonly int _height;

        private readonly int _size;

        private readonly bool[] _originNodes;

        private bool[] _nodes;

        private bool[] _swapNodes;

        private readonly (int, int)[] _neighbor8Offsets =
        [
            new (-1, 0),
            new (-1, 1),
            new (0, 1),
            new (1, 1),
            new (1, 0),
            new (1, -1),
            new (0, -1),
            new (-1, -1)
        ];

        private readonly (int, int)[] _neighbor4Offsets =
        [
            new (-1, 0),
            new (0, 1),
            new (1, 0),
            new (0, -1)
        ];

        private readonly float _aliveRate;

        private readonly int _iteration;

        private readonly int _minCaveSize;

        private IEnumerator<int>? _stepBehaviour;

        private Color _aliveColor = new (224, 224, 224, 255);

        private Color _deadColor = new (32, 32, 32, 255);

        private bool _disposed;

        public CellularAutomataCave (GraphicsDevice graphicsDevice, int width, int height, int cellSize, float aliveRate, int iteration)
        {
            _canvas = new Canvas (graphicsDevice, "CellularAutomataCave", Core.ScreenWidth / 2, Core.ScreenHeight / 2, width, height, cellSize);
            _width = width;
            _height = height;
            _size = width * height;
            _aliveRate = aliveRate;
            _iteration = iteration;
            _originNodes = new bool[_size];
            _nodes = new bool[_size];
            _swapNodes = new bool[_size];
            _minCaveSize = int.Log2 (width) * int.Log2 (height);

            Reset ();
        }

        ~CellularAutomataCave () => Dispose (false);

        public void Reset ()
        {
            for (int i = 0; i < _size; i++)
            {
                _originNodes[i] = _random.NextDouble () < _aliveRate;
            }

            _originNodes.CopyTo (_nodes, 0);
            _stepBehaviour = Run ();

            Draw ();
        }

        public void Redo ()
        {
            _originNodes.CopyTo (_nodes, 0);
            _stepBehaviour = Run ();

            Draw ();
        }

        public void NextStep ()
        {
            if (_stepBehaviour != null)
            {
                if (!_stepBehaviour.MoveNext ())
                {
                    _stepBehaviour = null;
                }

                Draw ();
            }
        }

        private IEnumerator<int> Run ()
        {
            for (int iteration = 0; iteration < _iteration; iteration++)
            {
                for (int x = 0; x < _width; x++)
                {
                    for (int y = 0; y < _height; y++)
                    {
                        int aliveNeighbors = 0;

                        foreach ((int xOffset, int yOffset) in _neighbor8Offsets)
                        {
                            int neighborX = x + xOffset;
                            int neighborY = y + yOffset;

                            if (neighborX < 0 || neighborX >= _width || neighborY < 0 || neighborY >= _height)
                            {
                                continue;
                            }

                            int neighborIndex = GetIndex (neighborX, neighborY);

                            if (_nodes[neighborIndex])
                            {
                                aliveNeighbors++;
                            }
                        }

                        int index = GetIndex (x, y);

                        if (_nodes[index])
                        {
                            _swapNodes[index] = aliveNeighbors >= 3;
                        }
                        else
                        {
                            _swapNodes[index] = aliveNeighbors > 4;
                        }
                    }
                }

                (_nodes, _swapNodes) = (_swapNodes, _nodes);

                yield return 0;
            }

            List<List<int>> caves = [];

            bool[] visits = new bool[_width * _height];

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    int index = GetIndex (x, y);

                    if (!_nodes[index] || visits[index])
                    {
                        continue;
                    }

                    visits[index] = true;

                    List<int> cave = [index];

                    Queue<int> visitQueue = new ();

                    foreach ((int xOffset, int yOffset) in _neighbor4Offsets)
                    {
                        int neighborX = x + xOffset;
                        int neighborY = y + yOffset;

                        if (neighborX < 0 || neighborX >= _width || neighborY < 0 || neighborY >= _height)
                        {
                            continue;
                        }

                        int neighborIndex = GetIndex (neighborX, neighborY);

                        if (_nodes[neighborIndex] && !visits[neighborIndex])
                        {
                            visitQueue.Enqueue (neighborIndex);
                        }
                    }

                    while (visitQueue.Count > 0)
                    {
                        int visitIndex = visitQueue.Dequeue ();

                        if (visits[visitIndex])
                        {
                            continue;
                        }

                        visits[visitIndex] = true;

                        cave.Add (visitIndex);

                        (int visitX, int visitY) = GetCoordinates (visitIndex);

                        foreach ((int xOffset, int yOffset) in _neighbor4Offsets)
                        {
                            int neighborX = visitX + xOffset;
                            int neighborY = visitY + yOffset;

                            if (neighborX < 0 || neighborX >= _width || neighborY < 0 || neighborY >= _height)
                            {
                                continue;
                            }

                            int neighborIndex = GetIndex (neighborX, neighborY);

                            if (_nodes[neighborIndex] && !visits[neighborIndex])
                            {
                                visitQueue.Enqueue (neighborIndex);
                            }
                        }
                    }

                    if (cave.Count >= _minCaveSize)
                    {
                        caves.Add (cave);
                    }
                    else
                    {
                        foreach (int caveIndex in cave)
                        {
                            _nodes[caveIndex] = false;
                        }
                    }
                }
            }

            while (caves.Count > 1)
            {
                int cave1Index = _random.Next (0, caves.Count);
                int cave2Index = (cave1Index + _random.Next (1, caves.Count - 1)) % caves.Count;

                List<int> cave1 = caves[cave1Index];
                List<int> cave2 = caves[cave2Index];

                int startIndex = cave1[_random.Next (0, cave1.Count)];
                int endIndex = cave2[_random.Next (0, cave2.Count)];

                List<int> path = AstarPathFinding.FindPath (_nodes, _width, _height, startIndex, endIndex);

                if (path.Count > 0)
                {
                    foreach (int pathIndex in path)
                    {
                        _nodes[pathIndex] = true;
                    }
                }

                cave1.AddRange (cave2);
                caves.RemoveAt (cave2Index);

                yield return 0;
            }
        }

        private void Draw ()
        {
            for (int i = 0; i < _size; i++)
            {
                _canvas.SetPixel (i, _nodes[i] ? _aliveColor : _deadColor);
            }
        }

        public void Draw (RenderManager render)
        {
            _canvas.Draw (render);
        }

        private int GetIndex (int x, int y) => x + (y * _width);

        private (int, int) GetCoordinates (int index) => (index % _width, index / _width);

        public void Dispose ()
        {
            Dispose (true);
            GC.SuppressFinalize (this);
        }

        protected virtual void Dispose (bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _canvas.Dispose ();
                }

                _disposed = true;
            }
        }
    }
}
