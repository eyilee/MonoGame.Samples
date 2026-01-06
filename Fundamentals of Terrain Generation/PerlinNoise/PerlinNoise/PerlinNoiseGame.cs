using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Samples.Library;
using MonoGame.Samples.Library.Canvas;
using MonoGame.Samples.Library.Input;
using System;

namespace MonoGame.Samples.PerlinNoise
{
    public class PerlinNoiseGame : Core
    {
        private Texture2D? _texture;
        private Canvas? _canvas;

        private float _frequency = 0.05f;
        private readonly float _frequencyStep = 0.005f;
        private bool _fractal = false;

        public PerlinNoiseGame ()
            : base ("PerlinNoise", 512, 512, false)
        {
            IsMouseVisible = true;
        }

        protected override void Initialize ()
        {
            base.Initialize ();
        }

        protected override void LoadContent ()
        {
            _texture = new Texture2D (GraphicsDevice, 1, 1);
            _texture.SetData ([Color.White]);

            int pixelSize = 4;
            _canvas = new Canvas (_texture, ScreenWidth / pixelSize, ScreenHeight / pixelSize, pixelSize);
            _canvas.SetOffset ((ScreenWidth - _canvas.PixelWidth) / 2, (ScreenHeight - _canvas.PixelHeight) / 2);

            GeneratePerlinNoise ();

            Input.SubscribeKeyPressed (Keys.Add, (sender, eventArgs) => { _frequency += _frequencyStep; GeneratePerlinNoise (); });
            Input.SubscribeKeyPressed (Keys.Subtract, (sender, eventArgs) => { _frequency -= _frequencyStep; GeneratePerlinNoise (); });
            Input.SubscribeKeyPressed (Keys.F, (sender, eventArgs) => { _fractal = !_fractal; GeneratePerlinNoise (); });
        }

        private void GeneratePerlinNoise ()
        {
            if (_canvas == null)
            {
                throw new InvalidOperationException ("Canvas has not been initialized.");
            }

            for (int x = 0; x < _canvas.Width; x++)
            {
                for (int y = 0; y < _canvas.Height; y++)
                {
                    float value = _fractal
                        ? PerlinNoise.FractalBrownianMotionNoise (x * _frequency, y * _frequency, 6)
                        : PerlinNoise.Noise (x * _frequency, y * _frequency);
                    _canvas.SetPixel (x, y, new Color (value, value, value, 1));
                }
            }
        }

        protected override void Update (GameTime gameTime)
        {
            if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState ().IsKeyDown (Keys.Escape))
            {
                Exit ();
            }

            Input.Update (gameTime);

            base.Update (gameTime);
        }

        protected override void Draw (GameTime gameTime)
        {
            GraphicsDevice.Clear (Color.CornflowerBlue);

            SpriteBatch.Begin ();
            _canvas?.Draw (SpriteBatch);
            SpriteBatch.End ();

            base.Draw (gameTime);
        }
    }
}
