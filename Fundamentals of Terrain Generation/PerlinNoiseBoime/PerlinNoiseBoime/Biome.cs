using Microsoft.Xna.Framework;

namespace MonoGame.Samples.PerlinNoiseBiome;

public struct Biome
{
    public BiomeType Primary;
    public float PrimaryWeight;
    public Color PrimaryColor;
    public BiomeType Secondary;
    public float SecondaryWeight;
    public Color SecondaryColor;
}
