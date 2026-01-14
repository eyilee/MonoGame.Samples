using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Samples.Library;
using MonoGame.Samples.Library.Canvas;
using MonoGame.Samples.Library.Input;
using System;

namespace MonoGame.Samples.PerlinNoise;

public class GameScene : Scene
{
    private const int PixelSize = 4;

    private Canvas? _canvas;

    private float _frequency = 0.05f;
    private readonly float _frequencyStep = 0.005f;
    private bool _fractal = false;

    public override void LoadContent ()
    {
        _canvas = new Canvas (GraphicsDevice, Core.ScreenWidth / PixelSize, Core.ScreenHeight / PixelSize, PixelSize);
        _canvas.SetOffset ((Core.ScreenWidth - _canvas.PixelWidth) / 2, (Core.ScreenHeight - _canvas.PixelHeight) / 2);

        GeneratePerlinNoise ();

        Input.Keyboard.SubscribePressed (Keys.Add, IncreaseFrequency);
        Input.Keyboard.SubscribePressed (Keys.Subtract, DecreaseFrequency);
        Input.Keyboard.SubscribePressed (Keys.F, ToggleFractal);

        base.LoadContent ();
    }

    public override void UnloadContent ()
    {
        Input.Keyboard.UnsubscribePressed (Keys.Add, IncreaseFrequency);
        Input.Keyboard.UnsubscribePressed (Keys.Subtract, DecreaseFrequency);
        Input.Keyboard.UnsubscribePressed (Keys.F, ToggleFractal);

        base.UnloadContent ();
    }

    public override void Draw (GameTime gameTime)
    {
        GraphicsDevice.Clear (Color.CornflowerBlue);

        SpriteBatch.Begin ();
        _canvas?.Draw (SpriteBatch);
        SpriteBatch.End ();

        base.Draw (gameTime);
    }

    private void GeneratePerlinNoise ()
    {
        if (_canvas is null)
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

    private void IncreaseFrequency (object? sender, KeyboardEventArgs eventArgs)
    {
        _frequency += _frequencyStep;
        GeneratePerlinNoise ();
    }

    private void DecreaseFrequency (object? sender, KeyboardEventArgs eventArgs)
    {
        _frequency -= _frequencyStep;
        GeneratePerlinNoise ();
    }

    private void ToggleFractal (object? sender, KeyboardEventArgs eventArgs)
    {
        _fractal = !_fractal;
        GeneratePerlinNoise ();
    }
}
