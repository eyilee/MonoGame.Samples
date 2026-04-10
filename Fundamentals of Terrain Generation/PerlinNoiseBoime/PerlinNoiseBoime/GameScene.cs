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

    private PerlinNoise? _perlinNoiseTemperatur;
    private PerlinNoise? _perlinNoiseHumidity;
    private float _frequency = 0.05f;
    private readonly float _frequencyStep = 0.005f;

    public override void LoadContent ()
    {
        _canvas = new Canvas (GraphicsDevice, Core.ScreenWidth / PixelSize, Core.ScreenHeight / PixelSize, PixelSize);
        _canvas.SetOffset ((Core.ScreenWidth - _canvas.PixelWidth) / 2, (Core.ScreenHeight - _canvas.PixelHeight) / 2);

        _perlinNoiseTemperatur = new PerlinNoise (DateTime.Now.Second);
        _perlinNoiseHumidity = new PerlinNoise (DateTime.Now.Second + 1);

        GeneratePerlinNoiseBiome ();

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

        if (_perlinNoiseTemperatur is null || _perlinNoiseHumidity is null)
        {
            throw new InvalidOperationException ("PerlinNoise has not been initialized.");
        }

        float temperatureFrequency = 0.05f * _frequency;
        float humidityFrequency = 0.2f * _frequency;
        float warpFrequency = 0.5f;
        float warpAmplitude = 0.35f;

        for (int x = 0; x < _canvas.Width; x++)
        {
            for (int y = 0; y < _canvas.Height; y++)
            {
                float temperature = _perlinNoiseTemperatur.DomainWarpedNoise (x * temperatureFrequency, y * temperatureFrequency, 6, warpFrequency, warpAmplitude, 3);
                temperature = temperature.Gain (0.4f);

                float humidity = _perlinNoiseHumidity.DomainWarpedNoise (x * humidityFrequency, y * humidityFrequency, 6, warpFrequency, warpAmplitude, 3);
                humidity = humidity.Bias (0.4f);

                Biome biome = BiomeResolver.Resolve (temperature, humidity);

                float totalWeight = biome.PrimaryWeight + biome.SecondaryWeight;
                if (totalWeight < 0.001f)
                {
                    _canvas.SetPixel (x, y, biome.PrimaryColor);
                    continue;
                }

                Color color = ColorUtility.LerpHSV (biome.PrimaryColor, biome.SecondaryColor, 1f - biome.PrimaryWeight / totalWeight);

                ColorUtility.RGBToHSV (color, out float h, out float s, out float v);

                if (s < 0.05f)
                {
                    color = biome.PrimaryColor;
                }
                else
                {
                    s = float.Max (s, 0.25f);
                    color = ColorUtility.HSVToRGB (h, s, v);
                }

                _canvas.SetPixel (x, y, color);
            }
        }
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
