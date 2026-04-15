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
        // HOT BAND

        {
            BiomeType.TropicalDesert,
            new ()
            {
                Type = BiomeType.TropicalDesert,
                MinTemperature = 0.75f,
                MaxTemperature = 1.00f,
                IdealTemperature = 0.92f,
                MinHumidity = 0.00f,
                MaxHumidity = 0.20f,
                IdealHumidity = 0.05f,
                Weight = 1.1f,
                Color = new Color (233, 196, 150)
            }
        },

        {
            BiomeType.Savanna,
            new ()
            {
                Type = BiomeType.Savanna,
                MinTemperature = 0.75f,
                MaxTemperature = 1.00f,
                IdealTemperature = 0.85f,
                MinHumidity = 0.20f,
                MaxHumidity = 0.50f,
                IdealHumidity = 0.35f,
                Weight = 0.9f,
                Color = new Color (176, 160, 80)
            }
        },

        {
            BiomeType.TropicalSeasonalForest,
            new ()
            {
                Type = BiomeType.TropicalSeasonalForest,
                MinTemperature = 0.75f,
                MaxTemperature = 1.00f,
                IdealTemperature = 0.88f,
                MinHumidity = 0.50f,
                MaxHumidity = 0.80f,
                IdealHumidity = 0.65f,
                Weight = 1.0f,
                Color = new Color (70, 140, 85)
            }
        },

        {
            BiomeType.TropicalRainForest,
            new ()
            {
                Type = BiomeType.TropicalRainForest,
                MinTemperature = 0.75f,
                MaxTemperature = 1.00f,
                IdealTemperature = 0.90f,
                MinHumidity = 0.80f,
                MaxHumidity = 1.00f,
                IdealHumidity = 0.92f,
                Weight = 1.1f,
                Color = new Color (30, 110, 60)
            }
        },

        // WARM BAND

        {
            BiomeType.TemperateDesert,
            new ()
            {
                Type = BiomeType.TemperateDesert,
                MinTemperature = 0.45f,
                MaxTemperature = 0.75f,
                IdealTemperature = 0.55f,
                MinHumidity = 0.00f,
                MaxHumidity = 0.20f,
                IdealHumidity = 0.10f,
                Weight = 1.0f,
                Color = new Color (200, 170, 130)
            }
        },

        {
            BiomeType.TemperateGrassland,
            new ()
            {
                Type = BiomeType.TemperateGrassland,
                MinTemperature = 0.45f,
                MaxTemperature = 0.75f,
                IdealTemperature = 0.55f,
                MinHumidity = 0.20f,
                MaxHumidity = 0.50f,
                IdealHumidity = 0.35f,
                Weight = 1.0f,
                Color = new Color (130, 160, 85)
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
                MinHumidity = 0.50f,
                MaxHumidity = 0.80f,
                IdealHumidity = 0.65f,
                Weight = 1.1f,
                Color = new Color (70, 130, 70)
            }
        },

        {
            BiomeType.TemperateRainforest,
            new ()
            {
                Type = BiomeType.TemperateRainforest,
                MinTemperature = 0.45f,
                MaxTemperature = 0.75f,
                IdealTemperature = 0.60f,
                MinHumidity = 0.80f,
                MaxHumidity = 1.00f,
                IdealHumidity = 0.90f,
                Weight = 1.05f,
                Color = new Color (50, 120, 90)
            }
        },

        // COOL BAND

        {
            BiomeType.ColdDesert,
            new ()
            {
                Type = BiomeType.ColdDesert,
                MinTemperature = 0.20f,
                MaxTemperature = 0.45f,
                IdealTemperature = 0.30f,
                MinHumidity = 0.00f,
                MaxHumidity = 0.20f,
                IdealHumidity = 0.10f,
                Weight = 0.95f,
                Color = new Color (170, 180, 190)
            }
        },

        {
            BiomeType.Steppe,
            new ()
            {
                Type = BiomeType.Steppe,
                MinTemperature = 0.20f,
                MaxTemperature = 0.45f,
                IdealTemperature = 0.30f,
                MinHumidity = 0.20f,
                MaxHumidity = 0.45f,
                IdealHumidity = 0.30f,
                Weight = 1.0f,
                Color = new Color (140, 155, 100)
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
                MinHumidity = 0.45f,
                MaxHumidity = 0.75f,
                IdealHumidity = 0.60f,
                Weight = 1.0f,
                Color = new Color (90, 120, 120)
            }
        },

        // COLD BAND

        {
            BiomeType.PolarDesert,
            new ()
            {
                Type = BiomeType.PolarDesert,
                MinTemperature = 0.00f,
                MaxTemperature = 0.20f,
                IdealTemperature = 0.05f,
                MinHumidity = 0.00f,
                MaxHumidity = 0.25f,
                IdealHumidity = 0.10f,
                Weight = 0.9f,
                Color = new Color (210, 220, 230)
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
                MinHumidity = 0.25f,
                MaxHumidity = 0.70f,
                IdealHumidity = 0.45f,
                Weight = 1.0f,
                Color = new Color (170, 200, 200)
            }
        }
    };
}