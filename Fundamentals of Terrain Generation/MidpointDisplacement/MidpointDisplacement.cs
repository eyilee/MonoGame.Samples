using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Library;
using MonoGame.Library.Graphics;
using System;
using System.Collections.Generic;

namespace MidpointDisplacement
{
    public class MidpointDisplacement : IDisposable
    {
        private readonly Random _random = new ();

        private readonly Canvas _canvas;

        private readonly int _size;

        private readonly float _spread;

        private readonly float _damping;

        private readonly float[] _nodes;

        private IEnumerator<int>? _stepBehaviour;

        private bool _disposed;

        public MidpointDisplacement (GraphicsDevice graphicsDevice, int exponent, int cellSize, float spread, float damping)
        {
            int size = (int)Math.Pow (2, exponent) + 1;
            _canvas = new Canvas (graphicsDevice, "MidpointDisplacement", Core.ScreenWidth / 2, Core.ScreenHeight / 2, size, size, cellSize);
            _size = size;
            _spread = Math.Max (spread, 0f);
            _damping = Math.Clamp (damping, 0f, 1f);
            _nodes = new float[size * size];

            Reset ();
        }

        ~MidpointDisplacement () => Dispose (false);

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

                for (int x = 0; x < _size - 1; x += step)
                {
                    for (int y = 0; y < _size - 1; y += step)
                    {
                        float leftTop = _nodes[GetIndex (x, y)];
                        float rightTop = _nodes[GetIndex (x + step, y)];
                        float leftBottom = _nodes[GetIndex (x, y + step)];
                        float rightBottom = _nodes[GetIndex (x + step, y + step)];

                        float centerTop = Math.Clamp (((leftTop + rightTop) / 2f) + (spread * (_random.NextSingle () - 0.5f)), 0f, 1f);
                        float rightCenter = Math.Clamp (((rightTop + rightBottom) / 2f) + (spread * (_random.NextSingle () - 0.5f)), 0f, 1f);
                        float centerBottom = Math.Clamp (((leftBottom + rightBottom) / 2f) + (spread * (_random.NextSingle () - 0.5f)), 0f, 1f);
                        float leftCenter = Math.Clamp (((leftTop + leftBottom) / 2f) + (spread * (_random.NextSingle () - 0.5f)), 0f, 1f);
                        float center = Math.Clamp (((centerTop + rightCenter + centerBottom + leftCenter) / 4f) + (spread * (_random.NextSingle () - 0.5f)), 0f, 1f);

                        _nodes[GetIndex (x + halfStep, y)] = centerTop;
                        _nodes[GetIndex (x + step, y + halfStep)] = rightCenter;
                        _nodes[GetIndex (x + halfStep, y + step)] = centerBottom;
                        _nodes[GetIndex (x, y + halfStep)] = leftCenter;
                        _nodes[GetIndex (x + halfStep, y + halfStep)] = center;
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
