using Microsoft.Xna.Framework;
using System;

namespace MonoGame.Samples.PerlinNoiseBiome;

public class BiomeResolver
{
    public static Biome ResolveSmooth (float temperature, float humidity)
    {
        float totalWeight = 0f;
        float bestScore = float.MinValue;
        Biome biome = default;

        foreach ((BiomeType type, BiomeDefinition definition) in BiomeDefinition.Definitions)
        {
            float rangeWeight = RangeWeight (temperature, humidity, definition);
            if (rangeWeight <= 0f)
            {
                continue;
            }

            float idealWeight = IdealWeight (temperature, humidity, definition);

            float score = rangeWeight * idealWeight * definition.Weight;

            totalWeight += score;

            if (score > bestScore)
            {
                bestScore = score;
                biome.Type = definition.Type;
                biome.Color = definition.Color;
            }
        }

        if (totalWeight == 0f)
        {
            BiomeType type = Resolve (temperature, humidity);
            if (!BiomeDefinition.Definitions.TryGetValue (type, out BiomeDefinition definition))
            {
                throw new InvalidOperationException ($"Biome definition not found for type {type}");
            }

            biome.Type = definition.Type;
            biome.Color = definition.Color;
        }

        return biome;
    }

    public static Color ResolveColor (float temperature, float humidity)
    {
        float bestScore = float.MinValue;
        Color bestColor = Color.White;

        foreach ((BiomeType type, BiomeDefinition definition) in BiomeDefinition.Definitions)
        {
            float weight = RangeWeight (temperature, humidity, definition)
                * IdealWeight (temperature, humidity, definition)
                * definition.Weight;

            if (weight <= 0f)
            {
                continue;
            }

            if (weight > bestScore)
            {
                bestScore = weight;
                bestColor = definition.Color;
            }
        }

        if (bestScore == float.MinValue)
        {
            BiomeType type = Resolve (temperature, humidity);
            if (!BiomeDefinition.Definitions.TryGetValue (type, out BiomeDefinition definition))
            {
                throw new InvalidOperationException ($"Biome definition not found for type {type}");
            }

            return definition.Color;
        }

        return bestColor;
    }

    private static BiomeType Resolve (float temperature, float humidity)
    {
        if (temperature < 0.25f)
        {
            if (humidity < 0.20f)
            {
                return BiomeType.ColdDesert;
            }

            return BiomeType.Tundra;
        }

        if (temperature < 0.45f)
        {
            if (humidity < 0.20f)
            {
                return BiomeType.ColdDesert;
            }
            else if (humidity < 0.50f)
            {
                return BiomeType.TemperateGrassland;
            }

            return BiomeType.Taiga;
        }

        if (temperature < 0.75f)
        {
            if (humidity < 0.20f)
            {
                return BiomeType.TemperateDesert;
            }
            else if (humidity < 0.55f)
            {
                return BiomeType.TemperateGrassland;
            }

            return BiomeType.TemperateForest;
        }

        if (humidity < 0.20f)
        {
            return BiomeType.TropicalDesert;
        }
        else if (humidity < 0.65f)
        {
            return BiomeType.Savanna;
        }

        return BiomeType.TropicalRainForest;
    }

    private static float RangeWeight (float temperature, float humidity, in BiomeDefinition definition)
    {
        float dt = SmoothRange (temperature, definition.MinTemperature, definition.MaxTemperature);
        float dh = SmoothRange (humidity, definition.MinHumidity, definition.MaxHumidity);

        return dt * dh;
    }

    private static float SmoothRange (float value, float min, float max)
    {
        if (value < min || value > max)
        {
            return 0f;
        }

        float mid = (min + max) * 0.5f;
        float half = (max - min) * 0.5f;

        float x = 1f - float.Abs (value - mid) / half;

        return x * x;
    }

    private static float IdealWeight (float temperature, float humidity, in BiomeDefinition definition)
    {
        float dx = temperature - definition.IdealTemperature;
        float dy = humidity - definition.IdealHumidity;

        return 1f / (1f + (dx * dx + dy * dy) * 10f);
    }
}