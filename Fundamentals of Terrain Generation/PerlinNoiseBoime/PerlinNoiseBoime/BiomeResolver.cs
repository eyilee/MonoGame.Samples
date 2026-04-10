using Microsoft.Xna.Framework;
using MonoGame.Samples.Library;
using System.Collections.Generic;

namespace MonoGame.Samples.PerlinNoiseBiome;

public class BiomeResolver
{
    private static readonly Dictionary<BiomeType, BiomeDefinition> s_biomeDefinitions = new ()
    {
        { BiomeType.Tundra, new BiomeDefinition (0.1f, 0.2f, 0.25f, new Color (0.85f, 0.88f, 0.92f)) },
        { BiomeType.Taiga, new BiomeDefinition (0.25f, 0.5f, 0.25f, new Color (0.30f, 0.45f, 0.35f)) },
        { BiomeType.Grassland, new BiomeDefinition (0.5f, 0.3f, 0.25f, new Color (0.55f, 0.75f, 0.35f)) },
        { BiomeType.TemperateForest, new BiomeDefinition (0.5f, 0.6f, 0.25f, new Color (0.20f, 0.55f, 0.25f)) },
        { BiomeType.Desert, new BiomeDefinition (0.9f, 0.1f, 0.3f, new Color (0.88f, 0.78f, 0.50f)) },
        { BiomeType.Savanna, new BiomeDefinition (0.8f, 0.4f, 0.25f, new Color (0.70f, 0.65f, 0.30f)) },
        { BiomeType.TropicalRainforest, new BiomeDefinition (0.9f, 0.8f, 0.3f, new Color (0.10f, 0.45f, 0.20f)) }
    };

    public static Biome Resolve (float temperature, float humidity)
    {
        Biome biome = default;

        foreach ((BiomeType biomeType, BiomeDefinition biomeDefinition) in s_biomeDefinitions)
        {
            float dx = temperature - biomeDefinition.Temperature;
            float dy = humidity - biomeDefinition.Humidity;
            float distance = dx * dx + dy * dy;

            float weight = float.Exp (-distance * 6f);
            if (weight <= 0f)
            {
                continue;
            }

            if (weight > biome.PrimaryWeight)
            {
                biome.Secondary = biome.Primary;
                biome.SecondaryWeight = biome.PrimaryWeight;
                biome.SecondaryColor = biome.PrimaryColor;
                biome.Primary = biomeType;
                biome.PrimaryWeight = weight;
                biome.PrimaryColor = biomeDefinition.Color;
            }
        }

        return biome;
    }
}