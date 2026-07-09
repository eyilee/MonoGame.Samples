using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Library;
using MonoGame.Library.Graphics;
using System;
using System.Collections.Generic;

namespace DiamondSquare
{
    public class DiamondSquare : IDisposable
    {
        private readonly Random _random = new ();

        private readonly Canvas _canvas;

        private readonly int _size;

        private readonly float _spread;

        private readonly float _damping;

        private readonly float[] _nodes;

        private IEnumerator<int>? _stepBehaviour;

        private bool _disposed;

        public DiamondSquare (GraphicsDevice graphicsDevice, int exponent, int cellSize, float spread, float damping)
        {
            int size = (int)Math.Pow (2, exponent) + 1;
            _canvas = new Canvas (graphicsDevice, "DiamondSquare", Core.ScreenWidth / 2, Core.ScreenHeight / 2, size, size, cellSize);
            _size = size;
            _spread = Math.Max (spread, 0f);
            _damping = Math.Clamp (damping, 0f, 1f);
            _nodes = new float[size * size];

            Reset ();
        }

        ~DiamondSquare () => Dispose (false);

        public void Reset ()
        {
            Array.Fill (_nodes, 0f);
            _nodes[GetIndex (0, 0)] = _random.NextSingle () * _spread;
            _nodes[GetIndex (0, _size - 1)] = _random.NextSingle () * _spread;
            _nodes[GetIndex (_size - 1, 0)] = _random.NextSingle () * _spread;
            _nodes[GetIndex (_size - 1, _size - 1)] = _random.NextSingle () * _spread;
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
            int step = _size - 1;
            float spread = _spread;

            while (step > 1)
            {
                int halfStep = step / 2;
                float halfSpread = spread / 2f;

                for (int x = 0; x < _size - 1; x += step)
                {
                    for (int y = 0; y < _size - 1; y += step)
                    {
                        float leftTop = _nodes[GetIndex (x, y)];
                        float rightTop = _nodes[GetIndex (x + step, y)];
                        float leftBottom = _nodes[GetIndex (x, y + step)];
                        float rightBottom = _nodes[GetIndex (x + step, y + step)];
                        float mean = (leftTop + rightTop + leftBottom + rightBottom) / 4f;
                        float offset = (_random.NextSingle () - 0.5f) * spread;
                        _nodes[GetIndex (x + halfStep, y + halfStep)] = Math.Clamp (mean + offset, 0f, 1f);
                    }
                }

                for (int x = 0; x < _size; x += halfStep)
                {
                    bool isOdd = (x / halfStep) % 2 == 0;

                    for (int y = isOdd ? halfStep : 0; y < _size; y += step)
                    {
                        float left = x != 0 ? _nodes[GetIndex (x - halfStep, y)] : 0f;
                        float top = y != 0 ? _nodes[GetIndex (x, y - halfStep)] : 0f;
                        float right = x != _size - 1 ? _nodes[GetIndex (x + halfStep, y)] : 0f;
                        float bottom = y != _size - 1 ? _nodes[GetIndex (x, y + halfStep)] : 0f;

                        float mean;

                        if (x == 0)
                        {
                            mean = (top + right + bottom) / 3f;
                        }
                        else if (x == _size - 1)
                        {
                            mean = (left + top + bottom) / 3f;
                        }
                        else if (y == 0)
                        {
                            mean = (left + right + bottom) / 3f;
                        }
                        else if (y == _size - 1)
                        {
                            mean = (left + top + right) / 3f;
                        }
                        else
                        {
                            mean = (left + top + right + bottom) / 4f;
                        }

                        float offset = (_random.NextSingle () - 0.5f) * halfSpread;
                        _nodes[GetIndex (x, y)] = Math.Clamp (mean + offset, 0f, 1f);
                    }
                }

                step /= 2;
                spread *= _damping;

                yield return 0;
            }
        }

        private void Draw ()
        {
            for (int i = 0; i < _nodes.Length; i++)
            {
                float value = _nodes[i];
                Color color = new (value, value, value, 1f);
                _canvas.SetPixel (i, color);
            }
        }

        public void Draw (RenderManager render)
        {
            _canvas.Draw (render);
        }

        private int GetIndex (int x, int y) => x + (y * _size);

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
