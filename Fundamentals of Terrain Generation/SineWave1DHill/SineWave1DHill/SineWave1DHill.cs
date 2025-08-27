using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SineWave1DHill
{
    public class SineWave1DHill
    {
        private static readonly Random _random = new ();

        private readonly Texture2D _texture;
        private readonly int _unitWidth = 1;
        private readonly int _unitHeight = 200;

        private readonly int _size;
        private readonly int _iteration;

        private float[] _heightMap;
        private float[] _randomHeightMap;
        private int _currentIteration;

        public SineWave1DHill (int size, int iteration)
        {
            _texture = new Texture2D (Core.GraphicsDevice, 1, 1);
            _texture.SetData ([Color.White]);

            if (size <= 0)
            {
                throw new ArgumentException ("Size must be greater than 0", nameof (size));
            }

            if (size <= (int)Math.Pow (2, iteration))
            {
                throw new ArgumentException ("Size must be greater than 2^iteration", nameof (size));
            }

            _size = size;
            _iteration = iteration;

            Reset ();
        }

        public void Reset ()
        {
            _heightMap = new float[_size];
            Array.Fill (_heightMap, 0.0f);

            _randomHeightMap = new float[_size];
            for (int i = 0; i < _randomHeightMap.Length; i++)
            {
                _randomHeightMap[i] = _random.NextSingle ();
            }

            _currentIteration = 0;
        }

        public void NextStep ()
        {
            if (_currentIteration >= _iteration)
            {
                return;
            }

            int step = (int)MathF.Pow (2, _currentIteration);
            float scaleFactor = 1.0f / (float)Math.Pow (2, _iteration - _currentIteration);

            float[] _sampleHeightMap = new float[_size];
            Array.Fill (_sampleHeightMap, 0.0f);

            for (int i = 0; i < _size; i += step)
            {
                _sampleHeightMap[i] = _randomHeightMap[i] * scaleFactor;
            }

            for (int beginIndex = 0; beginIndex < _size; beginIndex += step)
            {
                float value1 = _sampleHeightMap[beginIndex];

                int endIndex = Math.Min (beginIndex + step, _size - 1);
                float value2 = _sampleHeightMap[endIndex];

                int subStep = endIndex - beginIndex;
                for (int j = 1; j < subStep; j++)
                {
                    float amount = (float)j / subStep;
                    _sampleHeightMap[beginIndex + j] = SineInterpolation (value1, value2, amount);
                }
            }

            for (int i = 0; i < _size; i++)
            {
                _heightMap[i] += _sampleHeightMap[i];
            }

            _currentIteration++;
        }

        private static float SineInterpolation (float value1, float value2, float amount)
        {
            float f = (1.0f - MathF.Sin ((amount * MathF.PI) + (MathF.PI / 2.0f))) / 2.0f;
            return (value1 * (1.0f - f)) + (value2 * f);
        }

        public void Draw (SpriteBatch spriteBatch)
        {
            int halfWidth = _size * _unitWidth / 2;
            int halfHeight = _unitHeight / 2;
            Color color = new (32, 32, 32, 255);

            for (int x = 0; x < _size; x++)
            {
                int height = (int)(_heightMap[x] * _unitHeight);
                spriteBatch.Draw (_texture, new Rectangle ((x * _unitWidth) - halfWidth, halfHeight - height, _unitWidth, height), color);
            }
        }
    }
}
