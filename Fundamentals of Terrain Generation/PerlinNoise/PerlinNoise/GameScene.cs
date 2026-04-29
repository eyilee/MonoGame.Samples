using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Samples.Library;
using MonoGame.Samples.Library.Canvas;
using MonoGame.Samples.Library.GumUI;
using MonoGame.Samples.Library.Input;
using System;

namespace MonoGame.Samples.PerlinNoise;

public class GameScene : Scene
{
    public float MinFrequency { get; } = 0.001f;
    public float MaxFrequency { get; } = 0.099f;
    public float Frequency { get; private set; } = 0.05f;
    public float FrequencyStep { get; } = 0.001f;
    public bool IsDomainWarpEnabled { get; private set; }
    public bool IsFractalBrownianMotionEnabled { get; private set; }

    private GameUI? _gameUI;

    private const int PixelSize = 2;
    private Canvas? _canvas;

    private int _seed;
    private PerlinNoise? _perlinNoise;

    public override void Initialize ()
    {
        _gameUI = GumUI.Instantiate (new GameUI (this));

        Input.Keyboard.SubscribePressed (Keys.N, NextMap);
        Input.Keyboard.SubscribePressed (Keys.Add, IncreaseFrequency);
        Input.Keyboard.SubscribePressed (Keys.Subtract, DecreaseFrequency);
        Input.Keyboard.SubscribePressed (Keys.D, ToggleDomainWarp);
        Input.Keyboard.SubscribePressed (Keys.F, ToggleFractal);
        Input.Mouse.SubscribeWheelMoved (ChangeFrequency);

        base.Initialize ();
    }

    public override void LoadContent ()
    {
        _canvas = new Canvas (GraphicsDevice, Core.ScreenWidth / PixelSize, Core.ScreenHeight / PixelSize, PixelSize);
        _canvas.SetOffset ((Core.ScreenWidth - _canvas.PixelWidth) / 2, (Core.ScreenHeight - _canvas.PixelHeight) / 2);

        SetupGenerator ();
        GeneratePerlinNoise ();

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
            _gameUI?.Detach ();
            _gameUI = null;

            Input.Keyboard.UnsubscribePressed (Keys.N, NextMap);
            Input.Keyboard.UnsubscribePressed (Keys.Add, IncreaseFrequency);
            Input.Keyboard.UnsubscribePressed (Keys.Subtract, DecreaseFrequency);
            Input.Keyboard.UnsubscribePressed (Keys.F, ToggleFractal);
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

    private void GeneratePerlinNoise ()
    {
        if (_canvas is null)
        {
            throw new InvalidOperationException ("Canvas has not been initialized.");
        }

        if (_perlinNoise is null)
        {
            throw new InvalidOperationException ("PerlinNoise has not been initialized.");
        }

        float warpFrequency = 0.1f * Frequency;
        float warpAmplitude = 0.2f * (1 / Frequency);

        for (int x = 0; x < _canvas.Width; x++)
        {
            for (int y = 0; y < _canvas.Height; y++)
            {
                float baseX = x * Frequency;
                float baseY = y * Frequency;

                if (IsDomainWarpEnabled)
                {
                    float warpX = x * warpFrequency;
                    float warpY = y * warpFrequency;

                    float warpedX = _perlinNoise.FractalBrownianMotionNoise (warpX + 211f, warpY + 163f, 4, 2f, 0.5f);
                    float warpedY = _perlinNoise.FractalBrownianMotionNoise (warpX + 127f, warpY + 107f, 4, 2f, 0.5f);

                    baseX += warpedX * warpAmplitude;
                    baseY += warpedY * warpAmplitude;
                }

                float value = IsFractalBrownianMotionEnabled
                    ? _perlinNoise.FractalBrownianMotionNoise (baseX, baseY, 4, 2f, 0.5f)
                    : _perlinNoise.Noise (baseX, baseY);

                _canvas.SetPixel (x, y, Color.Lerp (Color.Black, Color.White, value));
            }
        }
    }

    public void SetFrequency (float frequency)
    {
        frequency = float.Clamp (frequency, MinFrequency, MaxFrequency);

        if (Frequency != frequency)
        {
            Frequency = frequency;
            GeneratePerlinNoise ();

            _gameUI?.SetFrequency (Frequency);
        }
    }

    public void EnableDomainWarp (bool enable)
    {
        if (IsDomainWarpEnabled != enable)
        {
            IsDomainWarpEnabled = enable;
            GeneratePerlinNoise ();

            _gameUI?.EnableDomainWarp (IsDomainWarpEnabled);
        }
    }

    public void EnableFractalBrownianMotion (bool enable)
    {
        if (IsFractalBrownianMotionEnabled != enable)
        {
            IsFractalBrownianMotionEnabled = enable;
            GeneratePerlinNoise ();

            _gameUI?.EnableFractalBrownianMotion (IsFractalBrownianMotionEnabled);
        }
    }

    private void NextMap (object? sender, KeyboardEventArgs eventArgs)
    {
        SetupGenerator ();
        GeneratePerlinNoise ();
    }

    private void IncreaseFrequency (object? sender, KeyboardEventArgs eventArgs) => SetFrequency (Frequency + FrequencyStep);

    private void DecreaseFrequency (object? sender, KeyboardEventArgs eventArgs) => SetFrequency (Frequency - FrequencyStep);

    private void ToggleDomainWarp (object? sender, KeyboardEventArgs eventArgs) => EnableDomainWarp (!IsDomainWarpEnabled);

    private void ToggleFractal (object? sender, KeyboardEventArgs eventArgs) => EnableFractalBrownianMotion (!IsFractalBrownianMotionEnabled);

    private void ChangeFrequency (object? sender, MouseEventArgs eventArgs) => SetFrequency (Frequency + FrequencyStep * float.Sign (eventArgs.ScrollWheelDelta) * float.Ceiling (float.Abs (eventArgs.ScrollWheelDelta) * 0.01f));
}
