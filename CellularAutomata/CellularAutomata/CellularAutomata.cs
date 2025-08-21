using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CellularAutomata
{
    public class CellularAutomata
    {
        private static readonly Random _random = new ();

        private Texture2D _texture;
        private int _cellSize = 4;

        private int _size;
        private float _aliveRate;

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

        private int GetIndex (int x, int y)
        {
            x = ((x % _size) + _size) % _size;
            y = ((y % _size) + _size) % _size;
            return x + (y * _size);
        }

        public CellularAutomata (int size, float aliveRate)
        {
            _texture = new Texture2D (Core.GraphicsDevice, 1, 1);
            _texture.SetData ([Color.White]);

            if (size <= 0)
            {
                throw new ArgumentException ("Size must be greater than 0", nameof (size));
            }

            _size = size;
            _aliveRate = aliveRate;

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

            for (int x = 0; x < _size; x++)
            {
                for (int y = 0; y < _size; y++)
                {
                    _cellMap[_currentIndex][GetIndex (x, y)] = _random.NextDouble () < _aliveRate;
                }
            }
        }

        public void NextStep ()
        {
            int nextIndex = (_currentIndex + 1) % _cellMap.Length;

            for (int x = 0; x < _size; x++)
            {
                for (int y = 0; y < _size; y++)
                {
                    int aliveNeighbors = 0;
                    foreach ((int xOffset, int yOffset) in _neighborOffsets)
                    {
                        aliveNeighbors += _cellMap[_currentIndex][GetIndex (x + xOffset, y + yOffset)] ? 1 : 0;
                    }

                    bool isAlive = _cellMap[_currentIndex][GetIndex (x, y)];
                    if (isAlive)
                    {
                        if (aliveNeighbors < 2)
                        {
                            _cellMap[nextIndex][GetIndex (x, y)] = false;
                        }
                        else if (aliveNeighbors > 3)
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
                        if (aliveNeighbors == 3)
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

        public void Draw (SpriteBatch spriteBatch)
        {
            int halfSize = _size * _cellSize / 2;
            Color aliveColor = new (224, 224, 224, 255);
            Color deadColor = new (32, 32, 32, 255);

            for (int x = 0; x < _size; x++)
            {
                for (int y = 0; y < _size; y++)
                {
                    bool isAlive = _cellMap[_currentIndex][GetIndex (x, y)];
                    spriteBatch.Draw (_texture, new Rectangle ((x * _cellSize) - halfSize, halfSize - (y * _cellSize), _cellSize, _cellSize), isAlive ? aliveColor : deadColor);
                }
            }
        }
    }
}
