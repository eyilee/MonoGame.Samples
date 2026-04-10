using Microsoft.Xna.Framework;

namespace MonoGame.Samples.PerlinNoiseBiome;

public struct BiomeDefinition (float temperature, float humidity, float radius, Color color)
{
    public float Temperature = temperature;
    public float Humidity = humidity;
    public float Radius = radius;
    public Color Color = color;
}
