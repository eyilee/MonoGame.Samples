using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Samples.Library;
using MonoGame.Samples.Library.Canvas;
using MonoGame.Samples.Library.Input;
using System;

namespace MonoGame.Samples.PerlinNoiseHash;

public class GameScene : Scene
{
    private const int PixelSize = 2;
    private Canvas? _canvas;

    private int _seed;
    private PerlinNoise? _perlinNoise;
    private float _frequency = 0.03f;
    private readonly float _frequencyStep = 0.003f;

    public override void Initialize ()
    {
        Input.Keyboard.SubscribePressed (Keys.N, NextMap);
        Input.Keyboard.SubscribePressed (Keys.Add, IncreaseFrequency);
        Input.Keyboard.SubscribePressed (Keys.Subtract, DecreaseFrequency);
        Input.Mouse.SubscribeWheelMoved (ChangeFrequency);

        base.Initialize ();
    }

    public override void LoadContent ()
    {
        _canvas = new Canvas (GraphicsDevice, Core.ScreenWidth / PixelSize, Core.ScreenHeight / PixelSize, PixelSize);
        _canvas.SetOffset ((Core.ScreenWidth - _canvas.PixelWidth) / 2, (Core.ScreenHeight - _canvas.PixelHeight) / 2);

        SetupGenerator ();
        GeneratePerlinNoiseHash ();

        base.LoadContent ();
    }

    public override void UnloadContent ()
    {
        _canvas?.Dispose ();
        _canvas = null;

        base.UnloadContent ();
    }

    protected override void Dispose (bool disposing)
    {
        if (disposing)
        {
            Input.Keyboard.UnsubscribePressed (Keys.N, NextMap);
            Input.Keyboard.UnsubscribePressed (Keys.Add, IncreaseFrequency);
            Input.Keyboard.UnsubscribePressed (Keys.Subtract, DecreaseFrequency);
            Input.Mouse.UnsubscribeWheelMoved (ChangeFrequency);
        }

        base.Dispose (disposing);
    }

    public override void Draw (GameTime gameTime)
    {
        GraphicsDevice.Clear (Color.CornflowerBlue);

        SpriteBatch.Begin ();
        _canvas?.Draw (SpriteBatch);
        SpriteBatch.End ();

        base.Draw (gameTime);
    }

    private void SetupGenerator ()
    {
        _seed = Environment.TickCount;
        _perlinNoise = new PerlinNoise (_seed);
    }

    private void GeneratePerlinNoiseHash ()
    {
        if (_canvas is null)
        {
            throw new InvalidOperationException ("Canvas has not been initialized.");
        }

        if (_perlinNoise is null)
        {
            throw new InvalidOperationException ("PerlinNoise has not been initialized.");
        }

        for (int x = 0; x < _canvas.Width; x++)
        {
            for (int y = 0; y < _canvas.Height; y++)
            {
                float value = 0f;
                float maxValue = 0f;

                float frequency = _frequency;
                float amplitude = 1f;

                for (int i = 0; i < 1; i++)
                {
                    value += _perlinNoise.Noise (x * frequency, y * frequency, _seed + i) * amplitude;
                    maxValue += amplitude;

                    frequency *= 2f;
                    amplitude *= 0.5f;
                }

                _canvas.SetPixel (x, y, Color.Lerp (Color.Black, Color.White, value / maxValue));
            }
        }
    }

    private void NextMap (object? sender, KeyboardEventArgs eventArgs)
    {
        SetupGenerator ();
        GeneratePerlinNoiseHash ();
    }

    private void IncreaseFrequency (object? sender, KeyboardEventArgs eventArgs)
    {
        _frequency += _frequencyStep;
        GeneratePerlinNoiseHash ();
    }

    private void DecreaseFrequency (object? sender, KeyboardEventArgs eventArgs)
    {
        _frequency -= _frequencyStep;
        GeneratePerlinNoiseHash ();
    }

    private void ChangeFrequency (object? sender, MouseEventArgs eventArgs)
    {
        _frequency += _frequencyStep * float.Sign (eventArgs.ScrollWheelDelta) * float.Ceiling (float.Abs (eventArgs.ScrollWheelDelta) * 0.01f);
        GeneratePerlinNoiseHash ();
    }
}
