using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Library;
using MonoGame.Library.Graphics;
using System;
using System.Collections.Generic;

namespace CellularAutomata
{
    public class CellularAutomata : IDisposable
    {
        private readonly Random _random = new ();

        private readonly Canvas _canvas;

        private readonly int _width;

        private readonly int _height;

        private readonly int _size;

        private readonly bool[] _originNodes;

        private bool[] _nodes;

        private bool[] _swapNodes;

        private readonly (int, int)[] _neighborOffsets =
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

        private readonly float _aliveRate;

        private IEnumerator<int>? _stepBehaviour;

        private Color _aliveColor = new (224, 224, 224, 255);

        private Color _deadColor = new (32, 32, 32, 255);

        private bool _disposed;

        public CellularAutomata (GraphicsDevice graphicsDevice, int width, int height, int cellSize, float aliveRate)
        {
            _canvas = new Canvas (graphicsDevice, "CellularAutomata", Core.ScreenWidth / 2, Core.ScreenHeight / 2, width, height, cellSize);
            _width = width;
            _height = height;
            _size = width * height;
            _aliveRate = aliveRate;
            _originNodes = new bool[_size];
            _nodes = new bool[_size];
            _swapNodes = new bool[_size];

            Reset ();
        }

        ~CellularAutomata () => Dispose (false);

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
            while (true)
            {
                for (int x = 0; x < _width; x++)
                {
                    for (int y = 0; y < _height; y++)
                    {
                        int aliveNeighbors = 0;

                        foreach ((int xOffset, int yOffset) in _neighborOffsets)
                        {
                            int neighborX = x + xOffset;
                            int neighborY = y + yOffset;

                            if (neighborX < 0 || neighborX >= _width || neighborY < 0 || neighborY >= _height)
                            {
                                continue;
                            }

                            if (_nodes[GetIndex (neighborX, neighborY)])
                            {
                                aliveNeighbors++;
                            }
                        }

                        int index = GetIndex (x, y);

                        if (_nodes[index])
                        {
                            _swapNodes[index] = aliveNeighbors >= 2 && aliveNeighbors <= 3;
                        }
                        else
                        {
                            _swapNodes[index] = aliveNeighbors == 3;
                        }
                    }
                }

                (_nodes, _swapNodes) = (_swapNodes, _nodes);

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
