using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Library;
using MonoGame.Library.Graphics;
using System;

namespace CellularAutomata
{
    public class CellularAutomata : IDisposable
    {
        private readonly Random _random = new ();

        private readonly Canvas _canvas;

        private readonly int _width;

        private readonly int _height;

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

        private Color _aliveColor = new (224, 224, 224, 255);

        private Color _deadColor = new (32, 32, 32, 255);

        private bool _disposed;

        public CellularAutomata (GraphicsDevice graphicsDevice, int width, int height, int cellSize, float aliveRate)
        {
            _canvas = new Canvas (graphicsDevice, "CellularAutomata", Core.ScreenWidth / 2, Core.ScreenHeight / 2, width, height, cellSize);
            _width = width;
            _height = height;
            _aliveRate = aliveRate;
            _nodes = new bool[width * height];
            _swapNodes = new bool[width * height];

            Reset ();
        }

        ~CellularAutomata () => Dispose (false);

        public void Reset ()
        {
            for (int i = 0; i < _nodes.Length; i++)
            {
                _nodes[i] = _random.NextDouble () < _aliveRate;
            }
        }

        public void NextStep ()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    int aliveNeighbors = 0;

                    foreach ((int xOffset, int yOffset) in _neighborOffsets)
                    {
                        if (x + xOffset < 0 || x + xOffset >= _width || y + yOffset < 0 || y + yOffset >= _height)
                        {
                            continue;
                        }

                        if (_nodes[GetIndex (x + xOffset, y + yOffset)])
                        {
                            aliveNeighbors++;
                        }
                    }

                    int index = GetIndex (x, y);

                    if (_nodes[index])
                    {
                        if (aliveNeighbors < 2)
                        {
                            _swapNodes[index] = false;
                        }
                        else if (aliveNeighbors > 3)
                        {
                            _swapNodes[index] = false;
                        }
                        else
                        {
                            _swapNodes[index] = true;
                        }
                    }
                    else
                    {
                        if (aliveNeighbors == 3)
                        {
                            _swapNodes[index] = true;
                        }
                        else
                        {
                            _swapNodes[index] = false;
                        }
                    }
                }
            }

            (_nodes, _swapNodes) = (_swapNodes, _nodes);

            Draw ();
        }

        private void Draw ()
        {
            for (int i = 0; i < _nodes.Length; i++)
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
