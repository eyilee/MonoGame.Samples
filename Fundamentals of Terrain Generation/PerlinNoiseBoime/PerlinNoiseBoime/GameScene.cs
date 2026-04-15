using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Samples.Library;
using MonoGame.Samples.Library.Canvas;
using MonoGame.Samples.Library.Input;
using System;

namespace MonoGame.Samples.PerlinNoiseBiome;

public class GameScene : Scene
{
    private const int PixelSize = 2;

    private Canvas? _canvas;
    private DisplayMode _displayMode;

    private PerlinNoise? _perlinNoiseTmacro;
    private PerlinNoise? _perlinNoiseHmacro;
    private PerlinNoise? _perlinNoiseTemperatur;
    private PerlinNoise? _perlinNoiseHumidity;
    private float _frequency = 1f;
    private readonly float _frequencyStep = 0.005f;

    public override void Initialize ()
    {
        GameUI.Initialize ();

        base.Initialize ();
    }

    public override void LoadContent ()
    {
        _canvas = new Canvas (GraphicsDevice, Core.ScreenWidth / PixelSize, Core.ScreenHeight / PixelSize, PixelSize);
        _canvas.SetOffset ((Core.ScreenWidth - _canvas.PixelWidth) / 2, (Core.ScreenHeight - _canvas.PixelHeight) / 2);

        _perlinNoiseTmacro = new PerlinNoise (DateTime.Now.Second);
        _perlinNoiseHmacro = new PerlinNoise (DateTime.Now.Second + 1);
        _perlinNoiseTemperatur = new PerlinNoise (DateTime.Now.Second + 2);
        _perlinNoiseHumidity = new PerlinNoise (DateTime.Now.Second + 3);

        GeneratePerlinNoiseBiome ();

        Input.Keyboard.SubscribePressed (Keys.N, NextMap);
        Input.Keyboard.SubscribePressed (Keys.T, ToggleMode);
        Input.Keyboard.SubscribePressed (Keys.Add, IncreaseFrequency);
        Input.Keyboard.SubscribePressed (Keys.Subtract, DecreaseFrequency);
        Input.Mouse.SubscribeWheelMoved (ChangeFrequency);

        base.LoadContent ();
    }

    public override void UnloadContent ()
    {
        Input.Keyboard.UnsubscribePressed (Keys.Add, IncreaseFrequency);
        Input.Keyboard.UnsubscribePressed (Keys.Subtract, DecreaseFrequency);

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

    private void GeneratePerlinNoiseBiome ()
    {
        if (_canvas is null)
        {
            throw new InvalidOperationException ("Canvas has not been initialized.");
        }

        if (_perlinNoiseTmacro is null || _perlinNoiseHmacro is null || _perlinNoiseTemperatur is null || _perlinNoiseHumidity is null)
        {
            throw new InvalidOperationException ("PerlinNoise has not been initialized.");
        }

        float macroFrequency = 0.003f;
        float temperatureFrequency = 0.03f * _frequency;
        float humidityFrequency = 0.03f * _frequency;

        float warpScale = 20f;

        for (int x = 0; x < _canvas.Width; x++)
        {
            for (int y = 0; y < _canvas.Height; y++)
            {
                float tmacro = _perlinNoiseTmacro.FractalBrownianMotionNoise (x * macroFrequency, y * macroFrequency, 4, 2f, 0.5f);
                tmacro = tmacro * 2f - 1f;

                float hmacro = _perlinNoiseHmacro.FractalBrownianMotionNoise (x * macroFrequency, y * macroFrequency, 4, 2f, 0.5f);
                hmacro = hmacro * 2f - 1f;

                float tx = x + tmacro * warpScale;
                float ty = y + tmacro * warpScale;

                float hx = x + hmacro * warpScale;
                float hy = y + hmacro * warpScale;

                float temperature = _perlinNoiseTemperatur.FractalBrownianMotionNoise (tx * temperatureFrequency, ty * temperatureFrequency, 4, 2f, 0.5f);
                temperature.SmoothStep ();

                float latitude = (float)y / _canvas.Height;
                temperature = float.Lerp (temperature, 1f - float.Abs (latitude * 2f - 1f), 0.4f);

                float humidity = _perlinNoiseHumidity.FractalBrownianMotionNoise (hx * humidityFrequency, hy * humidityFrequency, 4, 2f, 0.5f);
                humidity = humidity.SmoothStep ();

                switch (_displayMode)
                {
                    case DisplayMode.Biome:
                        _canvas.SetPixel (x, y, BiomeResolver.ResolveColor (temperature, humidity));
                        break;
                    case DisplayMode.Temperature:
                        _canvas.SetPixel (x, y, Color.Lerp (Color.Blue, Color.Red, temperature));
                        break;
                    case DisplayMode.Humidity:
                        _canvas.SetPixel (x, y, Color.Lerp (Color.Brown, Color.Green, humidity));
                        break;
                    case DisplayMode.TemperatureMacro:
                        _canvas.SetPixel (x, y, Color.Lerp (Color.Black, Color.White, (tmacro + 1f) * 0.5f));
                        break;
                    case DisplayMode.HumidityMacro:
                        _canvas.SetPixel (x, y, Color.Lerp (Color.Black, Color.White, (hmacro + 1f) * 0.5f));
                        break;
                }
            }
        }
    }

    private void NextMap (object? sender, KeyboardEventArgs eventArgs)
    {
        _perlinNoiseTmacro = new PerlinNoise (DateTime.Now.Second);
        _perlinNoiseTemperatur = new PerlinNoise (DateTime.Now.Second + 1);
        _perlinNoiseHumidity = new PerlinNoise (DateTime.Now.Second + 2);

        GeneratePerlinNoiseBiome ();
    }

    private void ToggleMode (object? sender, KeyboardEventArgs e)
    {
        _displayMode = (DisplayMode)(((int)_displayMode + 1) % Enum.GetValues (typeof (DisplayMode)).Length);
        GeneratePerlinNoiseBiome ();
    }

    private void IncreaseFrequency (object? sender, KeyboardEventArgs eventArgs)
    {
        _frequency += _frequencyStep;
        GeneratePerlinNoiseBiome ();
    }

    private void DecreaseFrequency (object? sender, KeyboardEventArgs eventArgs)
    {
        _frequency -= _frequencyStep;
        GeneratePerlinNoiseBiome ();
    }

    private void ChangeFrequency (object? sender, MouseEventArgs eventArgs)
    {
        _frequency += _frequencyStep * float.Sign (eventArgs.ScrollWheelDelta) * float.Ceiling (float.Abs (eventArgs.ScrollWheelDelta) * 0.01f);
        GeneratePerlinNoiseBiome ();
    }
}
