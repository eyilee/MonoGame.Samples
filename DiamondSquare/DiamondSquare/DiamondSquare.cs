using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DiamondSquare
{
    public class DiamondSquare
    {
        private static readonly Random _random = new ();

        private Texture2D _texture;
        private int _tileSize = 10;

        private int _exponent;
        private int _size;
        private float _spread;
        private float _damping;

        private float[] _heightMap;
        private int _currentStepSize;
        private float _currentSpread;

        private int GetIndex (int x, int y) => x + (y * _size);

        private float GetHeight (int x, int y)
        {
            if (x < 0 || x >= _size || y < 0 || y >= _size)
            {
                return 0.0f;
            }

            return _heightMap[GetIndex (x, y)];
        }

        private void SetHeight (int x, int y, float height)
        {
            if (x < 0 || x >= _size || y < 0 || y >= _size)
            {
                return;
            }

            _heightMap[GetIndex (x, y)] = height;
        }

        public DiamondSquare (int exponent, float spread, float damping)
        {
            _texture = new Texture2D (Core.GraphicsDevice, 1, 1);
            _texture.SetData ([Color.White]);

            _exponent = Math.Max (exponent, 0);
            _size = (int)Math.Pow (2, _exponent) + 1;
            _spread = Math.Max (spread, 0.0f);
            _damping = Math.Clamp (damping, 0.0f, 1.0f);

            Reset ();
        }

        public void Reset ()
        {
            _heightMap = new float[_size * _size];
            Array.Fill (_heightMap, 0.0f);

            SetHeight (0, _size - 1, _random.NextSingle () * _spread);
            SetHeight (_size - 1, _size - 1, _random.NextSingle () * _spread);
            SetHeight (_size - 1, 0, _random.NextSingle () * _spread);
            SetHeight (0, 0, _random.NextSingle () * _spread);

            _currentStepSize = _size - 1;
            _currentSpread = _spread * _damping;
        }

        public void NextStep ()
        {
            if (_currentStepSize <= 1)
            {
                return;
            }

            int halfStep = _currentStepSize / 2;
            float halfSpread = _currentSpread / 2.0f;

            for (int x = 0; x < _size - 1; x += _currentStepSize)
            {
                for (int y = 0; y < _size - 1; y += _currentStepSize)
                {
                    float leftTop = GetHeight (x, y + _currentStepSize);
                    float rightTop = GetHeight (x + _currentStepSize, y + _currentStepSize);
                    float rightBottom = GetHeight (x + _currentStepSize, y);
                    float leftBottom = GetHeight (x, y);
                    float center = Math.Clamp ((leftTop + rightTop + rightBottom + leftBottom) / 4.0f + (_random.NextSingle () - 0.5f) * _currentSpread, 0.0f, 1.0f);

                    SetHeight (x + halfStep, y + halfStep, center);
                }
            }

            for (int x = 0; x < _size; x += halfStep)
            {
                bool isOdd = (x / halfStep) % 2 == 0;
                for (int y = isOdd ? halfStep : 0; y < _size; y += _currentStepSize)
                {
                    float left = GetHeight (x - halfStep, y);
                    float top = GetHeight (x, y + halfStep);
                    float right = GetHeight (x + halfStep, y);
                    float bottom = GetHeight (x, y - halfStep);

                    if (x == 0)
                    {
                        SetHeight (x, y, Math.Clamp ((top + right + bottom) / 3.0f + (_random.NextSingle () - 0.5f) * halfSpread, 0.0f, 1.0f));
                    }
                    else if (x == _size - 1)
                    {
                        SetHeight (x, y, Math.Clamp ((left + top + bottom) / 3.0f + (_random.NextSingle () - 0.5f) * halfSpread, 0.0f, 1.0f));
                    }
                    else if (y == 0)
                    {
                        SetHeight (x, y, Math.Clamp ((left + top + right) / 3.0f + (_random.NextSingle () - 0.5f) * halfSpread, 0.0f, 1.0f));
                    }
                    else if (y == _size - 1)
                    {
                        SetHeight (x, y, Math.Clamp ((left + right + bottom) / 3.0f + (_random.NextSingle () - 0.5f) * halfSpread, 0.0f, 1.0f));
                    }
                    else
                    {
                        SetHeight (x, y, Math.Clamp ((left + top + right + bottom) / 4.0f + (_random.NextSingle () - 0.5f) * halfSpread, 0.0f, 1.0f));
                    }
                }
            }

            _currentStepSize /= 2;
            _currentSpread *= _damping;
        }

        public void Draw (SpriteBatch spriteBatch)
        {
            int halfSize = (_size * _tileSize) / 2;

            for (int x = 0; x < _size; x++)
            {
                for (int y = 0; y < _size; y++)
                {
                    float height = GetHeight (x, y);
                    Color color = new (height, height, height, 1.0f);
                    spriteBatch.Draw (_texture, new Rectangle (x * _tileSize - halfSize, y * _tileSize - halfSize, _tileSize, _tileSize), color);
                }
            }
        }
    }
}
