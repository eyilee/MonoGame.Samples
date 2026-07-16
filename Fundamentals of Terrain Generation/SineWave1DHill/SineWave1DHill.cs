using Microsoft.Xna.Framework;
using MonoGame.Library;
using MonoGame.Library.Graphics;
using System;
using System.Collections.Generic;

namespace SineWave1DHill
{
    public class SineWave1DHill
    {
        private readonly Random _random = new ();

        private readonly Sprite _spriteTemplate;

        private readonly int _size;

        private readonly float[] _randomNodes;

        private readonly Node[] _nodes;

        private readonly int _width;

        private readonly int _height;

        private readonly int _iteration;

        private IEnumerator<int>? _stepBehaviour;

        public SineWave1DHill (int size, int width, int height, int iteration)
        {
            if (Texture2DResource.TryGetValue ("Pixel", out Texture2DResource? texture) && texture != null)
            {
                _spriteTemplate = new Sprite (new TextureRegion (texture, 0, 0, 1, 1));
            }
            else
            {
                throw new InvalidOperationException ("Pixel resource not found");
            }

            _size = size;
            _width = width;
            _height = height;
            _iteration = iteration;
            _randomNodes = new float[size];
            _nodes = new Node[size];

            for (int i = 0; i < _size; i++)
            {
                Sprite sprite = _spriteTemplate.Clone ();
                sprite.Color = Color.Black;

                _nodes[i] = new Node ()
                {
                    Value = 0f,
                    Sprite = sprite
                };
            }

            Reset ();
        }

        public void Reset ()
        {
            for (int i = 0; i < _size; i++)
            {
                _randomNodes[i] = _random.NextSingle ();
                _nodes[i].Value = 0f;
            }

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
            int step = 1;
            float scale = 1f / float.Pow (2f, _iteration);

            for (int iteration = 0; iteration < _iteration; iteration++)
            {
                float[] sampleNodes = new float[_size];

                for (int i = 0; i < _size; i += step)
                {
                    sampleNodes[i] = _randomNodes[i] * scale;
                }

                for (int beginIndex = 0; beginIndex < _size; beginIndex += step)
                {
                    float value1 = sampleNodes[beginIndex];

                    int endIndex = int.Min (beginIndex + step, _size - 1);
                    float value2 = sampleNodes[endIndex];

                    int subStep = endIndex - beginIndex;
                    for (int j = 1; j < subStep; j++)
                    {
                        float amount = (float)j / subStep;
                        sampleNodes[beginIndex + j] = SineInterpolation (value1, value2, amount);
                    }
                }

                for (int i = 0; i < _size; i++)
                {
                    _nodes[i].Value += sampleNodes[i];
                }

                step *= 2;
                scale *= 2f;

                yield return 0;
            }
        }

        private static float SineInterpolation (float value1, float value2, float amount)
        {
            float f = (1f - MathF.Sin ((amount * float.Pi) + (float.Pi / 2f))) / 2f;
            return (value1 * (1f - f)) + (value2 * f);
        }

        private void Draw ()
        {
            Vector2 offset = new ((Core.ScreenWidth - (_size * _width)) / 2, (Core.ScreenHeight + _height) / 2);

            for (int i = 0; i < _size; i++)
            {
                Node node = _nodes[i];
                float height = node.Value * _height;
                node.Size = new Vector2 (_width, height);
                node.Position = offset + new Vector2 (i * _width, 0f);
                node.Origin = new Vector2 (0, height);
            }
        }

        public void Draw (RenderManager render)
        {
            foreach (Node node in _nodes)
            {
                node.Sprite.Draw (render);
            }
        }
    }
}
