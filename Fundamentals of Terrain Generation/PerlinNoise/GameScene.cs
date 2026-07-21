using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Library;
using MonoGame.Library.Graphics;
using MonoGame.Library.Input;
using System;

namespace PerlinNoise;

public class GameScene : Scene
{
    private Canvas _canvas = null!;

    private PerlinNoise _perlinNoise = new (Environment.TickCount);

    private readonly float _minFrequency = 0.001f;

    private readonly float _maxFrequency = 0.1f;

    private readonly float _frequencyStep = 0.01f;

    private float _frequency = 0.03f;

    private bool _domainWarpEnabled;

    private bool _fractalBrownianMotionEnabled;

    private readonly float _updateTime = 1f / 4f;

    private float _nextUpdateTime = 0f;

    public override void Initialize ()
    {
        Input.Keyboard.SubscribePressed (Keys.N, NextMap);
        Input.Keyboard.SubscribePressed (Keys.D, ToggleDomainWarp);
        Input.Keyboard.SubscribePressed (Keys.F, ToggleFractal);

        base.Initialize ();
    }

    public override void LoadContent ()
    {
        _canvas = new Canvas (GraphicsDevice, "PerlinNoise", Core.ScreenWidth / 2, Core.ScreenHeight / 2, 256, 256, 2);

        GeneratePerlinNoise ();

        base.LoadContent ();
    }

    public override void UnloadContent ()
    {
        _canvas.Dispose ();

        base.UnloadContent ();
    }

    protected override void Dispose (bool disposing)
    {
        if (disposing)
        {
            Input.Keyboard.UnsubscribePressed (Keys.N, NextMap);
            Input.Keyboard.UnsubscribePressed (Keys.D, ToggleDomainWarp);
            Input.Keyboard.UnsubscribePressed (Keys.F, ToggleFractal);
        }

        base.Dispose (disposing);
    }

    public override void Update (GameTime gameTime)
    {
        float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (Input.Keyboard.IsKeyDown (Keys.Add))
        {
            if (_nextUpdateTime <= 0f)
            {
                _nextUpdateTime = _updateTime;
            }

            SetFrequency (_frequency + (elapsedTime * _frequencyStep));
        }

        if (Input.Keyboard.IsKeyDown (Keys.Subtract))
        {
            if (_nextUpdateTime <= 0f)
            {
                _nextUpdateTime = _updateTime;
            }

            SetFrequency (_frequency - (elapsedTime * _frequencyStep));
        }

        if (_nextUpdateTime > 0f)
        {
            _nextUpdateTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_nextUpdateTime <= 0f)
            {
                GeneratePerlinNoise ();
            }
        }

        base.Update (gameTime);
    }

    public override void Draw (GameTime gameTime)
    {
        GraphicsDevice.Clear (Color.CornflowerBlue);

        _canvas.Draw (Render);

        base.Draw (gameTime);
    }

    private void GeneratePerlinNoise ()
    {
        if (_canvas == null)
        {
            throw new InvalidOperationException ("Canvas has not been initialized.");
        }

        if (_perlinNoise == null)
        {
            throw new InvalidOperationException ("PerlinNoise has not been initialized.");
        }

        float warpFrequency = 0.1f * _frequency;
        float warpAmplitude = 0.2f * (1 / _frequency);

        for (int x = 0; x < _canvas.TextureWidth; x++)
        {
            for (int y = 0; y < _canvas.TextureHeight; y++)
            {
                float baseX = x * _frequency;
                float baseY = y * _frequency;

                if (_domainWarpEnabled)
                {
                    float warpX = x * warpFrequency;
                    float warpY = y * warpFrequency;

                    float warpedX = _perlinNoise.FractalBrownianMotionNoise (warpX + 211f, warpY + 163f, 4, 2f, 0.5f);
                    float warpedY = _perlinNoise.FractalBrownianMotionNoise (warpX + 127f, warpY + 107f, 4, 2f, 0.5f);

                    baseX += warpedX * warpAmplitude;
                    baseY += warpedY * warpAmplitude;
                }

                float value = _fractalBrownianMotionEnabled
                    ? _perlinNoise.FractalBrownianMotionNoise (baseX, baseY, 4, 2f, 0.5f)
                    : _perlinNoise.Noise (baseX, baseY);

                _canvas.SetPixel (x, y, Color.Lerp (Color.Black, Color.White, value));
            }
        }
    }

    private void NextMap (object? sender, KeyboardEventArgs eventArgs)
    {
        _perlinNoise = new PerlinNoise (Environment.TickCount);

        GeneratePerlinNoise ();
    }

    public void SetFrequency (float frequency) => _frequency = float.Clamp (frequency, _minFrequency, _maxFrequency);

    private void ToggleDomainWarp (object? sender, KeyboardEventArgs eventArgs) => EnableDomainWarp (!_domainWarpEnabled);

    private void EnableDomainWarp (bool enable)
    {
        if (_domainWarpEnabled != enable)
        {
            _domainWarpEnabled = enable;

            GeneratePerlinNoise ();
        }
    }

    private void ToggleFractal (object? sender, KeyboardEventArgs eventArgs) => EnableFractalBrownianMotion (!_fractalBrownianMotionEnabled);

    private void EnableFractalBrownianMotion (bool enable)
    {
        if (_fractalBrownianMotionEnabled != enable)
        {
            _fractalBrownianMotionEnabled = enable;

            GeneratePerlinNoise ();
        }
    }
}
