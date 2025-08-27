using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MidpointDisplacement
{
    public class MidpointDisplacement
    {
        private static readonly Random _random = new ();

        private readonly Texture2D _texture;
        private readonly int _tileSize = 10;

        private readonly int _exponent;
        private readonly int _size;
        private readonly float _spread;
        private readonly float _damping;

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

        public MidpointDisplacement (int exponent, float spread, float damping)
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

            for (int x = 0; x < _size - 1; x += _currentStepSize)
            {
                for (int y = 0; y < _size - 1; y += _currentStepSize)
                {
                    float leftTop = GetHeight (x, y + _currentStepSize);
                    float rightTop = GetHeight (x + _currentStepSize, y + _currentStepSize);
                    float rightBottom = GetHeight (x + _currentStepSize, y);
                    float leftBottom = GetHeight (x, y);

                    float centerTop = Math.Clamp (((leftTop + rightTop) / 2.0f) + (_currentSpread * (_random.NextSingle () - 0.5f)), 0.0f, 1.0f);
                    float rightCenter = Math.Clamp (((rightTop + rightBottom) / 2.0f) + (_currentSpread * (_random.NextSingle () - 0.5f)), 0.0f, 1.0f);
                    float centerBottom = Math.Clamp (((leftBottom + rightBottom) / 2.0f) + (_currentSpread * (_random.NextSingle () - 0.5f)), 0.0f, 1.0f);
                    float leftCenter = Math.Clamp (((leftTop + leftBottom) / 2.0f) + (_currentSpread * (_random.NextSingle () - 0.5f)), 0.0f, 1.0f);
                    float center = Math.Clamp (((centerTop + rightCenter + centerBottom + leftCenter) / 4.0f) + (_currentSpread * (_random.NextSingle () - 0.5f)), 0.0f, 1.0f);

                    SetHeight (x + halfStep, y + _currentStepSize, centerTop);
                    SetHeight (x + _currentStepSize, y + halfStep, rightCenter);
                    SetHeight (x + halfStep, y, centerBottom);
                    SetHeight (x, y + halfStep, leftCenter);
                    SetHeight (x + halfStep, y + halfStep, center);
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
                    spriteBatch.Draw (_texture, new Rectangle ((x * _tileSize) - halfSize, halfSize - (y * _tileSize) - _tileSize, _tileSize, _tileSize), color);
                }
            }
        }
    }
}
