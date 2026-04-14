using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MonoGame.Samples.PerlinNoiseBiome;

public struct BiomeDefinition
{
    public BiomeType Type;

    public float MinTemperature;
    public float MaxTemperature;
    public float IdealTemperature;

    public float MinHumidity;
    public float MaxHumidity;
    public float IdealHumidity;

    public float Weight;

    public Color Color;

    public static readonly Dictionary<BiomeType, BiomeDefinition> Definitions = new ()
    {
        {
            BiomeType.TropicalDesert,
            new BiomeDefinition ()
            {
                Type = BiomeType.TropicalDesert,
                MinTemperature = 0.75f,
                MaxTemperature = 1.00f,
                IdealTemperature = 0.95f,
                MinHumidity  = 0.00f,
                MaxHumidity  = 0.20f,
                IdealHumidity = 0.05f,
                Weight = 1.2f,
                Color = new Color (237, 201, 175)
            }
        },

        {
            BiomeType.TemperateDesert,
            new ()
            {
                Type = BiomeType.TemperateDesert,
                MinTemperature = 0.40f,
                MaxTemperature = 0.75f,
                IdealTemperature = 0.55f,
                MinHumidity  = 0.00f,
                MaxHumidity  = 0.20f,
                IdealHumidity = 0.10f,
                Weight = 1.0f,
                Color = new Color (210, 180, 140)
            }
        },

        {
            BiomeType.ColdDesert,
            new ()
            {
                Type = BiomeType.ColdDesert,
                MinTemperature = 0.00f,
                MaxTemperature = 0.35f,
                IdealTemperature = 0.20f,
                MinHumidity  = 0.00f,
                MaxHumidity  = 0.20f,
                IdealHumidity = 0.10f,
                Weight = 0.9f,
                Color = new Color (176, 196, 222)
            }
        },

        {
            BiomeType.Savanna,
            new ()
            {
                Type = BiomeType.Savanna,
                MinTemperature = 0.70f,
                MaxTemperature = 1.00f,
                IdealTemperature = 0.85f,
                MinHumidity  = 0.35f,
                MaxHumidity  = 0.65f,
                IdealHumidity = 0.50f,
                Weight = 1.0f,
                Color = new Color (189, 183, 107)
            }
        },

        {
            BiomeType.TemperateGrassland,
            new ()
            {
                Type = BiomeType.TemperateGrassland,
                MinTemperature = 0.40f,
                MaxTemperature = 0.70f,
                IdealTemperature = 0.55f,
                MinHumidity  = 0.25f,
                MaxHumidity  = 0.55f,
                IdealHumidity = 0.40f,
                Weight = 1.0f,
                Color = new Color (140, 170, 90)
            }
        },

        {
            BiomeType.Tundra,
            new ()
            {
                Type = BiomeType.Tundra,
                MinTemperature = 0.00f,
                MaxTemperature = 0.25f,
                IdealTemperature = 0.10f,
                MinHumidity  = 0.25f,
                MaxHumidity  = 0.60f,
                IdealHumidity = 0.40f,
                Weight = 1.0f,
                Color = new Color (198, 226, 255)
            }
        },

        {
            BiomeType.TropicalRainforest,
            new ()
            {
                Type = BiomeType.TropicalRainforest,
                MinTemperature = 0.75f,
                MaxTemperature = 1.00f,
                IdealTemperature = 0.90f,
                MinHumidity  = 0.75f,
                MaxHumidity  = 1.00f,
                IdealHumidity = 0.90f,
                Weight = 1.0f,
                Color = new Color (42, 138, 64)
            }
        },

        {
            BiomeType.TemperateForest,
            new ()
            {
                Type = BiomeType.TemperateForest,
                MinTemperature = 0.45f,
                MaxTemperature = 0.75f,
                IdealTemperature = 0.60f,
                MinHumidity  = 0.55f,
                MaxHumidity  = 0.85f,
                IdealHumidity = 0.70f,
                Weight = 1.2f,
                Color = new Color (80, 160, 80)
            }
        },

        {
            BiomeType.Taiga,
            new ()
            {
                Type = BiomeType.Taiga,
                MinTemperature = 0.20f,
                MaxTemperature = 0.45f,
                IdealTemperature = 0.30f,
                MinHumidity  = 0.45f,
                MaxHumidity  = 0.75f,
                IdealHumidity = 0.60f,
                Weight = 1.0f,
                Color = new Color (110, 130, 130)
            }
        }
    };
}